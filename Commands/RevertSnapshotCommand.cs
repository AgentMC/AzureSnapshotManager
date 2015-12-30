using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace AzuureSnapshotManager
{
    class RevertSnapshotCommand : MainVmCommandBase
    {
        public RevertSnapshotCommand(MainVm vm) : base(vm) { }

        public override bool CanExecute(object parameter)
        {
            return Vm.CurrentBlob?.Blob.IsSnapshot == true;
        }

        public override async Task ExecuteInternal(object parameter)
        {
            var destination = Vm.CurrentBlob.Parent.Blob;
            await BreakLeaseOn(destination);
            //Since we're copying between snapshot and it's parent it'll actually run synchronously
            var copyId = destination.StartCopyFromBlob(Vm.CurrentBlob.Blob.SnapshotQualifiedUri);
            destination.FetchAttributes();
            if (destination.CopyState.CopyId != copyId) throw new Exception("Something went wrong. Copy state refers to wrong CopyId");
            if (destination.CopyState.Status != CopyStatus.Success) throw new Exception("Something went wrong. Copy status " + destination.CopyState.Status);
            Vm.ReloadContainer();
        }
    }
}
