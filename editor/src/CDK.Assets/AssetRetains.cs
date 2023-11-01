using System;
using System.Collections.Generic;
using System.IO;

using CDK.Assets.Containers;

namespace CDK.Assets
{
    internal class AssetRetains
    {
        private ProjectAsset _project;
        private SortedDictionary<string, SortedSet<string>> _originToTargets;
        private SortedDictionary<string, SortedSet<string>> _targetToOrigins;
        private bool _saving;
        private string Path => _project.ContentPath + ".retains";

        public AssetRetains(ProjectAsset project, ISet<string> hierachyKeys)
        {
            _project = project;

            _originToTargets = new SortedDictionary<string, SortedSet<string>>();
            _targetToOrigins = new SortedDictionary<string, SortedSet<string>>();

            if (hierachyKeys != null) Load(hierachyKeys);
        }

        public void Update(string origin, SortedSet<string> newTargets)
        {
            if (newTargets == null || newTargets.Count == 0)
            {
                if (_originToTargets.TryGetValue(origin, out var oldTargets))
                {
                    foreach (var oldTarget in oldTargets)
                    {
                        if (_targetToOrigins.TryGetValue(oldTarget, out var origins))
                        {
                            if (origins.Remove(origin) && origins.Count == 0)
                            {
                                _targetToOrigins.Remove(oldTarget);
                            }
                        }
                    }
                    _originToTargets.Remove(origin);
                }
                else return;
            }
            else
            {
                if (_originToTargets.TryGetValue(origin, out var oldTargets))
                {
                    var diff = false;

                    foreach (var oldTarget in oldTargets)
                    {
                        if (!newTargets.Contains(oldTarget))
                        {
                            if (_targetToOrigins.TryGetValue(oldTarget, out var origins))
                            {
                                origins.Remove(origin);
                            }
                            diff = true;
                        }
                    }
                    foreach (string newTarget in newTargets)
                    {
                        if (!oldTargets.Contains(newTarget))
                        {
                            if (!_targetToOrigins.TryGetValue(newTarget, out var origins))
                            {
                                origins = new SortedSet<string>();
                                _targetToOrigins.Add(newTarget, origins);
                            }
                            origins.Add(origin);

                            diff = true;
                        }
                    }
                    if (diff) _originToTargets[origin] = newTargets;
                    else return;
                }
                else
                {
                    foreach (var newTarget in newTargets)
                    {
                        if (!_targetToOrigins.TryGetValue(newTarget, out var origins))
                        {
                            origins = new SortedSet<string>();
                            _targetToOrigins.Add(newTarget, origins);
                        }
                        origins.Add(origin);
                    }
                    _originToTargets.Add(origin, newTargets);
                }
            }
            if (!_saving)
            {
                _saving = true;
                AssetManager.Instance.Invoke(Save);
            }
        }

        public SortedSet<string> GetOrigins(string target)
        {
            return _targetToOrigins.TryGetValue(target, out var origins) ? origins : null;
        }

        public SortedSet<string> GetTargets(string origin)
        {
            return _originToTargets.TryGetValue(origin, out var targets) ? targets : null;
        }

        public void Save()
        {
            if (_saving)
            {
                if (_originToTargets.Count != 0)
                {
                    try
                    {
                        using (var fs = File.Open(Path, FileMode.Create, FileAccess.Write))
                        {
                            using (var writer = new StreamWriter(fs))
                            {
                                foreach (var key in _originToTargets.Keys)
                                {
                                    writer.Write(key);
                                    foreach (var target in _originToTargets[key])
                                    {
                                        writer.Write(',');
                                        writer.Write(target);
                                    }
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
                _saving = false;
            }
        }

        private void Load(ISet<string> hierachyKeys)
        {
            _originToTargets.Clear();
            _targetToOrigins.Clear();

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
                            var keys = str.Split(',');

                            if (keys.Length == 1)
                            {
                                throw new IOException(string.Format("잘못된 참조 파일입니다. 라인({0})", line));
                            }
                            else if (!hierachyKeys.Contains(keys[0]))
                            {
                                resave = true;
                            }
                            else
                            {
                                var targets = new SortedSet<string>();
                                for (var i = 1; i < keys.Length; i++)
                                {
                                    if (hierachyKeys.Contains(keys[i])) targets.Add(keys[i]);
                                    else resave = true;
                                }
                                if (_originToTargets.ContainsKey(keys[0]))
                                {
                                    var prevTargets = _originToTargets[keys[0]];

                                    if (!prevTargets.SetEquals(targets))
                                    {
                                        throw new IOException(string.Format("중복된 참조 정보가 있습니다. 라인({0})", line));
                                    }
                                }
                                else if (targets.Count != 0)
                                {
                                    _originToTargets.Add(keys[0], targets);

                                    foreach (var target in targets)
                                    {
                                        if (!_targetToOrigins.TryGetValue(target, out var origins))
                                        {
                                            origins = new SortedSet<string>();
                                            _targetToOrigins.Add(target, origins);
                                        }
                                        origins.Add(keys[0]);
                                    }
                                }
                            }
                        }
                        line++;
                    }
                }
                if (resave)
                {
                    if (!_saving)
                    {
                        _saving = true;
                        AssetManager.Instance.Invoke(Save);
                    }
                    AssetManager.Instance.Message("참조 파일이 재저장 되었습니다.");
                }
            }
        }
    }
}
