using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.IO;

using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Interops;

namespace CDK.Assets
{
    public enum AudioControl
    {
        Bgm,
        Effect,
        Voice
    }

    internal class AudioPlayer
    {
        private Thread _thread;
        private bool _alive;
        private List<Action> _actions;
        private VlcManager _manager;

        enum PlayerState
        {
            Playing,
            Paused,
            Stopped
        }
        private class Player
        {
            public int handle;
            public VlcMediaPlayer player;
            public float volume;
            public AudioControl control;
            public int loopOrigin;
            public int loop;
            public Action<int> stopcb;
            public PlayerState state;
        }
        private List<Player> _players;
        private float[] _volumes;
        private int _handleSeed;

        private AudioPlayer()
        {
            _actions = new List<Action>();
            _alive = true;
            _thread = new Thread(new ThreadStart(ThreadRun));
            _volumes = new float[3];
            for (int i = 0; i < _volumes.Length; i++) _volumes[i] = 1;
            _thread.Start();
        }

        private void Dispose()
        {
            _alive = false;

            _thread.Join();
        }

        private void ThreadRun()
        {
            var libDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "libvlc", IntPtr.Size == 4 ? "x86" : "x64"));
            _manager = new VlcManager(libDirectory, null);
            _players = new List<Player>();

            while (_alive)
            {
                Action action;
                lock (_actions)
                {
                    if (_actions.Count != 0)
                    {
                        action = _actions[0];
                        _actions.RemoveAt(0);
                    }
                    else action = null;
                }
                if (action != null) action.Invoke();
                else Thread.Sleep(10);
            }

            _actions.Clear();

            foreach (var player in _players)
            {
                if (player.state != PlayerState.Stopped) player.player.Stop();
                //p.player.Dispose();       
                //TODO:Vlc.DotNet.Core 자체적인 문제, dispose시에 manager를 dispose시켜 버린다.
            }
            _players.Clear();

            //manager.Dispose();
            //TODO:Vlc.DotNet.Core 자체적인 문제
        }

        private void StopHandle(Player player)
        {
            if (player.stopcb != null)
            {
                player.stopcb(player.handle);
                player.stopcb = null;
            }
            player.state = PlayerState.Stopped;
        }

        public int Play(string path, float volume, AudioControl control, int loop, Action<int> stopcb = null)
        {
            lock (_actions)
            {
                var handle = ++_handleSeed;

                _actions.Add(() =>
                {
                    var fi = new FileInfo(path);

                    if (!fi.Exists) return;

                    var player = _players.FirstOrDefault(p => p.state == PlayerState.Stopped);

                    if (player == null)
                    {
                        player = new Player
                        {
                            player = new VlcMediaPlayer(_manager)
                        };
                        player.player.EndReached += (sender, e) =>
                        {
                            if (player.loopOrigin == 0 || ++player.loop < player.loopOrigin)
                            {
                                player.player.Position = 0;
                                player.player.Play();
                            }
                            else StopHandle(player);
                        };
                        player.player.Stopped += (sender, e) => StopHandle(player);
                        player.player.EncounteredError += (sender, e) => StopHandle(player);

                        lock (_players)
                        {
                            _players.Add(player);
                        }
                    }
                    player.handle = handle;
                    player.control = control;
                    player.volume = volume;
                    player.loopOrigin = loop;
                    player.loop = 0;
                    player.stopcb = stopcb;
                    player.state = PlayerState.Playing;
                    player.player.SetMedia(fi);
                    player.player.Audio.Volume = (int)(100 * volume * _volumes[(int)control]);
                    player.player.Play();
                });

                return handle;
            };
        }

        public bool IsPlaying(int handle)
        {
            lock (_players)
            {
                return _players.Any(p => p.state != PlayerState.Stopped && p.handle == handle);
            }
        }

        public void Stop(int handle)
        {
            lock (_actions)
            {
                _actions.Add(() =>
                {
                    _players.FirstOrDefault(p => p.state != PlayerState.Stopped && p.handle == handle)?.player.Stop();
                });
            }
        }

        public void Pause(int handle)
        {
            lock (_actions)
            {
                _actions.Add(() =>
                {
                    var player = _players.FirstOrDefault(p => p.handle == handle);

                    if (player != null && player.state == PlayerState.Playing)
                    {
                        player.player.SetPause(true);
                        player.state = PlayerState.Paused;
                    }
                });
            }
        }

        public void Resume(int handle)
        {
            lock (_actions)
            {
                _actions.Add(() =>
                {
                    var player = _players.FirstOrDefault(p => p.handle == handle);

                    if (player != null && player.state == PlayerState.Paused)
                    {
                        player.player.SetPause(false);
                        player.state = PlayerState.Playing;
                    }
                });
            }
        }

        public void SetVolume(int handle, float volume)
        {
            lock (_actions)
            {
                _actions.Add(() =>
                {
                    var player = _players.FirstOrDefault(p => p.handle == handle);
                    if (player != null && player.state != PlayerState.Stopped)
                    {
                        player.player.Audio.Volume = (int)(100 * volume * _volumes[(int)player.control]);
                    }
                });
            }
        }

        public void SetLoop(int handle, int loop)
        {
            lock (_actions)
            {
                _actions.Add(() =>
                {
                    var player = _players.FirstOrDefault(p => p.handle == handle);
                    if (player != null && player.state != PlayerState.Stopped)
                    {
                        player.loopOrigin = loop;
                        if (loop != 0 && player.loop >= player.loopOrigin) player.player.Stop();
                    }
                });
            }
        }

        public void StopControl(AudioControl control)
        {
            lock (_actions)
            {
                _actions.Add(() =>
                {
                    foreach (var player in _players)
                    {
                        if (player.control == control && player.state != PlayerState.Stopped) player.player.Stop();
                    }
                });
            }
        }

        public void PauseControl(AudioControl control)
        {
            lock (_actions)
            {
                _actions.Add(() =>
                {
                    foreach (var player in _players)
                    {
                        if (player.control == control && player.state == PlayerState.Playing)
                        {
                            player.player.SetPause(true);
                            player.state = PlayerState.Paused;
                        }
                    }
                });
            }
        }

        public void ResumeControl(AudioControl control)
        {
            lock (_actions)
            {
                _actions.Add(() =>
                {
                    foreach (var player in _players)
                    {
                        if (player.control == control && player.state == PlayerState.Paused)
                        {
                            player.player.SetPause(false);
                            player.state = PlayerState.Playing;
                        }
                    }
                });
            }
        }

        public void SetControlVolume(AudioControl control, float volume)
        {
            if (_volumes[(int)control] != volume)
            {
                _volumes[(int)control] = volume;

                lock (_actions)
                {
                    _actions.Add(() =>
                    {
                        foreach (var player in _players)
                        {
                            if (player.state != PlayerState.Stopped && player.control == control) player.player.Audio.Volume = (int)(100 * player.volume * volume);
                        }
                    });
                }
            }
        }


        public static AudioPlayer Instance { private set; get; }

        public static void CreateShared()
        {
            if (Instance == null) Instance = new AudioPlayer();
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
