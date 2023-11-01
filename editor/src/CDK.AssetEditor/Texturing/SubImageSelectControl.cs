using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Texturing
{
    partial class SubImageSelectControl : UserControl
    {
        private RootImageAsset _RootImage;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RootImageAsset RootImage
        {
            set
            {
                if (_RootImage != value)
                {
                    if (_RootImage != null)
                    {
                        listView.Items.Clear();

                        _RootImage.PropertyChanged -= RootImage_PropertyChanged;
                    }

                    _RootImage = value;

                    if (_RootImage != null)
                    {
                        ResetSubImages();

                        _RootImage.PropertyChanged += RootImage_PropertyChanged;
                    }
                    else
                    {
                        SelectedImages = null;
                    }

                    RootImageChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _RootImage;
        }

        public event EventHandler RootImageChanged;

        private bool _checkChanging;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SubImageAsset[] SelectedImages
        {
            set
            {
                _checkChanging = true;

                var changed = false;

                value = value?.Where(image => image.RootImage == _RootImage).ToArray();

                if (value != null && value.Length != 0)
                {
                    foreach (ListViewItem item in listView.Items) 
                    { 
                        var check = value.Contains((SubImageAsset)item.Tag);

                        if (item.Checked != check)
                        {
                            item.Checked = check;
                            changed = true;
                        }
                    }
                }
                else
                {
                    foreach (ListViewItem item in listView.Items)
                    {
                        if (item.Checked)
                        {
                            item.Checked = false;
                            changed = true;
                        }
                    }
                }

                _checkChanging = false;

                if (changed) SelectedImagesChanged?.Invoke(this, EventArgs.Empty);
            }
            get
            {
                var indices = new int[listView.CheckedIndices.Count];
                for (var i = 0; i < indices.Length; i++) indices[i] = listView.CheckedIndices[i];
                Array.Sort(indices);
                var images = indices.Select(i => (SubImageAsset)listView.Items[i].Tag).ToArray();
                return images;
            }
        }

        public event EventHandler SelectedImagesChanged;

        public SubImageSelectControl()
        {
            InitializeComponent();

            columnHeader.Width = listView.Width - 4 - SystemInformation.VerticalScrollBarWidth;

            Disposed += SubImageSelectControl_Disposed;
        }

        private void SubImageSelectControl_Disposed(object sender, EventArgs e)
        {
            if (_RootImage != null)
            {
                _RootImage.PropertyChanged -= RootImage_PropertyChanged;
            }
        }

        private void RootImage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SubImages") ResetSubImages();
        }

        private void ResetSubImages()
        {
            var selections = SelectedImages;

            _checkChanging = true;

            listView.Items.Clear();
            foreach (var sub in _RootImage.SubImages)
            {
                var check = selections.Contains(sub);

                var item = new ListViewItem
                {
                    Tag = sub,
                    Text = sub.TagName,
                    Checked = check
                };
                listView.Items.Add(item);
            }

            _checkChanging = false;

            var nextSelections = SelectedImages;

            if (!selections.SequenceEqual(nextSelections)) SelectedImagesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            screenControl.Image = listView.SelectedItems.Count > 0 ? (SubImageAsset)listView.SelectedItems[listView.SelectedItems.Count - 1].Tag : null;
        }

        private void ListView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!_checkChanging)
            {
                //한번에 다중선택시 이벤트가 너무 빈번히 발생, ItemCheck은 아직 변화된 상태가 아님
                //ItemChecked 는 코드에서 변화되었을 때도 발생함.

                _checkChanging = true;
                BeginInvoke((Action)(() =>      
                {
                    SelectedImagesChanged?.Invoke(this, EventArgs.Empty);
                    _checkChanging = false;
                }));
            }
        }

        private void ListView_SizeChanged(object sender, EventArgs e)
        {
            columnHeader.Width = listView.Width - 4 - SystemInformation.VerticalScrollBarWidth;
        }
    }
}
