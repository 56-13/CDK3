using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace CDK.SignalViewer
{
    public class Signal : NotifyPropertyChanged
    {
        public string Platform { private set; get; }
        private HashSet<string> _Devices;
        public string[] Devices => _Devices.ToArray();
        public string AppVersion { private set; get; }
        public string Code { private set; get; }
        public int Occurs { private set; get; }
        public string Stack { private set; get; }
        public string StackId { private set; get; }

        public Signal(string platform, string device, string appVersion, string code, int occurs, string stack, string stackId)
        {
            Platform = platform;
            _Devices = new HashSet<string>
            {
                device
            };
            AppVersion = appVersion;
            Code = code;
            Occurs = occurs;
            Stack = stack;
            StackId = stackId;
        }

        public bool Merge(Signal signal)
        {
            if (Platform == signal.Platform && AppVersion == signal.AppVersion && Code == signal.Code && StackId == signal.StackId)
            {
                foreach (var device in signal._Devices) _Devices.Add(device);
                Occurs += signal.Occurs;
                OnPropertyChanged("Title");
                OnPropertyChanged("Text");
                return true;
            }
            return false;
        }

        public string Title => $"{AppVersion} {Platform} {Code} ({Occurs})";

        public string Text
        {
            get
            {
                var strbuf = new StringBuilder();
                strbuf.Append("platform:");
                strbuf.Append(Platform);
                strbuf.Append("\r\n");

                strbuf.Append("device:");
                var first = true;
                foreach (var device in _Devices)
                {
                    if (first) first = false;
                    else strbuf.Append(',');
                    strbuf.Append(device);
                }
                strbuf.Append("\r\n");

                strbuf.Append("app version:");
                strbuf.Append(AppVersion);
                strbuf.Append("\r\n");

                strbuf.Append("occurs:");
                strbuf.Append(Occurs);
                strbuf.Append("\r\n");

                strbuf.Append(Code);
                strbuf.Append("\r\n");

                strbuf.Append(Stack);

                return strbuf.ToString();
            }
        }

        public override string ToString() => Text;
    }
}
