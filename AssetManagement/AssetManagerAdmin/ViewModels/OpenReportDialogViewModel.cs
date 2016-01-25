using System;
using System.Collections.Generic;
using AppFramework.Entities;
using AppFramework.Reports.Services;
using AssetManagerAdmin.Infrastructure;
using GalaSoft.MvvmLight.Command;

namespace AssetManagerAdmin.ViewModels
{
    public class OpenReportDialogViewModel : ToolkitViewModelBase
    {
        public event Action OnReportSelected;

        public const string ReportsListPropertyName = "ReportsList";
        private List<Report> _reportsList;

        public List<Report> ReportsList
        {
            get { return _reportsList; }

            set
            {
                _reportsList = value;
                RaisePropertyChanged(ReportsListPropertyName);
            }
        }

        private RelayCommand<Report> _viewReportCommand;

        public RelayCommand<Report> ViewReportCommand
        {
            get
            {
                return _viewReportCommand ??
                       (_viewReportCommand =
                           new RelayCommand<Report>(ExecuteViewReportCommand, model => true));
            }
        }

        private void ExecuteViewReportCommand(Report reportModel)
        {
            SelectedReport = reportModel;

            if (OnReportSelected != null)
                OnReportSelected();
        }

        private RelayCommand<Report> _deleteReportCommand;

        public RelayCommand<Report> DeleteReportCommand
        {
            get
            {
                return _deleteReportCommand ??
                       (_deleteReportCommand =
                           new RelayCommand<Report>(ExecuteDeleteReportCommand, model => true));
            }
        }

        public Report SelectedReport { get; private set; }

        private void ExecuteDeleteReportCommand(Report reportModel)
        {
            Api.DeleteReport(reportModel.ReportUid).ContinueWith(e =>
            {
                ReportsList.Remove(reportModel);
                RaisePropertyChanged(ReportsListPropertyName);
            });
        }

        public OpenReportDialogViewModel(
            ICustomReportService reportsService,
            IAppContext context)
            : base(context)
        {
            // ToDo get real user id
            ReportsList = reportsService.GetAllReports(1);
        }
    }
}