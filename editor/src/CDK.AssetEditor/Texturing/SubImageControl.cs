using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using CDK.Assets.Support;

namespace CDK.Assets.Texturing
{
    partial class SubImageControl : UserControl
    {
        private SubImageAsset _Asset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SubImageAsset Asset
        {
            set
            {
                if (_Asset != value)
                {
                    if (_Asset != null)
                    {
                        screenControl.DataBindings.Clear();

                        nameTextBox.DataBindings.Clear();

                        elementListBox.DataBindings.Clear();
                        elementListBox.DataSource = null;

                        borderXCheckBox.DataBindings.Clear();
                        borderYCheckBox.DataBindings.Clear();

                        _Asset.PropertyChanged -= Asset_PropertyChanged;
                    }

                    _Asset = value;

                    if (_Asset != null)
                    {
                        screenControl.DataBindings.Add("Element", _Asset, "SelectedElement", true, DataSourceUpdateMode.Never);

                        nameTextBox.DataBindings.Add("Text", _Asset, "TagName", false, DataSourceUpdateMode.Never);

                        elementListBox.DataSource = _Asset.Elements;
                        elementListBox.DisplayMember = "Name";
                        elementListBox.DataBindings.Add("SelectedItem", _Asset, "SelectedElement", true, DataSourceUpdateMode.OnPropertyChanged);

                        borderXCheckBox.DataBindings.Add("Checked", _Asset, "BorderX", false, DataSourceUpdateMode.OnPropertyChanged);
                        borderYCheckBox.DataBindings.Add("Checked", _Asset, "BorderY", false, DataSourceUpdateMode.OnPropertyChanged);

                        _Asset.PropertyChanged += Asset_PropertyChanged;
                    }

                    ResetSelectedElement();

                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }

        public event EventHandler AssetChanged;

        private bool _raiseLocaleCheckChanged;

        private void ResetSelectedElement()
        {
            _raiseLocaleCheckChanged = false;

            frameControl.DataBindings.Clear();
            scaledWidthUpDown.DataBindings.Clear();
            scaledHeightUpDown.DataBindings.Clear();
            pivotControl.DataBindings.Clear();

            var element = _Asset?.SelectedElement;

            if (element != null)
            {
                for (var i = 0; i < localeCheckedListBox.Items.Count; i++)
                {
                    var locale = (string)localeCheckedListBox.Items[i];

                    var check = element.Locales != null && Array.IndexOf(element.Locales, locale) >= 0;

                    localeCheckedListBox.SetItemChecked(i, check);
                }
                localeCheckedListBox.Enabled = true;

                frameControl.DataBindings.Add("ValueGDI", element, "Frame", false, DataSourceUpdateMode.OnPropertyChanged);

                var scaledWidthUpDownBinding = new Binding("Value", element, "Frame", true, DataSourceUpdateMode.Never);
                scaledWidthUpDownBinding.Format += (s, e) =>
                {
                    e.Value = (decimal)(((Rectangle)e.Value).Width * _Asset.ContentScale);
                };
                scaledWidthUpDown.DataBindings.Add(scaledWidthUpDownBinding);

                var scaledHeightUpDownBinding = new Binding("Value", element, "Frame", true, DataSourceUpdateMode.Never);
                scaledHeightUpDownBinding.Format += (s, e) =>
                {
                    e.Value = (decimal)(((Rectangle)e.Value).Height * _Asset.ContentScale);
                };
                scaledHeightUpDown.DataBindings.Add(scaledHeightUpDownBinding);

                pivotControl.DataBindings.Add("Value", element, "Pivot", false, DataSourceUpdateMode.OnPropertyChanged);
            }
            else
            {
                for (var i = 0; i < localeCheckedListBox.Items.Count; i++)
                {
                    localeCheckedListBox.SetItemChecked(i, false);
                }

                localeCheckedListBox.Enabled = false;
            }

            _raiseLocaleCheckChanged = true;
        }

        public SubImageControl()
        {
            InitializeComponent();

            openFileDialog.Filter = FileFilters.Image;

            _raiseLocaleCheckChanged = true;

            if (AssetManager.IsCreated)
            {
                foreach (string locale in AssetManager.Instance.Config.Locales)
                {
                    localeCheckedListBox.Items.Add(locale, false);
                }
            }

            Disposed += ImageSubControl_Disposed;
        }

        private void ImageSubControl_Disposed(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                _Asset.PropertyChanged -= Asset_PropertyChanged;
            }
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedElement":
                    ResetSelectedElement();
                    break;
                case "ContentScale":
                    if (_Asset.SelectedElement != null)
                    {
                        scaledWidthUpDown.DataBindings["Value"].ReadValue();
                        scaledHeightUpDown.DataBindings["Value"].ReadValue();
                    }
                    break;
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null && folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var dirpath = folderBrowserDialog.SelectedPath;

                    _Asset.Export(dirpath);

                    MessageBox.Show(this, "내보내기가 완료되었습니다.");
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "내보내기를 진행할 수 없습니다.");
                }
            }
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null && _Asset.SelectedElement != null && openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _Asset.SelectedElement.Import(openFileDialog.FileName);
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(openFileDialog.FileName + "파일을 로드할 수 없습니다.");
                }
            }
        }

        private void ResizeButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null && _Asset.SelectedElement != null)
            {
                using (var form = new ImageResizeForm())
                {
                    form.Element = _Asset.SelectedElement;

                    form.ShowDialog(this);
                }
            }
        }

        private void OpaqueButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                AssetManager.Instance.Purge();

                var prevImg = _Asset.RootImage.Content.Bitmap;

                if (prevImg != null)
                {
                    var nextImg = new Bitmap(prevImg);

                    var occured = false;

                    foreach (var element in _Asset.Elements)
                    {
                        var minx = Math.Max(element.Frame.Left, 0);
                        var miny = Math.Max(element.Frame.Top, 0);
                        var maxx = Math.Min(element.Frame.Right, nextImg.Width) - 1;
                        var maxy = Math.Min(element.Frame.Bottom, nextImg.Height) - 1;

                        for (var y = miny; y <= maxy; y++)
                        {
                            for (var x = minx; x <= maxx; x++)
                            {
                                var color = nextImg.GetPixel(x, y);

                                if (color.A != 255)
                                {
                                    var r = color.R * color.A / 255;
                                    var g = color.G * color.A / 255;
                                    var b = color.B * color.A / 255;
                                    
                                    nextImg.SetPixel(x, y, Color.FromArgb(255, r, g, b));

                                    occured = true;
                                }
                            }
                        }
                    }
                    if (occured)
                    {
                        _Asset.RootImage.Content.Bitmap = nextImg;
                    }
                    else
                    {
                        nextImg.Dispose();
                    }
                }
            }
        }

        private void ElementAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                _Asset.Elements.Add(new SubImageElement(_Asset));
            }
        }

        private void ElementRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                var index = elementListBox.SelectedIndex;

                if (index >= 0)
                {
                    Asset.Elements.RemoveAt(index);
                }
            }
        }

        private void ElementUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                var index = elementListBox.SelectedIndex;

                if (index > 0)
                {
                    Asset.Elements.Move(index, index - 1);
                }
            }
        }

        private void ElementDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                var index = elementListBox.SelectedIndex;

                if (index >= 0 && index < Asset.Elements.Count - 1)
                {
                    Asset.Elements.Move(index, index + 1);
                }
            }
        }

        private void LocaleCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_raiseLocaleCheckChanged && _Asset != null)
            {
                var element = _Asset.SelectedElement;

                if (element != null)
                {
                    var locales = new List<string>();

                    for (var i = 0; i < localeCheckedListBox.Items.Count; i++)
                    {
                        bool check;

                        if (i == e.Index)
                        {
                            check = e.NewValue == CheckState.Checked;
                        }
                        else
                        {
                            check = localeCheckedListBox.GetItemChecked(i);
                        }
                        if (check)
                        {
                            var locale = (string)localeCheckedListBox.Items[i];
                            
                            locales.Add(locale);
                        }
                    }

                    element.Locales = locales.Count != 0 ? locales.ToArray() : null;
                }
            }
        }
    }
}
