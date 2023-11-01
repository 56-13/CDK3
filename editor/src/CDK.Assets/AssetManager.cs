using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

using CDK.Drawing;

using CDK.Assets.Containers;
using CDK.Assets.Support;
using CDK.Assets.Configs;

namespace CDK.Assets
{
    #region commands
    public interface IAssetCommand
    {
        Asset Asset { get; }
        void Redo();
        void Undo();
        bool Merge(IAssetCommand command);
    }

    public class AssetPropertyCommand : IAssetCommand
    {
        private AssetElement _element;
        private object _src;
        private object _dest;
        private string _name;

        public Asset Asset => _element.Owner;

        public AssetPropertyCommand(AssetElement element, object src, object dest, string name)
        {
            _element = element;
            _src = src;
            _dest = dest;
            _name = name;
        }

        public void Undo()
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            _element.GetType().GetProperty(_name, flags).SetValue(_element, _src);
        }

        public void Redo()
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            _element.GetType().GetProperty(_name, flags).SetValue(_element, _dest);
        }

        public bool Merge(IAssetCommand other)
        {
            if (other is AssetPropertyCommand command && command._element == _element && command._name == _name)
            {
                _dest = command._dest;
                return true;
            }
            return false;
        }
    }

    #endregion

    #region events
    public class AssetInvokeEventArgs : EventArgs
    {
        public Action Action { private set; get; }

        public AssetInvokeEventArgs(Action action)
        {
            Action = action;
        }
    }

    public class AssetOpenEventArgs : EventArgs
    {
        public Asset Asset { private set; get; }

        public AssetOpenEventArgs(Asset asset)
        {
            Asset = asset;
        }
    }

    public class AssetMessageEventArgs : EventArgs
    {
        public Asset Asset { private set; get; }
        public string Message { private set; get; }

        public AssetMessageEventArgs(Asset asset, string message)
        {
            Asset = asset;

            Message = message;
        }
    }

    public class AssetBeginProgressEventArgs : EventArgs
    {
        public int Maximum { private set; get; }

        public AssetBeginProgressEventArgs(int maximum)
        {
            Maximum = maximum;
        }
    }
    public class AssetProgressEventArgs : EventArgs
    {
        public string Message { private set; get; }
        public int Progress { private set; get; }

        public AssetProgressEventArgs(string message, int progress)
        {
            Message = message;
            Progress = progress;
        }
    }
    public class AssetBeginLoadingEventArgs : EventArgs
    {
        public Asset Asset { private set; get; }
        public AssetBeginLoadingEventArgs(Asset asset)
        {
            Asset = asset;
        }
    }
    #endregion

    internal class AssetCommandHolder : IDisposable
    {
        public AssetCommandHolder() => AssetManager.Instance.HoldCommand();
        public void Dispose() => AssetManager.Instance.ReleaseCommand();
    }

    internal class AssetRetrieveHolder : IDisposable
    {
        public AssetRetrieveHolder() => AssetManager.Instance.HoldRetrieving();
        public void Dispose() => AssetManager.Instance.ReleaseRetrieving();
    }

    public class AssetManager : NotifyPropertyChanged
    {
        public static readonly string EditorVersion = GetAssemblyFileVersion();

        public Config Config { private set; get; }

        private string _Locale;
        public string Locale
        {
            set => SetProperty(ref _Locale, value);
            get => _Locale;
        }

        public AssetElementList<ProjectAsset> Projects { private set; get; }
        internal List<Asset> Dirties { private set; get; }

        private List<List<IAssetCommand>> _commands;
        private List<IAssetCommand> _currentCommands;
        private int _commandIndex;
        private int _commandReservation;
        private int _commandHolding;
        private int _retrieveHolding;
        private List<Action> _redirectActions;
        private Dictionary<object, object> _redirectObjects;
        private Dictionary<string, Asset> linkedAssets;

        public bool CommandEnabled { get => _commandHolding == 0; }
        public bool RetrieveEnabled { get => _retrieveHolding == 0; }

        private bool _UndoCommandEnabled;
        public bool UndoCommandEnabled
        {
            private set
            {
                if (_UndoCommandEnabled != value)
                {
                    _UndoCommandEnabled = value;
                    OnPropertyChanged("UndoCommandEnabled");
                }
            }
            get => _UndoCommandEnabled;
        }

        private bool _RedoCommandEnabled;
        public bool RedoCommandEnabled
        {
            private set
            {
                if (_RedoCommandEnabled != value)
                {
                    _RedoCommandEnabled = value;
                    OnPropertyChanged("RedoCommandEnabled");
                }
            }
            get => _RedoCommandEnabled;
        }

        private bool _IsDeveloper;
        public bool IsDeveloper
        {
            set
            {
                if (_IsDeveloper != value)
                {
                    _IsDeveloper = value;
                    OnPropertyChanged("IsDeveloper");
                }
            }
            get
            {
                return _IsDeveloper;
            }
        }

        public object ClipObject { private set; get; }
        public bool ClipCut { private set; get; }

        public event EventHandler<AssetInvokeEventArgs> Invoking;
        public event EventHandler<AssetOpenEventArgs> Opening;
        public event EventHandler<AssetMessageEventArgs> Messaging;
        public event EventHandler<EventArgs> Purging;
        public event EventHandler<AssetBeginProgressEventArgs> StartProgressing;
        public event EventHandler<AssetProgressEventArgs> Progressing;
        public event EventHandler<EventArgs> EndProgressing;
        public event EventHandler<AssetBeginLoadingEventArgs> BeginLoading;
        public event EventHandler<EventArgs> EndLoading;

        private const int CommandCapacity = 64;

        private const string ConfigPath = "config.xml";

        private AssetManager()
        {
            Instance = this;

            ErrorHandler.Start();

            GraphicsContext.CreateShared();
            AudioPlayer.CreateShared();
            ResourceManager.CreateShared();

            Projects = new AssetElementList<ProjectAsset>(null);
            Projects.BeforeListChanged += Projects_BeforeListChanged;

            Dirties = new List<Asset>();

            _commands = new List<List<IAssetCommand>>();
            _currentCommands = new List<IAssetCommand>();

            linkedAssets = new Dictionary<string, Asset>();

#if DEBUG
            _IsDeveloper = true;
#endif
            Config = new Config(ConfigPath);

            _Locale = Config.Locales[0];
        }
            
        private void Projects_BeforeListChanged(object sender, BeforeListChangedEventArgs<ProjectAsset> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    e.Object.Link();
                    break;
                case ListChangedType.ItemDeleted:
                    Projects[e.NewIndex].Unlink(false);
                    break;
                case ListChangedType.ItemChanged:
                    Projects[e.NewIndex].Unlink(false);
                    e.Object.Link();
                    break;
                case ListChangedType.Reset:
                    foreach (ProjectAsset project in Projects)
                    {
                        project.Unlink(false);
                    }
                    break;
            }
        }

        private void Dispose()
        {
            foreach (var project in Projects) project.Unlink(false);

            Config.Dispose();

            ResourceManager.DisposeShared();
            AudioPlayer.DisposeShared();
            GraphicsContext.DisposeShared();

            ErrorHandler.Stop();
        }

        public void Purge()
        {
            var process = Process.GetCurrentProcess();
            process.Refresh();

            var prevMemoryUsage = process.PrivateMemorySize64;

            Console.WriteLine($"purge check:{prevMemoryUsage / 1024768}mb");

            var limit = (long)Config.Memory * 1024768;

            if (prevMemoryUsage > limit)
            {
                ResourceManager.Instance.Purge(prevMemoryUsage - limit);

                Purging?.Invoke(this, EventArgs.Empty);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                process.Refresh();
                
                var nextMemoryUsage = process.PrivateMemorySize64;
                Console.WriteLine($"purge complete:{nextMemoryUsage / 1024768}mb");
            }
        }

        public void Invoke(Action action)
        {
            Invoking(this, new AssetInvokeEventArgs(action));
        }

        public void Open(Asset asset)
        {
            Opening?.Invoke(this, new AssetOpenEventArgs(asset));
        }

        public void BeginCommands()
        {
            _commandReservation++;
        }

        public void EndCommands()
        {
            if (--_commandReservation <= 0)
            {
                _commandReservation = 0;
                RecordCommands();
            }
        }

        internal void HoldCommand()
        {
            _commandHolding++;
        }

        internal void ReleaseCommand()
        {
            if (_commandHolding > 0) _commandHolding--;
        }

        private void RecordCommands()
        {
            if (_currentCommands.Count != 0)
            {
                bool redoStateChanged = _commandIndex < _commands.Count;

                if (redoStateChanged)
                {
                    _commands.RemoveRange(_commandIndex, _commands.Count - _commandIndex);
                }

                _commands.Add(new List<IAssetCommand>(_currentCommands));

                if (_commandIndex > CommandCapacity)
                {
                    _commands.RemoveAt(0);
                }
                else
                {
                    _commandIndex++;
                }

                RedoCommandEnabled = false;
                UndoCommandEnabled = true;

                _currentCommands.Clear();
            }
        }

        internal void Command(IAssetCommand command)
        {
            if (CommandEnabled)
            {
                var merge = false;
                foreach (var other in _currentCommands)
                {
                    if (other.Merge(command))
                    {
                        merge = true;
                        break;
                    }
                }
                if (!merge) _currentCommands.Add(command);

                if (_commandReservation == 0)
                {
                    Invoke(RecordCommands);
                }
            }
        }

        internal void ClearCommands()
        {
            _commandIndex = 0;
            _commands.Clear();
            _currentCommands.Clear();
        }

        internal void ClearCommands(Asset asset)
        {
            int i;

            foreach (var commands in this._commands)
            {
                i = 0;

                while (i < commands.Count)
                {
                    var command = commands[i];

                    if (command.Asset == asset)
                    {
                        commands.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            i = 0;

            while (i < _currentCommands.Count)
            {
                var command = _currentCommands[i];

                if (command.Asset == asset)
                {
                    _currentCommands.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public void UndoCommand()
        {
            if (_commandIndex > 0)
            {
                _commandIndex--;

                HoldCommand();
                HoldRetrieving();

                try
                {
                    for (int i = _commands[_commandIndex].Count - 1; i >= 0; i--)
                    {
                        var command = _commands[_commandIndex][i];

                        command.Undo();

                        var asset = command.Asset;
                        if (asset != null) asset.IsDirty = true;
                    }
                }
                finally
                {
                    ReleaseCommand();
                    ReleaseRetrieving();

                    if (_commandIndex == 0) UndoCommandEnabled = false;
                    RedoCommandEnabled = true;
                }
            }
        }

        public void RedoCommand()
        {
            if (_commandIndex < _commands.Count)
            {
                HoldCommand();
                HoldRetrieving();

                try
                {
                    foreach (var command in _commands[_commandIndex])
                    {
                        command.Redo();

                        var asset = command.Asset;
                        if (asset != null) asset.IsDirty = true;
                    }
                }
                finally
                {
                    ReleaseCommand();
                    ReleaseRetrieving();

                    _commandIndex++;

                    if (_commandIndex == _commands.Count) RedoCommandEnabled = false;
                    UndoCommandEnabled = true;
                }
            }
        }

        internal void HoldRetrieving()
        {
            _retrieveHolding++;
        }

        internal void ReleaseRetrieving()
        {
            if (_retrieveHolding > 0)
            {
                _retrieveHolding--;
            }
        }

        public void Clip(object obj, bool cut)
        {
            ClipObject = obj;
            ClipCut = cut;
        }

        public void ClearClip()
        {
            ClipObject = null;
            ClipCut = false;
        }

        internal bool LinkAsset(Asset asset)
        {
            if (linkedAssets.ContainsKey(asset.Key)) return false;

            linkedAssets.Add(asset.Key, asset);

            return true;
        }

        internal bool UnlinkAsset(Asset asset)
        {
            if (linkedAssets.ContainsKey(asset.Key))
            {
                if (linkedAssets[asset.Key] == asset)
                {
                    linkedAssets.Remove(asset.Key);

                    return true;
                }
            }
            return false;
        }

        public Asset GetAsset(string key)
        {
            return linkedAssets.ContainsKey(key) ? linkedAssets[key] : null;
        }

        public void Message(string msg)
        {
            Message(null, msg);
        }

        public void Message(Asset asset, string msg)
        {
            Messaging?.Invoke(this, new AssetMessageEventArgs(asset, msg));
        }

        private bool cancelProgress;

        public void BeginProgress(int maximum)
        {
            cancelProgress = false;

            StartProgressing?.Invoke(this, new AssetBeginProgressEventArgs(maximum));
        }

        public bool Progress(string message, int progress)
        {
            if (cancelProgress) return false;
            Progressing?.Invoke(this, new AssetProgressEventArgs(message, progress));
            return true;
        }

        public void EndProgress()
        {
            EndProgressing?.Invoke(this, EventArgs.Empty);
        }

        public void CancelProgress()
        {
            cancelProgress = true;
        }

        internal void BeginLoad(Asset asset)
        {
            BeginLoading?.Invoke(this, new AssetBeginLoadingEventArgs(asset));
        }

        internal void EndLoad()
        {
            EndLoading?.Invoke(this, EventArgs.Empty);
        }

        internal void BeginRedirection()
        {
            _redirectActions = new List<Action>();
            _redirectObjects = new Dictionary<object, object>();
        }

        internal void EndRedirection()
        {
            foreach (var action in _redirectActions) action();
            _redirectActions = null;
            _redirectObjects = null;
        }

        internal void AddRedirection(object src, object dest)
        {
            if (_redirectObjects != null && !_redirectObjects.ContainsKey(src))
            {
                _redirectObjects.Add(src, dest);
            }
        }

        internal void InvokeRedirection(Action action)
        {
            if (_redirectActions != null) _redirectActions.Add(action);
            else action();
        }

        internal T GetRedirection<T>(T src)
        {
            return _redirectObjects != null && src != null && _redirectObjects.TryGetValue(src, out var dest) ? (T)dest : src;
        }

        internal T[] GetRedirection<T>(T[] src)
        {
            if (_redirectObjects != null && src != null)
            {
                var dest = new T[src.Length];
                for (var i = 0; i < src.Length; i++)
                {
                    if (_redirectObjects.TryGetValue(src[i], out object e)) dest[i] = (T)e;
                    else dest[i] = src[i];
                }
                return dest;
            }
            return src;
        }

        private static string GetAssemblyFileVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            return version;
        }

        private static Thread _mainThread = Thread.CurrentThread;
        public static bool IsMainThread => Thread.CurrentThread == _mainThread;
        public static string NewKey()
        {
            var guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            return guid.Substring(0, guid.Length - 2);
        }

        public static AssetManager Instance { private set; get; }
        public static bool IsCreated => Instance != null;
        public static void CreateShared()
        {
            if (Instance == null) new AssetManager();
        }

        public static void DisposeShared()
        {
            if (Instance != null)
            {
                Instance.Dispose();
                Instance = null;
            }
        }

    }
}
