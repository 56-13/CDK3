using System.IO;

namespace CDK.Drawing
{
    public static class Textures
    {
        public static Texture BRDF
        {
            get
            {
                var texture = (Texture)ResourcePool.Instance.Get("BRDF");
                
                if (texture == null)
                {
                    var path = Path.Combine("Resources", "brdf.png");

                    using (var bitmap = BitmapTexture.Load(path))
                    {
                        texture = new Texture(bitmap, new TextureDescription()
                        {
                            Format = RawFormat.Rg8
                        }, null);
                    }

                    ResourcePool.Instance.Add("BRDF", texture, 0, false);
                }
                return texture;
            }
        }

        public static Texture New(object key, int life, bool recycle, TextureDescription desc, bool allocate = true)
        {
            desc.Validate();

            if (ResourcePool.Instance.Recycle(((IResource candidate) => candidate is Texture texture && texture.Description == desc), key, life, out var resource))
            {
                var texture = (Texture)resource;
                if (allocate) texture.Allocate();
                return texture;
            }
            var newTexture = new Texture(desc, allocate);
            ResourcePool.Instance.Add(key, newTexture, life, recycle);
            return newTexture;
        }

        public static Texture NewTemporary(TextureDescription desc, bool allocate = true) => New(null, 1, true, desc, allocate);
    }
}
