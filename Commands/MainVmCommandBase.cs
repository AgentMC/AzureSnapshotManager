using AzuureSnapshotManager.ViewModels;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AzuureSnapshotManager.Commands
{
    abstract class MainVmCommandBase : ICommand
    {
        protected readonly MainVm Vm;

        public MainVmCommandBase(MainVm vm)
        {
            Vm = vm;
            Vm.CurrentBlobChanged += delegate { if (CanExecuteChanged != null) CanExecuteChanged(this, null); };
        }

        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);

        public void Execute(object parameter)
        {
            try
            {
                ExecuteInternal(parameter).Wait();
                Vm.OnCommandSucceeded();
            }
            catch (AggregateException ae)
            {
                Vm.OnError(ae.InnerException);
            }
            catch (Exception ex)
            {
                Vm.OnError(ex);
            }
        }

        public abstract Task ExecuteInternal(object parameter);

        protected async Task BreakLeaseOn(ICloudBlob destination)
        {
            if (destination.Properties.LeaseStatus == LeaseStatus.Locked)
            {
                var waitTime = destination.BreakLease(TimeSpan.FromSeconds(0));
                await Task.Delay(waitTime);
                destination.FetchAttributes();
                if (destination.Properties.LeaseStatus == LeaseStatus.Locked)
                {
                    throw new Exception("Couldn't break lease on blob.");
                }
            }
        }

        protected Task Nop()
        {
            return Task.Run(delegate { });
        }
    }
}
