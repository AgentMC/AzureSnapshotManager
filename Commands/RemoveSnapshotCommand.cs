﻿namespace AzuureSnapshotManager
{
    class RemoveSnapshotCommand : MainVmCommandBase
    {
        public RemoveSnapshotCommand(MainVm vm) : base(vm) { }

        public override bool CanExecute(object parameter)
        {
            return Vm.CurrentBlob?.Blob.IsSnapshot == true;
        }

        public override void ExecuteInternal(object parameter)
        {
            Vm.CurrentBlob.Blob.Delete();
            Vm.ReloadContainer();
        }
    }
}
