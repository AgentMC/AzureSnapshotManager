using AzuureSnapshotManager.Global;
using AzuureSnapshotManager.Views;
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

        public override async Task ExecuteInternal(object parameter)
        {
            var nameAndDetails = new Credentials("Snapshot name", "Snapshot description", "Edit snapshot details", Vm.CurrentBlob.SnapshotTitle, Vm.CurrentBlob.SnapshotDescription);
            if (nameAndDetails.ShowDialog() == true)
            {
                await SetSnapshotDetails(Vm.CurrentBlob.Blob.GetTimeStampHash(), Vm.CurrentBlob.Parent, nameAndDetails);
            }
        }

        public async Task SetSnapshotDetails(string snapshotBlobTimeStampHash, BlobVm parentBlob, Credentials credentials)
        {
            await BreakLeaseOn(parentBlob.Blob);
            parentBlob.SetSnapshotMetadata(snapshotBlobTimeStampHash, credentials.ShortField.Text, credentials.LongField.Text);
            Vm.ReloadContainer();
        }
    }
}
