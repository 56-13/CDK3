using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using CDK.Assets.Containers;

namespace CDK.Assets
{
    internal class AssetTimes
    {
        private ProjectAsset project;
        private SortedDictionary<string, DateTime> times;
        private bool saving;

        private string Path
        {
            get
            {
                return project.ContentPath + ".times";
            }
        }

        public AssetTimes(ProjectAsset project, ISet<string> hierachyKeys)
        {
            this.project = project;

            times = new SortedDictionary<string, DateTime>();

            if (hierachyKeys != null)
            {
                Load(hierachyKeys);
            }
        }

        public void Update(string source, DateTime time)
        {
            if (time.Equals(DateTime.MinValue))
            {
                times.Remove(source);
            }
            else if (times.ContainsKey(source))
            {
                times[source] = time;
            }
            else
            {
                times.Add(source, time);
            }
            if (!saving)
            {
                saving = true;
                AssetManager.Instance.Invoke(Save);
            }
        }

        public DateTime GetTime(string source)
        {
            return times.TryGetValue(source, out var time) ? time : DateTime.MinValue;
        }

        private void Load(ISet<string> hierachyKeys)
        {
            times.Clear();

            if (File.Exists(Path))
            {
                var line = 1;

                var resave = false;

                using (var sr = File.OpenText(Path))
                {
                    string str;
                    while ((str = sr.ReadLine()) != null)
                    {
                        if (str.StartsWith("<") || str.StartsWith("=") || str.StartsWith(">"))
                        {
                            resave = true;
                        }
                        else
                        {
                            var param = str.Split(',');

                            if (param.Length != 2)
                            {
                                throw new IOException(string.Format("잘못된 시간 파일입니다. 라인({0})", line));
                            }
                            else if (!hierachyKeys.Contains(param[0]))
                            {
                                resave = true;
                            }
                            else
                            {
                                var key = param[0];
                                var newTime = DateTime.Parse(param[1]);

                                if (times.TryGetValue(key, out var oldTime))
                                {
                                    if (oldTime.CompareTo(newTime) < 0) times[key] = newTime;
                                }
                                else times.Add(key, newTime);
                            }
                        }
                        line++;
                    }
                }
                if (resave)
                {
                    if (!saving)
                    {
                        saving = true;
                        AssetManager.Instance.Invoke(Save);
                    }
                    AssetManager.Instance.Message("시간 파일이 재저장 되었습니다.");
                }
            }
        }

        private void Save()
        {
            if (saving)
            {
                if (times.Count != 0)
                {
                    try
                    {
                        using (var fs = File.Open(Path, FileMode.Create, FileAccess.Write))
                        {
                            using (var writer = new StreamWriter(fs))
                            {
                                foreach (var key in times.Keys)
                                {
                                    writer.Write(key);
                                    writer.Write(',');
                                    writer.Write(times[key]);
                                    writer.WriteLine();
                                }
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    File.Delete(Path);
                }
                saving = false;
            }
        }
    }
}
