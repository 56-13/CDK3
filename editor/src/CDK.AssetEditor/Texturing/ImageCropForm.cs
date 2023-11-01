using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Texturing
{
    partial class ImageCropForm : Form
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
                var checkedItems = new ListViewItem[listView.CheckedItems.Count];
                listView.CheckedItems.CopyTo(checkedItems, 0);

                listView.Items.Clear();

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
                    listView.Items.Add(item);
                }
            }
            else listView.Items.Clear();
        }

        private bool _raiseListViewItemChecked;

        public ImageCropForm()
        {
            InitializeComponent();

            _raiseListViewItemChecked = true;

            Disposed += ImageCropForm_Disposed;
        }

        private void ImageCropForm_Disposed(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                _Asset.PropertyChanged -= Asset_PropertyChanged;
            }
        }

        private IEnumerable<SubImageAsset> GetSelectedImages()
        {
            var items = new ListViewItem[listView.CheckedItems.Count];
            listView.CheckedItems.CopyTo(items, 0);
            return items.Select(i => (SubImageAsset)i.Tag);
        }

        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var image = listView.SelectedItems.Count != 0 ? (SubImageAsset)listView.SelectedItems[0].Tag : null;

            screenControl.Image = image;
        }

        private void ListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_raiseListViewItemChecked)
            {
                var count = listView.CheckedItems.Count;
                if (count == 0) selectAllCheckBox.CheckState = CheckState.Unchecked;
                else if (count == listView.Items.Count) selectAllCheckBox.CheckState = CheckState.Checked;
                else selectAllCheckBox.CheckState = CheckState.Indeterminate;
            }
        }

        private void SelectAllCheckBox_Click(object sender, EventArgs e)
        {
            _raiseListViewItemChecked = false;

            if (selectAllCheckBox.CheckState != CheckState.Checked)
            {
                foreach (ListViewItem item in listView.Items) item.Checked = true;
                selectAllCheckBox.CheckState = CheckState.Checked;
            }
            else
            {
                foreach (ListViewItem item in listView.Items) item.Checked = false;
                selectAllCheckBox.CheckState = CheckState.Unchecked;
            }

            _raiseListViewItemChecked = true;
        }

        private void CropButton_Click(object sender, EventArgs e)
        {
            foreach (var image in GetSelectedImages())
            {
                foreach (var element in image.Elements)
                {
                    if (image.RootImage.Crop(element, out var frame, out var pivot))
                    {
                        element.Frame = frame;
                        element.Pivot = pivot;
                    }
                }
            }
            screenControl.Invalidate();

            MessageBox.Show(this, "이미지가 축소되었습니다.");
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        private void MovePivot(int x, int y)
        {
            foreach (var image in GetSelectedImages())
            {
                foreach (var element in image.Elements)
                {
                    var pivot = element.Pivot;
                    pivot.X += x;
                    pivot.Y += y;
                    element.Pivot = pivot;
                }
            }
            screenControl.Invalidate();
        }

        private void PivotLeftButton_Click(object sender, EventArgs e) => MovePivot(-1, 0);
        private void PivotRightButton_Click(object sender, EventArgs e) => MovePivot(1, 0);
        private void PivotUpButton_Click(object sender, EventArgs e) => MovePivot(0, -1);
        private void PivotDownButton_Click(object sender, EventArgs e) => MovePivot(0, 1);

        private void ListView_SizeChanged(object sender, EventArgs e)
        {
            columnHeader.Width = listView.Width - 4 - SystemInformation.VerticalScrollBarWidth;
        }
    }
}
