using System;
using System.Xml;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Configs
{
    public class ResourceConfig : IDisposable
    {
        public string SkyboxPath { private set; get; }

        private Texture _SkyboxMap;
        public Texture SkyboxMap
        {
            get
            {
                if (_SkyboxMap == null)
                {
                    AssetManager.Instance.Purge();

                    var path = Path.Combine("Resources", "Skybox", SkyboxPath);

                    if (File.Exists(path))
                    {
                        using (var bitmap = BitmapTexture.Load(path))
                        {
                            SkyboxColor = (Color3)BitmapTexture.Average(bitmap);

                            _SkyboxMap = new Texture(bitmap, new TextureDescription()
                            {
                                Target = TextureTarget.TextureCubeMap,
                                Format = RawFormat.Srgb8,
                                WrapS = TextureWrapMode.ClampToEdge,
                                WrapT = TextureWrapMode.ClampToEdge,
                                WrapR = TextureWrapMode.ClampToEdge,
                                MinFilter = TextureMinFilter.LinearMipmapLinear,
                                MagFilter = TextureMagFilter.Linear,
                                MipmapCount = 10
                            }, "skybox");
                        }
                    }
                }
                return _SkyboxMap;
            }
        }

        public Color3 SkyboxColor { private set; get; }
        
        public ResourceConfig(XmlNode node)
        {
            SkyboxPath = node.GetChildNode("skybox").ReadAttributeString("path");
        }

        public void Dispose()
        {
            _SkyboxMap?.Dispose();
        }
    }
}
