using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using CDK.Assets.Support;

namespace CDK.Assets
{
    public class LocaleString : AssetElement
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        private string _Key;
        public string Key => $"{Owner.Key}/{_Key}";

        private DateTime _Timestamp;
        internal DateTime Timestamp
        {
            set
            {
                if (_Timestamp != value)
                {
                    _Timestamp = value;
                    OnPropertyChanged("Timestamp");
                }
            }
            get => _Timestamp;
        }

        private string _baseValue;
        private Dictionary<string, string> _localeValues;

        public string Value
        {
            set => SetLocaleValue(AssetManager.Instance.Locale, value);
            get => GetLocaleValue(AssetManager.Instance.Locale);
        }

        public string GetLocaleValue(string locale)
        {
            if (locale == null || !_Locale) return _baseValue;
            
            return _localeValues.TryGetValue(locale, out var value) ? value : string.Empty;
        }

        public void SetLocaleValue(string locale, string value)
        {
            if (value == null) value = string.Empty;

            var oldValue = GetLocaleValue(locale);

            if (value != oldValue)
            {
                if (AssetManager.Instance.CommandEnabled)
                {
                    AssetManager.Instance.Command(new LocaleValueCommand(this, locale, oldValue, value));
                }
                if (locale == null || !_Locale) _baseValue = value;
                else if (value == string.Empty) _localeValues.Remove(locale);

                else if (_localeValues.ContainsKey(locale)) _localeValues[locale] = value;
                else _localeValues.Add(locale, value);
                
                IsDirty = true;

                OnPropertyChanged("Value");

                Timestamp = DateTime.Now;
            }
        }

        private class LocaleValueCommand : IAssetCommand
        {
            private LocaleString _str;
            private string _locale;
            private string _prevValue;
            private string _nextValue;
            public Asset Asset => _str.Owner;

            public LocaleValueCommand(LocaleString str, string locale, string prevValue, string nextValue)
            {
                _str = str;

                _locale = locale;
                _prevValue = prevValue;
                _nextValue = nextValue;
            }

            private void Restore(string value)
            {
                if (_locale == null) _str._baseValue = value;
                else if (string.IsNullOrEmpty(value)) _str._localeValues.Remove(_locale);
                else if (_str._localeValues.ContainsKey(_locale)) _str._localeValues[_locale] = value;
                else _str._localeValues.Add(_locale, value);

                _str.IsDirty = true;

                _str.OnPropertyChanged("Value");
            }

            public void Undo() => Restore(_prevValue);
            public void Redo() => Restore(_nextValue);
            public bool Merge(IAssetCommand other) => false;
        }

        private bool _Locale;
        public bool Locale
        {
            set
            {
                if (SetProperty(ref _Locale, value)) OnPropertyChanged("Value");
            }
            get => _Locale;
        }

        public LocaleString(AssetElement parent)
        {
            Parent = parent;

            _Key = AssetManager.NewKey();

            _Timestamp = DateTime.Now;

            _baseValue = string.Empty;

            _localeValues = new Dictionary<string, string>();

            AssetManager.Instance.AddWeakPropertyChanged(AssetManager_PropertyChanged);
        }

        public LocaleString(AssetElement parent, LocaleString other, bool content, bool isNew)
        {
            Parent = parent;

            _Key = isNew ? AssetManager.NewKey() : other._Key;

            _Timestamp = DateTime.Now;

            _localeValues = new Dictionary<string, string>();
            if (content)
            {
                _baseValue = other._baseValue;
                foreach (string locale in other._localeValues.Keys)
                {
                    _localeValues.Add(locale, other._localeValues[locale]);
                }
            }
            else _baseValue = string.Empty;

            _Locale = other._Locale;

            AssetManager.Instance.AddWeakPropertyChanged(AssetManager_PropertyChanged);
        }

        public LocaleString(AssetElement parent, bool locale)
        {
            Parent = parent;

            _Key = AssetManager.NewKey();

            _Timestamp = DateTime.Now;

            _localeValues = new Dictionary<string, string>();
            _baseValue = string.Empty;
            _Locale = locale;

            AssetManager.Instance.AddWeakPropertyChanged(AssetManager_PropertyChanged);
        }

        private void AssetManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_Locale && e.PropertyName.Equals("Locale")) OnPropertyChanged("Value");
        }

        public void Copy(LocaleString other, bool isNew, bool dirty)
        {
            if (AssetManager.Instance.CommandEnabled)
            {
                AssetManager.Instance.Command(new LocaleValueCommand(this, null, _baseValue, other._baseValue));

                foreach (var locale in other._localeValues.Keys)
                {
                    var localeValue = other._localeValues[locale];
                    
                    if (!_localeValues.TryGetValue(locale, out var oldLocaleValue)) oldLocaleValue = string.Empty;

                    if (!oldLocaleValue.Equals(localeValue))
                    {
                        AssetManager.Instance.Command(new LocaleValueCommand(this, locale, oldLocaleValue, localeValue));
                    }
                }
            }
            _baseValue = other._baseValue;
            Locale = other._Locale;
            _localeValues.Clear();
            foreach (var locale in other._localeValues.Keys)
            {
                var localeValue = other._localeValues[locale];
                _localeValues.Remove(locale);
                _localeValues.Add(locale, localeValue);
            }
            if (!isNew) _Key = other._Key;

            if (dirty) IsDirty = true;

            OnPropertyChanged("Value");

            Timestamp = DateTime.Now;
        }

        private bool IsValidLocale(string locale)
        {
            foreach (var validLocale in AssetManager.Instance.Config.Locales)
            {
                if (validLocale.Equals(locale)) return true;
            }
            return false;
        }

        internal string Save()
        {
            var str = new string[4 + 2 * _localeValues.Count];
            var i = 0;
            str[i++] = _Key;
            str[i++] = _Timestamp.ToString();
            str[i++] = _baseValue;
            str[i++] = _Locale.ToString();
            foreach (var locale in _localeValues.Keys)
            {
                str[i++] = locale;
                str[i++] = _localeValues[locale];
            }
            return CSV.Make(str);
        }

        internal void Load(string str)
        {
            var csv = CSV.Parse(str);

            _Key = csv[0];
            _Timestamp = DateTime.Parse(csv[1]);

            if (AssetManager.Instance.CommandEnabled)
            {
                AssetManager.Instance.Command(new LocaleValueCommand(this, null, _baseValue, csv[2]));

                for (var i = 4; i < csv.Length - 1; i += 2)
                {
                    var locale = csv[i];

                    if (IsValidLocale(locale))
                    {
                        var localeValue = csv[i + 1];
                        
                        if (!_localeValues.TryGetValue(locale, out var oldLocaleValue)) oldLocaleValue = string.Empty;

                        if (!oldLocaleValue.Equals(localeValue))
                        {
                            AssetManager.Instance.Command(new LocaleValueCommand(this, locale, oldLocaleValue, localeValue));
                        }
                    }
               }
            }
            _baseValue = csv[2];
            Locale = bool.Parse(csv[3]);
            _localeValues.Clear();
            for (var i = 4; i < csv.Length - 1; i += 2)
            {
                var locale = csv[i];

                if (IsValidLocale(locale))
                {
                    var localeValue = csv[i + 1];
                    _localeValues.Remove(locale);
                    if (localeValue != string.Empty) _localeValues.Add(locale, localeValue);
                }
            }
            IsDirty = true;

            OnPropertyChanged("Value");
            OnPropertyChanged("Timestamp");
        }

        public class ImportCollision
        {
            public string Key { private set; get; }
            public string Origin { private set; get; }
            public string Value { private set; get; }

            public ImportCollision(string key, string origin, string value)
            {
                Key = key;
                Origin = origin;
                Value = value;
            }
        }

        public ImportCollision Import(string[] values, string[] locales, bool checkCollision)
        {
            var count = Math.Min(locales.Length, values.Length);

            if (checkCollision)
            {
                var originLocale = AssetManager.Instance.Locale;
                if (originLocale != null)
                {
                    for (var i = 0; i < count; i++)
                    {
                        var locale = locales[i];
                        var localeValue = values[i];

                        if (locale.Equals(originLocale))
                        {
                            var originLocaleValue = GetLocaleValue(originLocale);

                            if (originLocaleValue != localeValue)
                            {
                                return new ImportCollision(Key, originLocaleValue, localeValue);
                            }
                        }
                    }
                }
            }

            if (AssetManager.Instance.CommandEnabled)
            {
                for (var i = 0; i < count; i++)
                {
                    var locale = locales[i];

                    if (IsValidLocale(locale))
                    {
                        var localeValue = values[i];
                        
                        if (!_localeValues.TryGetValue(locale, out var oldLocaleValue)) oldLocaleValue = string.Empty;

                        if (!oldLocaleValue.Equals(localeValue))
                        {
                            AssetManager.Instance.Command(new LocaleValueCommand(this, locale, oldLocaleValue, localeValue));
                        }
                    }
                }
            }
            var dirty = false;

            for (var i = 0; i < count; i++)
            {
                var locale = locales[i];

                if (IsValidLocale(locale))
                {
                    var localeValue = values[i];

                    if (_localeValues.TryGetValue(locale, out var oldLocaleValue))
                    {
                        if (!oldLocaleValue.Equals(localeValue))
                        {
                            _localeValues[locale] = localeValue;

                            dirty = true;
                        }
                    }
                    else if (localeValue != string.Empty)
                    {
                        _localeValues.Add(locale, localeValue);

                        dirty = true;
                    }
                }
            }
            if (dirty)
            {
                IsDirty = true;

                OnPropertyChanged("Value");

                Timestamp = DateTime.Now;
            }

            return null;
        }

        internal void Clean()
        {
            var dirty = false;

            if (_Locale)
            {
                if (_baseValue != string.Empty)
                {
                    if (AssetManager.Instance.CommandEnabled)
                    {
                        AssetManager.Instance.Command(new LocaleValueCommand(this, null, _baseValue, string.Empty));
                    }
                    
                    _baseValue = string.Empty;

                    dirty = true;
                }
            }
            else
            {
                if (_localeValues.Count != 0)
                {
                    if (AssetManager.Instance.CommandEnabled)
                    {
                        foreach (var locale in _localeValues.Keys)
                        {
                            AssetManager.Instance.Command(new LocaleValueCommand(this, locale, _localeValues[locale], string.Empty));
                        }
                    }

                    _localeValues.Clear();

                    dirty = true;
                }
            }
            if (dirty) IsDirty = true;
        }

        internal int BuildSize(string enc)
        {
            if (_Locale)
            {
                var size = TypeUtil.GetLengthSize(_localeValues.Count);
                foreach (var locale in _localeValues.Keys)
                {
                    size += TypeUtil.GetStringSize(locale, enc);
                    size += TypeUtil.GetStringSize(_localeValues[locale], enc);
                }
                return size;
            }
            else return TypeUtil.GetStringSize(_baseValue, enc);
        }

        private static Regex FormatRegEx = new Regex("%[.0-9]*[a-zA-Z]");
        private string[] SplitFormats(string str)
        {
            var matches = FormatRegEx.Matches(str);
            var formats = new string[matches.Count];
            for (var i = 0; i < matches.Count; i++)
            {
                formats[i] = matches[i].Value;
            }
            return formats;
        }

        internal void Build(BinaryWriter writer, string enc)
        {
            if (_Locale)
            {
                string[] formats = null;

                writer.WriteLength(_localeValues.Count);
                foreach (var locale in _localeValues.Keys)
                {
                    var value = _localeValues[locale];
                    
                    if (formats == null) formats = SplitFormats(value);
                    else if (!Enumerable.SequenceEqual(formats, SplitFormats(value))) throw new AssetException(Owner, "언어별 텍스트포맷 형식이 맞지 않습니다.");
                    writer.WriteString(locale, enc);
                    writer.WriteString(value, enc);
                }
            }
            else writer.WriteString(_baseValue, enc);
        }
    }
}
