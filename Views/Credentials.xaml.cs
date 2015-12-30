using System.Windows;

namespace AzuureSnapshotManager.Views
{
    /// <summary>
    /// Interaction logic for Credentials.xaml
    /// </summary>
    public partial class Credentials : Window
    {
        public Credentials(string shortFieldName, string longFieldName, string caption)
        {
            InitializeComponent();
            Owner = MainWindow.Singleton;
            ShortFieldLabel.Content = shortFieldName;
            LongFieldLabel.Content = longFieldName;
            Title = caption;
        }
        public Credentials(string shortFieldName, string longFieldName, string caption, string shortValue, string longValue) : this(shortFieldName, longFieldName, caption)
        {
            ShortField.Text = shortValue;
            LongField.Text = longValue;
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
