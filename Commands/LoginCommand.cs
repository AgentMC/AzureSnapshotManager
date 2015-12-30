namespace AzuureSnapshotManager
{
    class LoginCommand : MainVmCommandBase
    {
        public LoginCommand(MainVm vm) : base(vm) { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void ExecuteInternal(object parameter)
        {
            var cred = new Views.Credentials("Storage account name:", "Storage account key:", "Connect to Storage account");
            if (cred.ShowDialog() == true)
            {
                Vm.Login(cred.ShortField.Text, cred.LongField.Text);
            }
        }
    }
}
