using AzuureSnapshotManager.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace AzuureSnapshotManager.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Singleton;

        private MainVm Context { get { return (MainVm)DataContext; } }

        public MainWindow()
        {
            InitializeComponent();
            Singleton = this;
            Context.ErrorOccurred += (o, e) => 
                MessageBox.Show(this, e.ExceptionObject.ToString(), "Error occurred", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            Context.CommandSucceeded += Context_CommandSucceeded;
        }

        private async void Context_CommandSucceeded(object sender, System.EventArgs e)
        {
            SuccessIndicator.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            SuccessIndicator.Visibility = Visibility.Hidden;
        }

        private void Blobs_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Context.CurrentBlob = (BlobVm)e.NewValue;
            e.Handled = true;
        }
    }
}
