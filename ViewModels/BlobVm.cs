using AzuureSnapshotManager.Global;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AzuureSnapshotManager
{
    public class BlobVm
    {
        public BlobVm(IEnumerable<ICloudBlob> items)
        {
            Children = new ObservableCollection<BlobVm>();
            foreach(var item in items)
            {
                if (item.IsSnapshot)
                {
                    Children.Add(new BlobVm(item, this));
                }
                else if (Blob == null)
                {
                    Blob = item;
                }
                else
                {
                    throw new Exception("More than one non-snapshot blob exists in group.");
                }
            }
            ActiveMark = "[>]";
            if(Blob.CopyState != null)
            {
                foreach (var child in Children)
                {
                    if(Blob.CopyState.Source.AbsoluteUri.EndsWith(child.Blob.SnapshotQualifiedUri.Query))
                    {
                        child.ActiveMark = ActiveMark;
                        ActiveMark = null;
                        break;
                    }
                }
            }
        }
        private BlobVm (ICloudBlob blob, BlobVm parent)
        {
            Blob = blob;
            Parent = parent;
        }
        public ObservableCollection<BlobVm> Children { get; set; }
        public BlobVm Parent { get; set; }
        public string ActiveMark { get; set; }
        public ICloudBlob Blob { get; set; }

        private string GetBlobMetadata(string key)
        {
            string data;
            if (Blob == null || 
                Parent == null || 
                Parent.Blob == null || 
                !Parent.Blob.Metadata.TryGetValue(key + Blob.GetTimeStampHash(), out data))
                return null;
            return data;
        }

        public string SnapshotTitle
        {
            get { return GetBlobMetadata(Constants.KeySnapshotName); }
        }
        public string SnapshotDate
        {
            get { return Blob?.SnapshotTime?.ToString(); }
        }
        public string SnapshotDescription
        {
            get { return GetBlobMetadata(Constants.KeySnapshotDesc); }
        }
        public bool HasDescription
        {
            get { return !string.IsNullOrWhiteSpace(SnapshotDescription); }
        }
    }
}
