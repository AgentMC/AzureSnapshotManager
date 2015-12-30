using AzuureSnapshotManager.Commands;
using AzuureSnapshotManager.Global;
using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace AzuureSnapshotManager
{
    class NewSnapshotCommand : MainVmCommandBase
    {
        public NewSnapshotCommand(MainVm vm) : base(vm) { }

        public override bool CanExecute(object parameter)
        {
            return Vm.CurrentBlob?.Blob.IsSnapshot == false;
        }

        public override async void ExecuteInternal(object parameter)
        {
            var b = Vm.CurrentBlob.Blob as CloudBlob;
            if (b != null)
            {
                var nameAndDetails = new Views.Credentials("Snapshot name", "Snapshot description", "Create snapshot");
                if(nameAndDetails.ShowDialog() == true)
                {
                    var snap = b.Snapshot();
                    await ((SetMetadataCommand)Vm.SetMetadataCommand).SetMetadataCore(snap.GetTimeStampHash(), Vm.CurrentBlob.Blob, nameAndDetails);
                }
            }
            else
            {
                throw new NotSupportedException("Blob type is not supported: " + Vm.CurrentBlob.Blob.GetType().FullName);
            }
        }
    }
}
