using AzuureSnapshotManager.Global;
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

        public override Task ExecuteInternal(object parameter)
        {
            var cred = new Views.Credentials("Storage account name:",
                                             "Storage account key:",
                                             "Connect to Storage account",
                                             Preferences.Instance.LoginName,
                                             Preferences.Instance.AuthKey);
            if (cred.ShowDialog() == true)
            {
                Vm.Login(cred.ShortField.Text, cred.LongField.Text);
                Preferences.Instance.LoginName = cred.ShortField.Text;
                Preferences.Instance.AuthKey = cred.LongField.Text;
            }
            return Nop();
        }
    }
}
