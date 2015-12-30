using AzuureSnapshotManager.Global;
using AzuureSnapshotManager.Views;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace AzuureSnapshotManager.Commands
{
    class SetMetadataCommand : MainVmCommandBase
    {
        public SetMetadataCommand(MainVm vm) : base(vm) { }

        public override bool CanExecute(object parameter)
        {
            return Vm.CurrentBlob?.Blob.IsSnapshot == true;
        }

        public override async void ExecuteInternal(object parameter)
        {
            var nameAndDetails = new Credentials("Snapshot name", "Snapshot description", "Edit snapshot details", Vm.CurrentBlob.SnapshotTitle, Vm.CurrentBlob.SnapshotDescription);
            if (nameAndDetails.ShowDialog() == true)
            {
                await SetMetadataCore(Vm.CurrentBlob.Blob.GetTimeStampHash(), Vm.CurrentBlob.Parent.Blob, nameAndDetails);
            }
        }

        public async Task SetMetadataCore(string snapshotTimeStampHash, ICloudBlob parentBlob, Credentials credentials)
        {
            await BreakLeaseOn(parentBlob);
            SetMetadataKey(parentBlob, Constants.KeySnapshotName + snapshotTimeStampHash, credentials.ShortField.Text);
            SetMetadataKey(parentBlob, Constants.KeySnapshotDesc + snapshotTimeStampHash, credentials.LongField.Text);
            parentBlob.SetMetadata();
            Vm.ReloadContainer();
        }

        private void SetMetadataKey(ICloudBlob blob, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                blob.Metadata[key] = value;
            }
            else if (blob.Metadata.ContainsKey(key))
            {
                blob.Metadata.Remove(key);
            }
        }
    }
}
