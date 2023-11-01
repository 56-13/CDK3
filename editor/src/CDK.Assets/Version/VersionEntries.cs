using System;
using System.ComponentModel;

namespace CDK.Assets.Version
{
    public class VersionEntries : NotifyPropertyChanged
    {
        private VersionEntry[] _entries;

        public VersionAsset Parent => _entries[0].Parent;

        public string Path
        {
            set
            {
                if (_entries.Length == 1) _entries[0].Path = value;
            }
            get => _entries.Length == 1 ? _entries[0].Path : string.Empty;
        }

        public string CheckSum => _entries.Length == 1 ? _entries[0].CheckSum : string.Empty;

        public int Version
        {
            get
            {
                var version = _entries[0].Version;
                for (var i = 1; i < _entries.Length; i++)
                {
                    if (_entries[i].Version != version) return 0;
                }
                return version;
            }
        }
        public int AppVersion0
        {
            set
            {
                foreach (var entry in _entries) entry.AppVersion0 = value;
            }
            get
            {
                var version = _entries[0].AppVersion0;
                for (var i = 1; i < _entries.Length; i++)
                {
                    if (_entries[i].AppVersion0 != version) return 0;
                }
                return version;
            }
        }
        public int AppVersion1
        {
            set
            {
                foreach (var entry in _entries) entry.AppVersion1 = value;
            }
            get
            {
                var version = _entries[0].AppVersion1;
                for (var i = 1; i < _entries.Length; i++)
                {
                    if (_entries[i].AppVersion1 != version) return 0;
                }
                return version;
            }
        }
        public int AppVersion2
        {
            set
            {
                foreach (var entry in _entries) entry.AppVersion2 = value;
            }
            get
            {
                var version = _entries[0].AppVersion2;
                for (var i = 1; i < _entries.Length; i++)
                {
                    if (_entries[i].AppVersion2 != version) return 0;
                }
                return version;
            }
        }

        public bool Local
        {
            set
            {
                foreach (var entry in _entries) entry.Local = value;
            }
            get
            {
                var local = _entries[0].Local;
                for (var i = 1; i < _entries.Length; i++)
                {
                    if (_entries[i].Local != local) return false;
                }
                return local;
            }
        }

        public VersionEntry this[int index] => _entries[index];
        public int Count => _entries.Length;

        public VersionEntries(VersionEntry[] entries)
        {
            _entries = entries;

            foreach (var entry in entries)
            {
                if (entries[0].Parent != entry.Parent) throw new InvalidOperationException();

                entry.AddWeakPropertyChanged(Entry_PropertyChanged);
            }
        }

        private void Entry_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
    }
}
