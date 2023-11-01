using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

using CDK.Drawing;

using CDK.Assets.Support;

using GDIRectangle = System.Drawing.Rectangle;

namespace CDK.Assets.Texturing
{
    public partial class ImageAssetControl : UserControl
    {
        private RootImageAsset _Asset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RootImageAsset Asset
        {
            set
            {
                if (_Asset != value)
                {
                    if (_Asset != null)
                    {
                        colorComboBox.DataBindings.Clear();
                        encodingComboBox.DataBindings.Clear();
                        contentScaleComboBox.DataBindings.Clear();
                        hasSubImagesCheckBox.DataBindings.Clear();

                        _Asset.PropertyChanged -= Asset_PropertyChanged;
                        _Asset.Content.PropertyChanged -= Content_PropertyChanged;
                    }

                    SubAsset = null;
                    _Asset = value;

                    if (_Asset != null)
                    {
                        colorComboBox.DataBindings.Add("SelectedIndex", _Asset.ScreenColor, "SelectedIndex", false, DataSourceUpdateMode.OnPropertyChanged);
                        encodingComboBox.DataBindings.Add("SelectedItem", _Asset, "Encoding", false, DataSourceUpdateMode.OnPropertyChanged);
                        contentScaleComboBox.DataBindings.Add("SelectedItem", _Asset, "ContentScale", false, DataSourceUpdateMode.OnPropertyChanged);
                        hasSubImagesCheckBox.DataBindings.Add("Checked", _Asset, "HasSubImages", false, DataSourceUpdateMode.OnPropertyChanged);

                        _Asset.PropertyChanged += Asset_PropertyChanged;
                        _Asset.Content.PropertyChanged += Content_PropertyChanged;
                    }
                    screenControl.Asset = _Asset;
                    textureDescriptionControl.Description = _Asset?.TextureDescription;

                    ResetOriginSize();
                    ResetEncodedSize();
                    ResetScaledSize();

                    containerAssetControl.Asset = _Asset;

                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }

        public event EventHandler AssetChanged;

        private SubImageAsset _SubAsset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SubImageAsset SubAsset
        {
            set
            {
                if (_SubAsset != value)
                {
                    if (value != null)
                    {
                        Asset = value.RootImage;
                    }

                    _SubAsset = value;

                    subControl.Asset = _SubAsset;
                    subControl.Visible = _SubAsset != null;

                    screenControl.SubAsset = _SubAsset;

                    SubAssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _SubAsset;
        }

        public event EventHandler SubAssetChanged;

        public ImageAssetControl()
        {
            InitializeComponent();

            colorComboBox.DataSource = ScreenColor.Items;
            encodingComboBox.DataSource = Enum.GetValues(typeof(ImageAssetEncoding));

            ratioComboBox.DataSource = ScreenRatio.Items;
            ratioComboBox.DataBindings.Add("SelectedIndex", screenControl.Ratio, "SelectedIndex", false, DataSourceUpdateMode.OnPropertyChanged);

            pivotCheckBox.DataBindings.Add("Checked", screenControl, "Pivot", false, DataSourceUpdateMode.OnPropertyChanged);

            contentScaleComboBox.Items.Add(1.0f);
            contentScaleComboBox.Items.Add(0.5f);
            contentScaleComboBox.Items.Add(0.25f);

            openFileDialog.Filter = FileFilters.Image;

            Disposed += ImageAssetControl_Disposed;
        }

        private void ImageAssetControl_Disposed(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                _Asset.PropertyChanged -= Asset_PropertyChanged;
                _Asset.Content.PropertyChanged -= Content_PropertyChanged;
            }
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Encoding":
                    ResetEncodedSize();
                    break;
                case "ContentScale":
                    ResetScaledSize();
                    break;
            }
        }

        private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Bitmap":
                    ResetOriginSize();
                    ResetEncodedSize();
                    ResetScaledSize();
                    break;
                case "SubImages":
                    if (_SubAsset != null && _Asset.SubImages.Contains(_SubAsset))
                    {
                        SubAsset = null;
                    }
                    break;
            }
        }

        private void ResetOriginSize()
        {
            var image = _Asset?.Content.Bitmap;

            if (image != null)
            {
                widthUpDown.Value = image.Width;
                heightUpDown.Value = image.Height;
            }
            else
            {
                widthUpDown.Value = 0;
                heightUpDown.Height = 0;
            }
        }

        private void ResetEncodedSize()
        {
            var image = _Asset?.Content.Bitmap;

            if (image != null)
            {
                var size = image.Size;
                Texture.GetEncodedSize(ref size, _Asset.GetFormats());
                encodedWidthUpDown.Value = size.Width;
                encodedHeightUpDown.Value = size.Height;
            }
            else
            {
                encodedWidthUpDown.Value = 0;
                encodedHeightUpDown.Height = 0;
            }
        }

        private void ResetScaledSize()
        {
            var image = _Asset?.Content.Bitmap;

            if (image != null)
            {
                scaledWidthUpDown.Value = (decimal)(image.Width * _Asset.ContentScale);
                scaledHeightUpDown.Value = (decimal)(image.Height * _Asset.ContentScale);
            }
            else
            {
                scaledWidthUpDown.Value = 0;
                scaledHeightUpDown.Height = 0;
            }
        }

        private void ScreenControl_SubAssetChanged(object sender, EventArgs e)
        {
            SubAsset = screenControl.SubAsset;
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _Asset.Import(openFileDialog.FileName);
                    }
                    catch (Exception uex)
                    {
                        ErrorHandler.Record(uex);

                        MessageBox.Show($"{openFileDialog.FileName} 파일을 로드할 수 없습니다.");
                    }
                }
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    var filenames = openFileDialog.FileNames;
                    Array.Sort(filenames);

                    AssetManager.Instance.Purge();

                    var scratch = _Asset.Content.Bitmap;

                    BitmapScratch scratchTarget;

                    int x = 0, y, w, h;

                    if (scratch != null)
                    {
                        y = scratch.Height + SubImageAsset.Padding;
                        h = scratch.Height;
                        w = scratch.Width;
                        scratch = new Bitmap(scratch);
                        scratchTarget = new BitmapScratch(scratch);
                    }
                    else
                    {
                        y = 0;
                        h = 0;
                        w = 0;
                        scratchTarget = null;
                    }

                    foreach (string filename in filenames)
                    {
                        try
                        {
                            using (var image = BitmapTexture.Load(filename))
                            {
                                var resetScratch = false;

                                if (x + image.Width > 2048)
                                {
                                    if (w < image.Width)
                                    {
                                        w = image.Width;
                                    }
                                    x = 0;
                                    y = h + SubImageAsset.Padding;
                                    h = y + image.Height;
                                    resetScratch = true;
                                }
                                else
                                {
                                    if (h < y + image.Height)
                                    {
                                        h = y + image.Height;
                                        resetScratch = true;
                                    }
                                    if (w < x + image.Width)
                                    {
                                        w = x + image.Width;
                                        resetScratch = true;
                                    }
                                }
                                if (resetScratch)
                                {
                                    var newScratch = new Bitmap(w, h);
                                    var newScratchTarget = new BitmapScratch(newScratch);
                                    if (scratch != null)
                                    {
                                        scratchTarget.Dispose();
                                        newScratchTarget.Copy(scratch, 0, 0);
                                        scratch.Dispose();
                                    }
                                    scratch = newScratch;
                                    scratchTarget = newScratchTarget;
                                }
                                scratchTarget.Copy(image, x, y);

                                var asset = new SubImageAsset();
                                asset.MainElement.Frame = new GDIRectangle(x, y, image.Width, image.Height);
                                _Asset.Children.Add(asset);

                                x += image.Width + SubImageAsset.Padding;
                            }
                        }
                        catch (Exception uex)
                        {
                            ErrorHandler.Record(uex);

                            MessageBox.Show(filename + "파일을 로드할 수 없습니다.");
                        }
                    }

                    scratchTarget.Dispose();

                    _Asset.Content.Bitmap = scratch;
                }
            }
        }


        private void CropButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                using (var form = new ImageCropForm())
                {
                    form.Asset = _Asset;

                    form.ShowDialog(this);
                }
            }
        }

        private void PackButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null && MessageBox.Show(this, "이미지를 패킹하시겠습니까?", "패킹", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                float rate = _Asset.Pack();

                MessageBox.Show(this, string.Format("이미지를 패킹이 완료되었습니다.\n이전 대비 {0}%로 변경", rate * 100));
            }
        }

        private void ReplaceButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                using (var form = new ImageReplaceForm())
                {
                    form.Asset = _Asset;

                    form.ShowDialog(this);
                }
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                if (_Asset.SubImages.Length == 0)
                {
                    if (MessageBox.Show(this, "이미지를 삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        _Asset.Content.Bitmap = null;
                    }
                }
                else
                {
                    MessageBox.Show(this, "이미지를 참조하는 하위 이미지가 있습니다.");
                }
            }
        }

        private void OpaqueButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null && MessageBox.Show(this, "이미지에서 알파채널을 제거하시겠습니까?", "불투명화", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                AssetManager.Instance.Purge();

                var prevImg = _Asset.Content.Bitmap;

                if (prevImg != null)
                {
                    var nextImg = new Bitmap(prevImg);

                    var changed = false;

                    foreach (var sub in _Asset.SubImages)
                    {
                        foreach (var se in sub.Elements)
                        {
                            var minx = Math.Max(se.Frame.Left, 0);
                            var miny = Math.Max(se.Frame.Top, 0);
                            var maxx = Math.Min(se.Frame.Right, nextImg.Width) - 1;
                            var maxy = Math.Min(se.Frame.Bottom, nextImg.Height) - 1;

                            for (var y = miny; y <= maxy; y++)
                            {
                                for (var x = minx; x <= maxx; x++)
                                {
                                    var color = nextImg.GetPixel(x, y);

                                    if (color.A != 255)
                                    {
                                        int r = color.R * color.A / 255;
                                        int g = color.G * color.A / 255;
                                        int b = color.B * color.A / 255;

                                        nextImg.SetPixel(x, y, Color.FromArgb(255, r, g, b));

                                        changed = true;
                                    }
                                }
                            }
                        }
                    }
                    if (changed) _Asset.Content.Bitmap = nextImg;
                    else nextImg.Dispose();
                }
            }
        }

    }
}
