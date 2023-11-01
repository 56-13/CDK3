using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CDK.Assets.Support
{
    public class BitmapScratch : IDisposable
    {
        private Bitmap _scratch;
        private BitmapData _scratchData;

        public BitmapScratch(Bitmap scratch)
        {
            if (scratch.PixelFormat != PixelFormat.Format32bppArgb) throw new InvalidOperationException();

            _scratch = scratch;

            _scratchData = scratch.LockBits(new Rectangle(0, 0, scratch.Width, scratch.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        }

        public void Copy(Bitmap image, int dx, int dy, int bx, int by, Rectangle srcRect)
        {
            if (image.PixelFormat != PixelFormat.Format32bppArgb) throw new InvalidOperationException();

            var imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                var sptr = (uint*)imageData.Scan0;
                var dptr = (uint*)_scratchData.Scan0;

                var sw = image.Width;
                var sh = image.Height;
                var dw = _scratch.Width;
                var dh = _scratch.Height;

                for (var y = -by; y < srcRect.Height + by; y++)
                {
                    for (var x = -bx; x < srcRect.Width + bx; x++)
                    {
                        var scx = srcRect.X + Math.Min(Math.Max(x, 0), srcRect.Width - 1);
                        var scy = srcRect.Y + Math.Min(Math.Max(y, 0), srcRect.Height - 1);
                        var dcx = dx + x;
                        var dcy = dy + y;

                        if (scx >= 0 && scx < sw && scy >= 0 && scy < sh && dcx >= 0 && dcx < dw && dcy >= 0 && dcy < dh)
                        {
                            var si = scy * sw + scx;
                            var di = dcy * dw + dcx;
                            dptr[di] = sptr[si];
                        }
                    }
                }
            }
            image.UnlockBits(imageData);
        }

        public void Copy(Bitmap image, int dx, int dy)
        {
            Copy(image, dx, dy, 0, 0, new Rectangle(0, 0, image.Width, image.Height));
        }

        public void Clear(Rectangle rect)
        {
            rect.Intersect(new Rectangle(0, 0, _scratch.Width, _scratch.Height));

            unsafe
            {
                var dptr = (uint*)_scratchData.Scan0;

                var dw = _scratch.Width;

                for (var y = rect.Top; y < rect.Bottom; y++)
                {
                    for (var x = rect.Left; x < rect.Right; x++)
                    {
                        var di = y * dw + x;

                        dptr[di] = 0;
                    }
                }
            }
        }

        public void Dispose()
        {
            _scratch.UnlockBits(_scratchData);
        }
    }
}
