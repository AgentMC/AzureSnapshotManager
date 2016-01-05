using System;
using System.IO;
using System.Xml.Linq;

namespace AzuureSnapshotManager.Global
{
    class Preferences
    {
        private static Preferences _singletone;
        public static Preferences Instance { get { return _singletone ?? (_singletone = new Preferences()); } }

        private readonly string PrefsPath;
        private Preferences()
        {
            PrefsPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AzureSnapshotManager"), "prefs.xml");
            _checkboxes = BlobFilter.Page;
            if (File.Exists(PrefsPath))
            {
                var xd = XDocument.Load(PrefsPath);
                var root = xd.Root;
                XElement element;
                if ((element = root.Element(nameof(LoginName))) != null) _loginName = element.Value;
                if ((element = root.Element(nameof(AuthKey))) != null) _authKey = element.Value;
                if ((element = root.Element(nameof(LastUsedContainer))) != null) _lastUsedContainer = element.Value;
                if ((element = root.Element(nameof(Checkboxes))) != null) _checkboxes = (BlobFilter)int.Parse(element.Value);
            }
        }

        private void Save()
        {
            var folder = Path.GetDirectoryName(PrefsPath);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            var xd = new XDocument(
                new XElement("Settings",
                    new XElement(nameof(LoginName), LoginName),
                    new XElement(nameof(AuthKey), AuthKey),
                    new XElement(nameof(LastUsedContainer), LastUsedContainer),
                    new XElement(nameof(Checkboxes), (int)Checkboxes)));
            xd.Save(PrefsPath);
        }

        private string _loginName, _authKey, _lastUsedContainer;
        private BlobFilter _checkboxes;

        public string LoginName
        {
            get
            {
                return _loginName;
            }
            set
            {
                if(_loginName != value)
                {
                    _loginName = value;
                    Save();
                }
            }
        }
        public string AuthKey
        {
            get
            {
                return _authKey;
            }
            set
            {
                if (_authKey != value)
                {
                    _authKey = value;
                    Save();
                }
            }
        }
        public string LastUsedContainer
        {
            get
            {
                return _lastUsedContainer;
            }
            set
            {
                if (_lastUsedContainer != value)
                {
                    _lastUsedContainer = value;
                    Save();
                }
            }
        }
        public BlobFilter Checkboxes
        {
            get
            {
                return _checkboxes;
            }
            set
            {
                if (_checkboxes != value)
                {
                    _checkboxes = value;
                    Save();
                }
            }
        }

        [Flags]
        public enum BlobFilter
        {
            Page = 1,
            Append = 2,
            Block = 4
        }
    }
}
