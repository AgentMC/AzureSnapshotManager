using AzuureSnapshotManager.Global;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AzuureSnapshotManager.ViewModels
{
    public class BlobVm
    {
        public BlobVm(IEnumerable<ICloudBlob> items)
        {
            //Step 1: sorting and tree-ing
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

            //Step 2: finding the actual snapshot
            ActiveMark = "[>]";
            var activeSnapshotHash = GetMetadata(Constants.KeySnapshotCurr);
            //round 1: try to identify active snapshot based on active snapshot mark
            if(activeSnapshotHash != null)
            {
                foreach (var child in Children)
                {
                    if (child.Blob.GetTimeStampHash() == activeSnapshotHash)
                    {
                        child.ActiveMark = ActiveMark;
                        ActiveMark = null;
                        break;
                    }
                }
            }
            //round 2 - in case round 1 failed. Fallback to naїve way - based on last rollback done.
            if (Blob.CopyState != null && ActiveMark != null)
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

        //--------

        public string GetMetadata(string key)
        {
            string data;
            if (Blob == null || !Blob.Metadata.TryGetValue(key, out data)) return null;
            return Uri.UnescapeDataString(data);
        }
        public void SetMetadata(IEnumerable<KeyValuePair<string, string>> metas)
        {
            if (Blob == null) throw new InvalidOperationException("The underlying blob is not initialized");
            foreach (var pair in metas)
            {
                if (!string.IsNullOrWhiteSpace(pair.Value))
                {
                    Blob.Metadata[pair.Key] = Uri.EscapeDataString(pair.Value);
                }
                else if (Blob.Metadata.ContainsKey(pair.Key))
                {
                    Blob.Metadata.Remove(pair.Key);
                }
            }
            Blob.SetMetadata();
        }

        //--------

        public void SetActiveSnapshot(string snapshotTimeStampHash)
        {
            SetMetadata(new[] { new KeyValuePair<string, string>(Constants.KeySnapshotCurr, snapshotTimeStampHash) });
        }

        public void SetSnapshotMetadata(string snapshotTimeStampHash, string snapshotName, string snapshotDescription)
        {
            SetMetadata(new Dictionary<string, string>
            {
                { Constants.KeySnapshotName + snapshotTimeStampHash, snapshotName},
                { Constants.KeySnapshotDesc + snapshotTimeStampHash, snapshotDescription}
            });
        }
        private string GetSnapshotMetadata(string snapshotProprtyName)
        {
            if (Blob == null || Parent == null) return null;
            return Parent.GetMetadata(snapshotProprtyName + Blob.GetTimeStampHash());
        }
        public string SnapshotTitle
        {
            get { return GetSnapshotMetadata(Constants.KeySnapshotName); }
        }
        public string SnapshotDate
        {
            get { return Blob?.SnapshotTime?.ToString(); }
        }
        public string SnapshotDescription
        {
            get { return GetSnapshotMetadata(Constants.KeySnapshotDesc); }
        }
        public bool HasDescription
        {
            get { return !string.IsNullOrWhiteSpace(SnapshotDescription); }
        }
    }
}
