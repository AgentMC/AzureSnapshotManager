using AzuureSnapshotManager.ViewModels;
using System.Threading.Tasks;

namespace AzuureSnapshotManager.Commands
{
    class RemoveSnapshotCommand : MainVmCommandBase
    {
        public RemoveSnapshotCommand(MainVm vm) : base(vm) { }

        public override bool CanExecute(object parameter)
        {
            return Vm.CurrentBlob?.Blob.IsSnapshot == true;
        }

        public override Task<bool> ExecuteInternal(object parameter)
        {
            Vm.CurrentBlob.Blob.Delete();
            Vm.ReloadContainer();
            return True;
        }
    }
}
