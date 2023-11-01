using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets
{
    public partial class LocaleStringExportForm : Form
    {
        public DateTime? Timestamp => timestampCheckBox.Checked ? new DateTime?(timestampPicker.Value) : null;

        public string KeyLocale
        {
            get
            {
                var selectedIndex = keyComboBox.SelectedIndex;

                return selectedIndex > 0 ? AssetManager.Instance.Config.Locales[selectedIndex - 1] : null;
            }
        }

        public string[] TargetLocales
        {
            get
            {
                var locales = new string[localeCheckedListBox.CheckedItems.Count];
                localeCheckedListBox.CheckedItems.CopyTo(locales, 0);
                return locales;
            }
        }

        private bool raiseLocaleChecked;

        public LocaleStringExportForm()
        {
            InitializeComponent();

            timestampPicker.DataBindings.Add("Enabled", timestampCheckBox, "Checked", false, DataSourceUpdateMode.Never);

            raiseLocaleChecked = true;

            if (AssetManager.IsCreated)
            {
                keyComboBox.Items.Add("(key)");
                foreach (var locale in AssetManager.Instance.Config.Locales)
                {
                    keyComboBox.Items.Add(locale);

                    localeCheckedListBox.Items.Add(locale, true);
                }
                keyComboBox.SelectedIndex = 0;
            }
        }

        private void LocaleCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (raiseLocaleChecked)
            {
                raiseLocaleChecked = false;

                var checkCount = localeCheckedListBox.CheckedItems.Count;
                
                if (e.NewValue == CheckState.Checked) checkCount++;
                else checkCount--;

                CheckState state;
                if (checkCount == 0) state = CheckState.Unchecked;
                else if (checkCount == localeCheckedListBox.Items.Count) state = CheckState.Checked;
                else state = CheckState.Indeterminate;

                localeAllCheckBox.CheckState = state;

                raiseLocaleChecked = true;
            }
        }

        private void LocaleAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (raiseLocaleChecked)
            {
                raiseLocaleChecked = false;

                if (localeAllCheckBox.CheckState != CheckState.Indeterminate)
                {
                    var check = localeAllCheckBox.CheckState == CheckState.Checked;
                    var count = localeCheckedListBox.Items.Count;
                    for (var i = 0; i < count; i++)
                    {
                        localeCheckedListBox.SetItemChecked(i, check);
                    }
                }
                raiseLocaleChecked = true;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }
    }
}
