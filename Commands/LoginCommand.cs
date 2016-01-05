using AzuureSnapshotManager.ViewModels;
using System.Threading.Tasks;

namespace AzuureSnapshotManager.Commands
{
    class LoginCommand : MainVmCommandBase
    {
        public LoginCommand(MainVm vm) : base(vm) { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task ExecuteInternal(object parameter)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var cred = new Views.Credentials("Storage account name:", "Storage account key:", "Connect to Storage account");
            if (cred.ShowDialog() == true)
            {
                Vm.Login(cred.ShortField.Text, cred.LongField.Text);
            }
        }

    }
}
