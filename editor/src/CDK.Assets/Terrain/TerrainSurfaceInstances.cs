using System.IO;
using System.Drawing;

namespace CDK.Assets.Terrain
{
    internal class TerrainSurfaceInstances
    {
        public int Width { private set; get; }
        public int Height { private set; get; }
        public int FillCount { private set; get; }

        private TerrainSurfaceInstance[,] _instances;
        private int _id;
        private static int _idSeed;

        private string ContentPath => Path.Combine(ResourceManager.Instance.DirectoryPath, $"surface-instances-{_id}");

        public TerrainSurfaceInstances(int width, int height)
        {
            Width = width;
            Height = height;

            _id = ++_idSeed;
        }

        public TerrainSurfaceInstances(TerrainSurfaceInstances other)
        {
            Width = other.Width;
            Height = other.Height;
            FillCount = other.FillCount;
            _id = ++_idSeed;

            if (other._instances != null)
            {
                _instances = (TerrainSurfaceInstance[,])other._instances.Clone();
            }
            else if (File.Exists(other.ContentPath))
            {
                File.Copy(other.ContentPath, ContentPath);
            }
        }

        public void Save(BinaryWriter writer)
        {
            Load();

            writer.Write(FillCount);

            if (FillCount != 0)
            {
                for (var sy = 0; sy < Height; sy++)
                {
                    for (var sx = 0; sx < Width; sx++)
                    {
                        if (!_instances[sx, sy].IsEmpty)
                        {
                            writer.Write((short)sx);
                            writer.Write((short)sy);
                            writer.Write(_instances[sx, sy].Intermediate);
                            writer.Write(_instances[sx, sy].Current);
                        }
                    }
                }
            }
        }

        public void Load(BinaryReader reader)
        {
            _instances = null;

            FillCount = reader.ReadInt32();

            if (FillCount != 0)
            {
                Directory.CreateDirectory(ResourceManager.Instance.DirectoryPath);

                using (var fs = new FileStream(ContentPath, FileMode.Create))
                {
                    using (var writer = new BinaryWriter(fs))
                    {
                        writer.Write(FillCount);
                        writer.Write(reader.ReadBytes(FillCount * 20));
                    }
                }
            }
        }

        public void Purge()
        {
            if (FillCount != 0)
            {
                if (_instances != null)
                {
                    Directory.CreateDirectory(ResourceManager.Instance.DirectoryPath);

                    using (var fs = new FileStream(ContentPath, FileMode.Create))
                    {
                        using (var writer = new BinaryWriter(fs))
                        {
                            Save(writer);
                        }
                    }
                    _instances = null;
                }
            }
            else
            {
                File.Delete(ContentPath);
            }
        }

        private void Load()
        {
            if (FillCount != 0 && _instances == null)
            {
                using (var fs = new FileStream(ContentPath, FileMode.Open))
                {
                    using (var reader = new BinaryReader(fs))
                    {
                        _instances = new TerrainSurfaceInstance[Width, Height];

                        FillCount = reader.ReadInt32();
                        for (var i = 0; i < FillCount; i++)
                        {
                            int sx = reader.ReadInt16();
                            int sy = reader.ReadInt16();
                            var intermediate = reader.ReadDouble();
                            var current = reader.ReadDouble();
                            _instances[sx, sy] = new TerrainSurfaceInstance(intermediate, current);
                        }
                    }
                }
                File.Delete(ContentPath);
            }
        }

        public TerrainSurfaceInstance this[int sx, int sy]
        {
            set
            {
                Load();

                if (value.IsEmpty)
                {
                    if (_instances != null && !_instances[sx, sy].IsEmpty)
                    {
                        if (--FillCount == 0) _instances = null;
                        else _instances[sx, sy] = value;
                    }
                }
                else
                {
                    if (_instances == null)
                    {
                        _instances = new TerrainSurfaceInstance[Width, Height];

                        FillCount++;
                    }
                    else if (_instances[sx, sy].IsEmpty)
                    {
                        FillCount++;
                    }
                    _instances[sx, sy] = value;
                }
            }
            get
            {
                Load();

                return _instances != null ? _instances[sx, sy] : TerrainSurfaceInstance.Empty;
            }
        }

        public TerrainSurfaceInstance this[in Point sp]
        {
            set => this[sp.X, sp.Y] = value;
            get => this[sp.X, sp.Y];
        }

        public bool IsEmpty => FillCount == 0;
    }
}
