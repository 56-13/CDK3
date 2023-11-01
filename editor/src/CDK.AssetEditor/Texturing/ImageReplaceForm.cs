using System;
using System.Numerics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Support;

using GDIRectangle = System.Drawing.Rectangle;

namespace CDK.Assets.Texturing
{
    partial class ImageReplaceForm : Form
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
                    if (_Asset != null) _Asset.PropertyChanged -= Asset_PropertyChanged;

                    _Asset = value;

                    if (_Asset != null) _Asset.PropertyChanged += Asset_PropertyChanged;

                    ResetSubImages();
                }
            }
            get => _Asset;
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SubImages")) ResetSubImages();
        }

        private void ResetSubImages()
        {
            if (_Asset != null)
            {
                var checkedItems = new ListViewItem[srcListView.CheckedItems.Count];
                srcListView.CheckedItems.CopyTo(checkedItems, 0);

                srcListView.Items.Clear();

                foreach (var sub in Asset.SubImages)
                {
                    var check = false;

                    foreach (var checkedItem in checkedItems)
                    {
                        if (sub == checkedItem.Tag)
                        {
                            check = true;
                            break;
                        }
                    }
                    var item = new ListViewItem
                    {
                        Tag = sub,
                        Text = sub.TagName,
                        Checked = check
                    };
                    srcListView.Items.Add(item);
                }
            }
            else
            {
                srcListView.Items.Clear();
            }
        }

        private bool _raiseSrcListViewSelectedIndexChanged;

        public ImageReplaceForm()
        {
            InitializeComponent();

            srcColumnHeader.Width = srcListView.Width - 4 - SystemInformation.VerticalScrollBarWidth;

            openFileDialog.Filter = FileFilters.Image;

            _raiseSrcListViewSelectedIndexChanged = true;

            Disposed += ImageReplaceForm_Disposed;
        }

        private void ImageReplaceForm_Disposed(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                _Asset.PropertyChanged -= Asset_PropertyChanged;
            }
        }

        private void UpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (destListBox.SelectedIndices.Count != 0)
            {
                var indices = new int[destListBox.SelectedIndices.Count];
                destListBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);

                if (indices[0] > 0)
                {
                    destListBox.SelectedIndices.Clear();

                    foreach (var index in indices)
                    {
                        var item = (string)destListBox.Items[index];
                        destListBox.Items.RemoveAt(index);
                        destListBox.Items.Insert(index - 1, item);

                        destListBox.SelectedIndices.Add(index - 1);
                    }
                }
            }
        }

        private void DownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (destListBox.SelectedIndices.Count != 0)
            {
                var indices = new int[destListBox.SelectedIndices.Count];
                destListBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);

                if (indices[indices.Length - 1] < destListBox.Items.Count - 1)
                {
                    destListBox.SelectedIndices.Clear();

                    for (var i = indices.Length - 1; i >= 0; i--)
                    {
                        var index = indices[i];

                        var item = (string)destListBox.Items[index];
                        destListBox.Items.RemoveAt(index);
                        destListBox.Items.Insert(index + 1, item);

                        destListBox.SelectedIndices.Add(index + 1);
                    }
                }
            }
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var paths = openFileDialog.FileNames;

                destListBox.Items.AddRange(paths);
            }
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (destListBox.SelectedIndices.Count != 0)
            {
                var indices = new int[destListBox.SelectedIndices.Count];
                destListBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);

                for (var i = indices.Length - 1; i >= 0; i--)
                {
                    var index = indices[i];

                    destListBox.Items.RemoveAt(index);
                }
            }
        }

        private void SrcListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_raiseSrcListViewSelectedIndexChanged)
            {
                _raiseSrcListViewSelectedIndexChanged = false;

                if (!IsHandleCreated) CreateHandle();

                BeginInvoke((Action)(() =>
                {
                    var index = srcListView.SelectedIndices[0];

                    screenControl.Source = (ImageAsset)srcListView.Items[index].Tag;

                    var checkedIndex = srcListView.CheckedIndices.IndexOf(index);

                    destListBox.SelectedIndices.Clear();

                    if (checkedIndex >= 0 && checkedIndex < destListBox.Items.Count)
                    {
                        destListBox.SelectedIndices.Add(checkedIndex);
                    }

                    _raiseSrcListViewSelectedIndexChanged = true;
                }));
            }
        }

        private void DestListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dest = (string)destListBox.SelectedItem;

            if (dest != null)
            {
                try
                {
                    screenControl.Destination = new Bitmap(new MemoryStream(File.ReadAllBytes(dest)));
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    screenControl.Destination = null;

                    MessageBox.Show(this, "이미지를 불러올 수 없습니다.");
                }
            }
            else
            {
                screenControl.Destination = null;
            }
        }
        private void ReplaceButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                var count = srcListView.CheckedItems.Count;

                if (count != 0)
                {
                    if (destListBox.Items.Count == count)
                    {
                        AssetManager.Instance.Purge();

                        _Asset.PropertyChanged -= Asset_PropertyChanged;

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

                        for (var i = 0; i < count; i++)
                        {
                            var sub = (SubImageAsset)srcListView.CheckedItems[i].Tag;

                            var se = sub.MainElement;

                            if (se == null) continue;

                            var path = (string)destListBox.Items[i];

                            Bitmap image = null;

                            try
                            {
                                image = BitmapTexture.Load(path);
                            }
                            catch (Exception uex)
                            {
                                ErrorHandler.Record(uex);

                                MessageBox.Show(this, "이미지를 불러올 수 없습니다.");

                                continue;
                            }

                            if (scratch != null && image.Width <= se.Frame.Width && image.Height <= se.Frame.Height)
                            {
                                scratchTarget.Clear(se.Frame);
                                se.Frame = new GDIRectangle(se.Frame.X, se.Frame.Y, image.Width, image.Height);
                                scratchTarget.Copy(image, se.Frame.X, se.Frame.Y);
                            }
                            else
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

                                se.Frame = new GDIRectangle(x, y, image.Width, image.Height);
                                se.Pivot = Vector2.Zero;

                                x += image.Width + SubImageAsset.Padding;
                            }

                            image.Dispose();
                        }

                        scratchTarget?.Dispose();

                        _Asset.Content.Bitmap = scratch;

                        _Asset.PropertyChanged += Asset_PropertyChanged;

                        foreach (ListViewItem srcItem in srcListView.Items) srcItem.Checked = false;

                        ResetSubImages();

                        destListBox.Items.Clear();

                        MessageBox.Show(this, "이미지가 교체되었습니다.");
                    }
                    else
                    {
                        MessageBox.Show(this, "교체하는 이미지의 개수가 맞지 않습니다.");
                    }
                }
            }
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        private void SrcListView_SizeChanged(object sender, EventArgs e)
        {
            srcColumnHeader.Width = srcListView.Width - 4 - SystemInformation.VerticalScrollBarWidth;
        }
    }
}
