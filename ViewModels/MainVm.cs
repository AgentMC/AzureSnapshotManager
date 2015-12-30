using AzuureSnapshotManager.Commands;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace AzuureSnapshotManager
{
    public class MainVm
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
        }

        public void Login (string storageName, string key)
        {
            var x = new StorageCredentials(storageName, key);
            var y = new Microsoft.WindowsAzure.Storage.CloudStorageAccount(x, false);
            Containers.Clear();
            Blobs.Clear();
            y.CreateCloudBlobClient().ListContainers().ToList().ForEach(Containers.Add);
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
                ReloadContainer();
            }
        }

        public void ReloadContainer()
        {
            Blobs.Clear();
            _currentContainer.ListBlobs(useFlatBlobListing: true, blobListingDetails: BlobListingDetails.All)
                                .Where(t => t is ICloudBlob)
                                .Cast<ICloudBlob>()
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
                if(_currentBlob != value)
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
        public void OnCommandSucceeded()
        {
            if (CommandSucceeded != null) CommandSucceeded(this, null);
        }
    }
}
