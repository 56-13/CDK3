using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets
{
    internal class ResourceManager
    {
        public string DirectoryPath { private set; get; }
        
        private int _key;

        private ResourceManager()
        {
            DirectoryPath = Path.Combine("Cache", Process.GetCurrentProcess().Id.ToString());
        }

        private void Dispose()
        {
            if (Directory.Exists(DirectoryPath)) Directory.Delete(DirectoryPath, true);
        }

        public string AddFile(string path)
        {
            _key++;

            string copyPath = Path.Combine(DirectoryPath, $"{_key}{Path.GetExtension(path)}");

            Directory.CreateDirectory(DirectoryPath);
            File.Delete(copyPath);
            File.Copy(path, copyPath);

            return copyPath;
        }

        private class BitmapResource : IResource
        {
            public string Path { private set; get; }
            public Bitmap Value { private set; get; }

            public BitmapResource(string path, Bitmap value)
            {
                Path = path;
                Value = value;
            }

            public int Cost => Value.Width * Value.Height * Image.GetPixelFormatSize(Value.PixelFormat) / 8;

            public void Dispose()
            {
                if (Directory.Exists(System.IO.Path.GetDirectoryName(Path))) Value.Save(Path);
                Value.Dispose();
            }
        }

        private string GetBitmapPath(int key) => Path.Combine(DirectoryPath, $"{key}.png");

        public int AddBitmap(Bitmap bitmap)
        {
            if (bitmap == null) return 0;

            _key++;

            var path = GetBitmapPath(_key);
            var resource = new BitmapResource(path, bitmap);

            ResourcePool.Instance.Add(path, resource, 0, false);

            return _key;
        }

        public Bitmap GetBitmap(int key, bool loading = true)
        {
            if (key == 0) return null;

            var path = GetBitmapPath(key);
            var resource = (BitmapResource)ResourcePool.Instance.Get(path);

            if (resource != null) return resource.Value;

            if (!loading) return null;

            AssetManager.Instance.Purge();

            var bitmap = BitmapTexture.Load(path);
            resource = new BitmapResource(path, bitmap);
            ResourcePool.Instance.Add(path, resource, 0, false);

            return bitmap;
        }

        public void LoadTextureCache(int bitmapKey, string textureCache)
        {
            Directory.CreateDirectory(DirectoryPath);
            foreach (var file in Directory.GetFiles(Path.GetDirectoryName(textureCache), $"{Path.GetFileName(textureCache)}*.pvr"))
            {
                var filename = Path.GetFileName(file);
                var suffix = filename.Substring(filename.IndexOf('.') + 1);
                File.Copy(file, Path.Combine(DirectoryPath, $"{bitmapKey}.{suffix}"));
            }
        }

        public void SaveTextureCache(int bitmapKey, RawFormat[] formats, int mipmapCount, string textureCache)
        {
            var list = new List<string>();

            Texture.GetCachePaths(Path.Combine(DirectoryPath, $"{bitmapKey}"), formats, mipmapCount, list);

            foreach (var file in list)
            {
                if (File.Exists(file))
                {
                    var filename = Path.GetFileName(file);
                    var suffix = filename.Substring(filename.IndexOf('.') + 1);
                    File.Copy(file, $"{textureCache}.{suffix}");
                }
            }
        }

        public void ClearTextureCache(string textureCache)
        {
            foreach (var file in Directory.GetFiles(Path.GetDirectoryName(textureCache), $"{Path.GetFileName(textureCache)}*.pvr"))
            {
                File.Delete(file);
            }
        }

        public Texture GetTexture(in TextureDescription desc, int bitmapKey, bool loading = true)
        {
            if (bitmapKey == 0) return null;

            var key = (desc.Target, desc.Format, bitmapKey);

            var texture = (Texture)ResourcePool.Instance.Get(key);

            if (texture != null)
            {
                if (texture.Reload(desc)) return texture;

                ResourcePool.Instance.Remove(key);
            }

            if (!loading) return null;

            AssetManager.Instance.Purge();

            var bitmap = GetBitmap(bitmapKey);

            var cachePath = Path.Combine(DirectoryPath, _key.ToString());

            texture = new Texture(bitmap, desc, cachePath);

            ResourcePool.Instance.Add(key, texture, 0, false);

            return texture;
        }

        public void Purge(long cost) => ResourcePool.Instance.Purge(cost);

        private Terrain.TerrainShader _TerrainShader;
        public Terrain.TerrainShader TerrainShader
        {
            get
            {
                if (_TerrainShader == null) _TerrainShader = new Terrain.TerrainShader();

                return _TerrainShader;
            }
        }

        public static ResourceManager Instance { private set; get; }

        public static void CreateShared()
        {
            if (Instance == null) Instance = new ResourceManager();
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
