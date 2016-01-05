using AzuureSnapshotManager.Commands;
using AzuureSnapshotManager.Global;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AzuureSnapshotManager.ViewModels
{
    public class MainVm: INotifyPropertyChanged
    {
        private readonly ICommand _loginCommand, _newSnapshotCommand, _revertSnapshotCommand, _removeSnapshotCommand, _setMetadataCommand;
        public MainVm()
        {
            Containers = new ObservableCollection<CloudBlobContainer>();
            Blobs = new ObservableCollection<BlobVm>();
            _loginCommand = new LoginCommand(this);
            _newSnapshotCommand = new NewSnapshotCommand(this);
            _removeSnapshotCommand = new RemoveSnapshotCommand(this);
            _revertSnapshotCommand = new RevertSnapshotCommand(this);
            _setMetadataCommand = new SetMetadataCommand(this);
            //----
            _showAppendBlobs = (Preferences.Instance.Checkboxes & Preferences.BlobFilter.Append) > 0;
            _showBlockBlobs = (Preferences.Instance.Checkboxes & Preferences.BlobFilter.Block) > 0;
            _showPageBlobs = (Preferences.Instance.Checkboxes & Preferences.BlobFilter.Page) > 0;
        }

        public void Login(string storageName, string key)
        {
            var credentials = new StorageCredentials(storageName, key);
            var account = new Microsoft.WindowsAzure.Storage.CloudStorageAccount(credentials, false);
            Containers.Clear();
            Blobs.Clear();
            account.CreateCloudBlobClient().ListContainers().ToList().ForEach(Containers.Add);
            var lastUsed = Preferences.Instance.LastUsedContainer;
            CloudBlobContainer container;
            if(lastUsed != null && (container = Containers.FirstOrDefault(c=>c.Name == lastUsed)) != null)
            {
                CurrentContainer = container;
            }
        }

        public ObservableCollection<CloudBlobContainer> Containers { get; set; }

        private CloudBlobContainer _currentContainer;
        public CloudBlobContainer CurrentContainer
        {
            get
            {
                return _currentContainer;
            }
            set
            {
                _currentContainer = value;
                if (_currentContainer != null) Preferences.Instance.LastUsedContainer = _currentContainer.Name;
                OnPropertyChanged();
                ReloadContainer();
            }
        }

        public void ReloadContainer()
        {
            if (_currentContainer == null) return;
            Blobs.Clear();
            _currentContainer.ListBlobs(useFlatBlobListing: true, blobListingDetails: BlobListingDetails.All)
                                .Where(t => t is ICloudBlob)
                                .Cast<ICloudBlob>()
                                .Where(b => b.BlobType == BlobType.AppendBlob && ShowAppendBlobs ||
                                            b.BlobType == BlobType.BlockBlob && ShowBlockBlobs ||
                                            b.BlobType == BlobType.PageBlob && ShowPageBlobs)
                                .GroupBy(b => b.Name)
                                .OrderBy(g => g.Key)
                                .Select(items => new BlobVm(items))
                                .ToList()
                                .ForEach(Blobs.Add);
        }

        public ObservableCollection<BlobVm> Blobs { get; set; }

        private BlobVm _currentBlob;

        public BlobVm CurrentBlob
        {
            get
            {
                return _currentBlob;
            }
            set
            {
                if (_currentBlob != value)
                {
                    _currentBlob = value;
                    if (CurrentBlobChanged != null) CurrentBlobChanged(this, null);
                }
            }
        }

        public event EventHandler CurrentBlobChanged;

        public void OnError(Exception e)
        {
            if (ErrorOccurred != null) ErrorOccurred(this, new UnhandledExceptionEventArgs(e, false));
        }

        public event UnhandledExceptionEventHandler ErrorOccurred;

        public ICommand LoginCommand
        {
            get { return _loginCommand; }
        }
        public ICommand NewSnapshotCommand
        {
            get { return _newSnapshotCommand; }
        }
        public ICommand RemoveSnapshotCommand
        {
            get { return _removeSnapshotCommand; }
        }
        public ICommand RevertSnapshotCommand
        {
            get { return _revertSnapshotCommand; }
        }
        public ICommand SetMetadataCommand
        {
            get { return _setMetadataCommand; }
        }

        public event EventHandler CommandSucceeded;
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnCommandSucceeded()
        {
            if (CommandSucceeded != null) CommandSucceeded(this, null);
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _showPageBlobs = true, _showAppendBlobs = false, _showBlockBlobs = false;
        public bool ShowPageBlobs
        {
            get
            {
                return _showPageBlobs;
            }
            set
            {
                if (value != _showPageBlobs)
                {
                    _showPageBlobs = value;
                    Preferences.Instance.Checkboxes ^= Preferences.BlobFilter.Page;
                    ReloadContainer();
                }
            }
        }
        public bool ShowAppendBlobs
        {
            get
            {
                return _showAppendBlobs;
            }
            set
            {
                if (value != _showAppendBlobs)
                {
                    _showAppendBlobs = value;
                    Preferences.Instance.Checkboxes ^= Preferences.BlobFilter.Append;
                    ReloadContainer();
                }
            }
        }
        public bool ShowBlockBlobs
        {
            get
            {
                return _showBlockBlobs;
            }
            set
            {
                if (value != _showBlockBlobs)
                {
                    _showBlockBlobs = value;
                    Preferences.Instance.Checkboxes ^= Preferences.BlobFilter.Block;
                    ReloadContainer();
                }
            }
        }
    }
}
