using System;
using System.Diagnostics;
using AppFramework.Core;
using AppFramework.Core.AC.Providers;
using AppFramework.DataProxy;
using Common.Logging;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Impl;
using System.Configuration;
using System.ServiceProcess;

namespace AssetManagementBatch
{
	public partial class BatchService : ServiceBase
	{
		private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        private readonly IScheduler _scheduler;
        private readonly IUnityContainer _container;

		public BatchService()
		{
			InitializeComponent();
			this.ServiceName = ConfigurationManager.AppSettings.Get("ServiceName");

		    // setup container
            _container = new UnityContainer();
            _container.RegisterType<IUnitOfWork, UnitOfWork>(
                        new PerResolveLifetimeManager(),
                        new InjectionFactory(c => new UnitOfWork()))
                      .RegisterType<IAuthenticationStorageProvider,
                        InMemoryAuthenticationStorageProvider>()
                      .AddNewExtension<CommonConfiguration>();

            // setup Quartz
            var schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler();
            _scheduler.JobFactory = new UnityJobFactory(_container);
            _scheduler.ListenerManager.AddJobListener(new ExceptionOccuredJobListener(_logger));

            var jobIndexing = JobBuilder.Create<Jobs.RunBatchJob>()
                                       .WithIdentity("Run Batch Job")
                                       .Build();
            var triggerIndexing = TriggerBuilder.Create()
                                                .WithIdentity("Run Batch Job Trigger")
                                                .StartNow()
                                                .WithSimpleSchedule(x => x.RepeatForever().WithIntervalInSeconds(20))
                                                .Build();
            _scheduler.ScheduleJob(jobIndexing, triggerIndexing);
		}

		protected override void OnStart(string[] args)
		{
            _scheduler.Start();
		}

	    protected override void OnPause()
	    {
	        base.OnPause();
            _scheduler.Standby();
	    }

	    protected override void OnStop()
	    {
	        base.OnStop();
            _scheduler.Shutdown();
	    }

	    protected override void OnShutdown()
	    {
	        base.OnShutdown();
            _scheduler.Shutdown();
            _container.Dispose();
	    }
	}
}