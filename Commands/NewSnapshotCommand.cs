using AzuureSnapshotManager.Commands;
using AzuureSnapshotManager.Global;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace AzuureSnapshotManager
{
    class NewSnapshotCommand : MainVmCommandBase
    {
        public NewSnapshotCommand(MainVm vm) : base(vm) { }

        public override bool CanExecute(object parameter)
        {
            return Vm.CurrentBlob?.Blob.IsSnapshot == false;
        }

        public override async Task ExecuteInternal(object parameter)
        {
            var b = Vm.CurrentBlob.Blob as CloudBlob;
            if (b != null)
            {
                var nameAndDetails = new Views.Credentials("Snapshot name", "Snapshot description", "Create snapshot");
                if(nameAndDetails.ShowDialog() == true)
                {
                    var snap = b.Snapshot();
                    var snapHash = snap.GetTimeStampHash();
                    await ((SetMetadataCommand)Vm.SetMetadataCommand).SetSnapshotDetails(snapHash, Vm.CurrentBlob, nameAndDetails);
                    Vm.CurrentBlob.SetActiveSnapshot(snapHash);
                }
            }
            else
            {
                throw new NotSupportedException("Blob type is not supported: " + Vm.CurrentBlob.Blob.GetType().FullName);
            }
        }
    }
}
