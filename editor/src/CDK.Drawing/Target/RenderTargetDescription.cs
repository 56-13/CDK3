using System;
using System.Linq;
using System.Text;

namespace CDK.Drawing
{
    public struct RenderTargetAttachmentDescription : IEquatable<RenderTargetAttachmentDescription>
    {
        public FramebufferAttachment Attachment;
        public RawFormat Format;
        public int Samples;
        public bool Texture;
        public TextureTarget TextureTarget;
        public TextureWrapMode TextureWrapS;
        public TextureWrapMode TextureWrapT;
        public TextureWrapMode TextureWrapR;
        public TextureMinFilter TextureMinFilter;
        public TextureMagFilter TextureMagFilter;
        public Color4 TextureBorderColor;
        public int TextureLayer;

        public void Validate()
        {
            Debug.Assert(Attachment != 0 && Format != 0);

            if (Samples < 1) Samples = 1;

            if (Texture)
            {
                if (TextureTarget == 0) TextureTarget = TextureTarget.Texture2D;
                if (TextureWrapS == 0) TextureWrapS = TextureWrapMode.Repeat;
                if (TextureWrapR == 0) TextureWrapR = TextureWrapMode.Repeat;
                if (TextureWrapT == 0) TextureWrapT = TextureWrapMode.Repeat;
                if (TextureMinFilter == 0) TextureMinFilter = TextureMinFilter.Nearest;
                if (TextureMagFilter == 0) TextureMagFilter = TextureMagFilter.Nearest;
            }
            else
            {
                TextureTarget = 0;
                TextureWrapS = 0;
                TextureWrapR = 0;
                TextureWrapT = 0;
                TextureMinFilter = 0;
                TextureMagFilter = 0;
                TextureBorderColor = Color4.Transparent;
                TextureLayer = 0;
            }
        }

        public static bool operator ==(in RenderTargetAttachmentDescription left, in RenderTargetAttachmentDescription right) => left.Equals(right);
        public static bool operator !=(in RenderTargetAttachmentDescription left, in RenderTargetAttachmentDescription right) => !left.Equals(right);
        public override string ToString() => $"Attachment:{Attachment} Format:{Format} Texture:{Texture}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Attachment.GetHashCode());
            hash.Combine(Format.GetHashCode());
            hash.Combine(Samples.GetHashCode());
            hash.Combine(Texture.GetHashCode());
            hash.Combine(TextureTarget.GetHashCode());
            hash.Combine(TextureWrapS.GetHashCode());
            hash.Combine(TextureWrapT.GetHashCode());
            hash.Combine(TextureWrapR.GetHashCode());
            hash.Combine(TextureMinFilter.GetHashCode());
            hash.Combine(TextureMagFilter.GetHashCode());
            hash.Combine(TextureBorderColor.GetHashCode());
            hash.Combine(TextureLayer.GetHashCode());
            return hash;
        }

        public bool Equals(RenderTargetAttachmentDescription other)
        {
            return Attachment == other.Attachment &&
                Format == other.Format &&
                Samples == other.Samples &&
                Texture == other.Texture &&
                TextureTarget == other.TextureTarget &&
                TextureWrapS == other.TextureWrapS &&
                TextureWrapT == other.TextureWrapT &&
                TextureWrapR == other.TextureWrapR &&
                TextureMinFilter == other.TextureMinFilter &&
                TextureMagFilter == other.TextureMagFilter &&
                TextureBorderColor == other.TextureBorderColor &&
                TextureLayer == other.TextureLayer;
        }

        public override bool Equals(object obj) => obj is RenderTargetAttachmentDescription other && Equals(other);
    }

    public struct RenderTargetDescription : IEquatable<RenderTargetDescription>
    {
        public int Width;
        public int Height;
        public RenderTargetAttachmentDescription[] Attachments;

        internal void Validate()
        {
            Debug.Assert(Width > 0 && Height > 0);
            if (Attachments != null)
            {
                for (var i = 0; i < Attachments.Length; i++) Attachments[i].Validate();
            }
        }

        public static bool operator ==(in RenderTargetDescription left, in RenderTargetDescription right) => left.Equals(right);
        public static bool operator !=(in RenderTargetDescription left, in RenderTargetDescription right) => !left.Equals(right);
        public override string ToString()
        {
            var strbuf = new StringBuilder();
            strbuf.Append($"Size:{Width}, {Height}");
            if (Attachments != null)
            {
                foreach (var attachment in Attachments)
                {
                    strbuf.Append(' ');
                    strbuf.Append(attachment.ToString());
                }
            }
            return strbuf.ToString();
        }

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Width.GetHashCode());
            hash.Combine(Height.GetHashCode());
            if (Attachments != null) 
            {
                foreach (var attachment in Attachments) hash.Combine(attachment.GetHashCode());
            }
            return hash;
        }

        public bool Equals(RenderTargetDescription other)
        {
            return Width == other.Width &&
                Height == other.Height &&
                (Attachments != null && Attachments.Length > 0 ? (other.Attachments != null && Attachments.SequenceEqual(other.Attachments)) : (other.Attachments == null || other.Attachments.Length == 0));
        }

        public override bool Equals(object obj) => obj is RenderTargetDescription other && Equals(other);
    }
}
