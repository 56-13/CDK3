using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace CDK.SignalViewer
{
    public class SignalLoader
    {
        private string _ndkPath;
        private string _androidExecutableNames;
        private string _androidPackageNames;
        private string _iOSPackageName;

        private class AndroidExecutableInfo
        {
            public string Name;
            public string AppVersionFrom;
            public string AppVersionTo;
            public string Path;
        }
        private AndroidExecutableInfo[] _androidExecutableInfos;

        private string _androidSignalRegEx;
        private string _androidExceptionRegEx;
        private string _iOSStackRegEx;

        private const string LoadRegEx1 = "platform:(.+)\ndevice:(.+)\napp version:(.+)\n((signal|exception) .+)\n((\\w.+\n)+)";
        private const string LoadRegEx2 = "platform:(.+)\ndevice:(.+)\napp version:(.+)\noccurs:([0-9]+)\r?\n((signal|exception) .+)\n((\\w.+\n)+)";
        private const string AndroidSignalLineNumberRegEx = ".*\n[A-Z]:/(.+)\n";
        public SignalLoader(string path)
        {
            var doc = new XmlDocument();

            doc.Load(path);

            var node = doc.ChildNodes[1];

            foreach (XmlNode subnode in node.ChildNodes)
            {
                switch(subnode.LocalName)
                {
                    case "ndkVersion":
                        {
                            var ndkVersion = subnode.Attributes["value"].Value;
                            var androidHome = Environment.GetEnvironmentVariable("ANDROID_HOME");
                            _ndkPath = Path.Combine(androidHome, "ndk", ndkVersion);
                        }
                        break;
                    case "androidExecutables":
                        {
                            var androidExecutableNames = new StringBuilder();
                            var androidExecutableInfos = new List<AndroidExecutableInfo>();
                            foreach (XmlNode infonode in subnode.ChildNodes)
                            {
                                var info = new AndroidExecutableInfo()
                                {
                                    Name = infonode.Attributes["name"].Value,
                                    AppVersionFrom = infonode.Attributes["appVersionFrom"]?.Value,
                                    AppVersionTo = infonode.Attributes["appVersionTo"]?.Value,
                                    Path = infonode.Attributes["path"].Value
                                };
                                if (!androidExecutableInfos.Any(a => a.Name == info.Name))
                                {
                                    if (androidExecutableNames.Length != 0) androidExecutableNames.Append('|');
                                    androidExecutableNames.Append(info.Name);
                                }
                                androidExecutableInfos.Add(info);
                            }
                            _androidExecutableNames = androidExecutableNames.ToString();
                            _androidExecutableInfos = androidExecutableInfos.ToArray();
                        }
                        break;
                    case "androidPackageNames":
                        _androidPackageNames = subnode.Attributes["value"].Value;
                        break;
                    case "iOSPackageName":
                        _iOSPackageName = subnode.Attributes["value"].Value;
                        break;

                }
            }
            _androidSignalRegEx = $"@*at /({_androidExecutableNames.Replace(".", "\\.")})\\.so/.+\\((.+)\\)";
            _androidExceptionRegEx = $"at (({_androidPackageNames.Replace(".", "\\.")}).*)\n";
            _iOSStackRegEx = $"[0-9]+\\s+{_iOSPackageName}\\s+(0x.+)\n";
        }

        public static string RemoveCR(string str) => str.EndsWith("\r") ? str.Substring(0, str.Length - 1) : str;

        public IEnumerable<Signal> Load(string text, bool parse)
        {
            var signals = new List<Signal>();

            foreach (Match match in Regex.Matches(text, parse ? LoadRegEx1 : LoadRegEx2))
            {
                var platform = RemoveCR(match.Groups[1].Value);
                var device = RemoveCR(match.Groups[2].Value);
                var appVersion = RemoveCR(match.Groups[3].Value);
                var occurs = parse ? 1 : int.Parse(match.Groups[4].Value);
                var code = RemoveCR(match.Groups[parse ? 4 : 5].Value);
                var stack = RemoveCR(match.Groups[parse ? 6 : 7].Value);
                var stackId = stack;

                if (platform == "android")
                {
                    if (code.StartsWith("signal")) ConvertAndroidSignalStack(parse, appVersion, ref stack, out stackId);
                    else if (code.StartsWith("exception")) ConvertAndroidExceptionStack(parse, ref stack, out stackId);
                }
                else if (platform == "ios")
                {
                    ConvertIOSStack(parse, ref stack, out stackId);
                }

                var signal = new Signal(platform, device, appVersion, code, occurs, stack, stackId);
                var flag = true;
                foreach (var other in signals)
                {
                    if (other.Merge(signal))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag) signals.Add(signal);
            }

            return signals;
        }

        private void ConvertAndroidSignalStack(bool parse, string appVersion, ref string stack, out string stackId)
        {
            var stackIdBuf = new StringBuilder();

            stack = Regex.Replace(stack, _androidSignalRegEx, ((Match match) =>
            {
                var executableName = match.Groups[1].Value;
                var pc = match.Groups[2].Value;
                stackIdBuf.Append(pc);

                if (parse && GetAndroidSignalStackLineNumber(executableName, appVersion, pc, out var lineNumber))
                {
                    return "@" + match.Value.Replace(pc, lineNumber);
                }
                return match.Value;
            }));

            stackId = stackIdBuf.ToString();
        }

        private bool GetAndroidSignalStackLineNumber(string executableName, string appVersion, string pc, out string lineNumber)
        {
            lineNumber = null;

            var executableDebugInfo = _androidExecutableInfos.FirstOrDefault(a =>
                a.Name == executableName &&
                (a.AppVersionFrom == null || appVersion.CompareTo(a.AppVersionFrom) >= 0) &&
                (a.AppVersionTo == null || appVersion.CompareTo(a.AppVersionTo) <= 0));

            if (executableDebugInfo == null) return false;

            string output;

            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.FileName = Path.Combine(_ndkPath, "toolchains", "llvm", "prebuilt", "windows-x86_64", "bin", "llvm-addr2line.exe");
                    process.StartInfo.Arguments = $"-i -C -f -e {executableDebugInfo.Path} {pc.Substring(2)}";

                    process.Start();
                    var reader = process.StandardOutput;
                    output = reader.ReadToEnd();
                    process.WaitForExit();
                }

                var lineNumbers = new StringBuilder();

                foreach (Match match in Regex.Matches(output, AndroidSignalLineNumberRegEx))
                {
                    var line = match.Groups[1].Value;
                    line = line.Substring(line.LastIndexOf('\\') + 1);
                    if (lineNumbers.Length != 0) lineNumbers.Append(',');
                    lineNumbers.Append(line);
                }

                if (lineNumbers.Length == 0) return false;

                lineNumber = lineNumbers.ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ConvertAndroidExceptionStack(bool parse, ref string stack, out string stackId)
        {
            var stackIdBuf = new StringBuilder();

            stack = Regex.Replace(stack, _androidExceptionRegEx, ((Match match) =>
            {
                stackIdBuf.Append(match.Groups[1].Value);

                return parse ? "@" + match.Value : match.Value;
            }));
            
            stackId = stackIdBuf.ToString();
        }

        private void ConvertIOSStack(bool parse, ref string stack, out string stackId)
        {
            var stackIdBuf = new StringBuilder();

            stack = Regex.Replace(stack, _iOSStackRegEx, ((Match match) =>
            {
                stackIdBuf.Append(match.Groups[1].Value);

                return parse ? "@" + match.Value : match.Value;
            }));

            stackId = stackIdBuf.ToString();
        }
    }
}
