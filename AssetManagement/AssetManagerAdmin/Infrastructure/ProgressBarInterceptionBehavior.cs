using AssetManagerAdmin.Model;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AssetManagerAdmin.Infrastructure;

namespace AssetManagerAdmin
{
    // Interception and how to use it:
    // https://msdn.microsoft.com/en-us/library/dn178466%28v=pandp.30%29.aspx

    class ProgressBarInterceptionBehavior : IInterceptionBehavior
    {
        private readonly IMessenger _messenger;
        private readonly ConcurrentDictionary<Type, Func<Task, IMethodInvocation, Task>> wrapperCreators 
            = new ConcurrentDictionary<Type, Func<Task, IMethodInvocation, Task>>();

        public ProgressBarInterceptionBehavior(IMessenger messenger)
        {
            if (messenger == null)
                throw new ArgumentNullException("messenger");
            _messenger = messenger;
        }

        public bool WillExecute
        {
            get { return true; }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            // Before invoking the method on the original target.
            var attribute = input.MethodBase.GetCustomAttribute<ProgressBarAttribute>();
            if (attribute != null)
                _messenger.Send(attribute.Message, AppActions.LoadingStarted);

            // Invoke the next behavior in the chain.
            var result = getNext()(input, getNext);

            var method = input.MethodBase as MethodInfo;
            if (attribute != null
              && result.ReturnValue != null
              && method != null
              && typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                // If this method returns a Task, override the original return value
                var task = (Task)result.ReturnValue;
                return input.CreateMethodReturn(
                    GetWrapperCreator(method.ReturnType)(task, input), result.Outputs);
            }

            return result;
        }

        // Below goes some heavy stuff taken from this article:
        // https://msdn.microsoft.com/en-us/magazine/dn574805.aspx

        private Func<Task, IMethodInvocation, Task> GetWrapperCreator(Type taskType)
        {
            return wrapperCreators.GetOrAdd(
              taskType,
              (Type t) =>
              {
                  if (t == typeof(Task))
                  {
                      return CreateWrapperTask;
                  }
                  else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>))
                  {
                      return (Func<Task, IMethodInvocation, Task>)this.GetType()
                          .GetMethod("CreateGenericWrapperTask",
                             BindingFlags.Instance | BindingFlags.NonPublic)
                          .MakeGenericMethod(new Type[] { t.GenericTypeArguments[0] })
                          .CreateDelegate(typeof(Func<Task, IMethodInvocation, Task>), this);
                  }
                  else
                  {
                      // Other cases are not supported
                      return (task, _) => task;
                  }
              });
        }

        private Task CreateWrapperTask(Task task, IMethodInvocation input)
        {
            var tcs = new TaskCompletionSource<bool>();
            task.ContinueWith(
              t =>
              {
                  // always complete loading
                  _messenger.Send("", AppActions.LoadingCompleted);

                  if (t.IsFaulted)
                  {
                      var e = t.Exception.InnerException;
                      // show error notification
                      _messenger.Send(new StatusMessage(e));
                      tcs.SetException(e);
                  }
                  else if (t.IsCanceled)
                  {
                      tcs.SetCanceled();
                  }
                  else
                  {
                      tcs.SetResult(true);
                  }
              },
              TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        private Task CreateGenericWrapperTask<T>(Task task, IMethodInvocation input)
        {
            return DoCreateGenericWrapperTask<T>((Task<T>)task, input);
        }

        private async Task<T> DoCreateGenericWrapperTask<T>(Task<T> task,
          IMethodInvocation input)
        {
            try
            {
                T value = await task.ConfigureAwait(false);
                return value;
            }
            catch (Exception e)
            {
                // show error notification
                _messenger.Send(new StatusMessage(e));
                throw;
            }
            finally
            {
                // always complete loading
                _messenger.Send("", AppActions.LoadingCompleted);
            }
        }
    }
}
