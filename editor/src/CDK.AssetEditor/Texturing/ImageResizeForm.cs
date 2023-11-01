using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

using CDK.Assets.Support;

namespace CDK.Assets.Texturing
{
    partial class ImageResizeForm : Form
    {
        private SubImageElement _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SubImageElement Element
        {
            set
            {
                if (_Element != value)
                {
                    _Element = value;

                    if (_Element != null)
                    {
                        widthUpDown.Value = _Element.Frame.Width;
                        heightUpDown.Value = _Element.Frame.Height;
                    }
                }
            }
            get => _Element;
        }
        

        public ImageResizeForm()
        {
            InitializeComponent();
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            widthUpDown.ReadOnly = !radioButton1.Checked;
            heightUpDown.ReadOnly = !radioButton1.Checked;
            rateUpDown.ReadOnly = radioButton1.Checked;
        }

        private void RateUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (_Element != null)
            {
                widthUpDown.Value = _Element.Frame.Width * rateUpDown.Value / 100;
                heightUpDown.Value = _Element.Frame.Height * rateUpDown.Value / 100;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (_Element != null && _Element.Parent.RootImage != null)
            {
                var width = (int)widthUpDown.Value;
                var height = (int)heightUpDown.Value;

                if (width == 0 || height == 0) return;

                AssetManager.Instance.Purge();

                var prevImg = _Element.Parent.RootImage.Content.Bitmap;

                if (prevImg != null)
                {
                    Bitmap nextImg;

                    var stretch = stretchCheckBox.Checked;

                    int x, y;

                    var frame = _Element.Frame;

                    if (width <= _Element.Frame.Width && height <= _Element.Frame.Height)
                    {
                        nextImg = new Bitmap(prevImg);
                        using (var s = new BitmapScratch(nextImg))
                        {
                            s.Clear(_Element.Frame);
                        }
                        x = frame.X;
                        y = frame.Y;
                    }
                    else
                    {
                        nextImg = new Bitmap(Math.Max(prevImg.Width, height), prevImg.Height + height + SubImageAsset.Padding);
                        using (var s = new BitmapScratch(nextImg))
                        {
                            s.Copy(prevImg, 0, 0);

                            s.Clear(frame);
                        }
                        x = 0;
                        y = prevImg.Height + SubImageAsset.Padding;
                    }
                    using (var g = Graphics.FromImage(nextImg))
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        if (stretch)
                        {
                            var hw = Math.Min(width, frame.Width) * 0.5f;
                            var hh = Math.Min(height, frame.Height) * 0.5f;
                            var lhw = (int)hw;
                            var lhh = (int)hh;
                            var rhw = (int)Math.Ceiling(hw);
                            var rhh = (int)Math.Ceiling(hh);

                            g.DrawImage(prevImg, new Rectangle(x, y, lhw, lhh), new Rectangle(frame.X, frame.Y, lhw, lhh), GraphicsUnit.Pixel);
                            g.DrawImage(prevImg, new Rectangle(x + width - rhw, y, rhw, lhh), new Rectangle(frame.Right - rhw, frame.Y, rhw, lhh), GraphicsUnit.Pixel);
                            g.DrawImage(prevImg, new Rectangle(x, y + height - rhh, lhw, rhh), new Rectangle(frame.Left, frame.Bottom - rhh, lhw, rhh), GraphicsUnit.Pixel);
                            g.DrawImage(prevImg, new Rectangle(x + width - rhw, y + height - rhh, rhw, rhh), new Rectangle(frame.Right - rhw, frame.Bottom - rhh, rhw, rhh), GraphicsUnit.Pixel);
                            if (width > frame.Width)
                            {
                                g.DrawImage(prevImg, new Rectangle(x + lhw, y, width - frame.Width, height), new Rectangle(frame.Left + lhw, frame.Top, 1, frame.Height), GraphicsUnit.Pixel);
                            }
                            if (height > frame.Height)
                            {
                                g.DrawImage(prevImg, new Rectangle(x, y + lhh, width, height - frame.Height), new Rectangle(frame.Left, frame.Top + lhh, frame.Width, 1), GraphicsUnit.Pixel);
                            }
                        }
                        else
                        {
                            g.DrawImage(prevImg, new Rectangle(x, y, width, height), frame, GraphicsUnit.Pixel);
                        }
                    }
                    _Element.Parent.RootImage.Content.Bitmap = nextImg;
                    _Element.Frame = new Rectangle(x, y, width, height);

                    DialogResult = DialogResult.OK;

                    Close();
                }
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }
    }
}
