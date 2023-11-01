using System;

namespace CDK.Assets.Components
{
    public class ListBox : System.Windows.Forms.ListBox
    {
        private bool _deferSelectionChanged;

        protected override void OnSelectedValueChanged(EventArgs e)
        {
            if (!_deferSelectionChanged)
            {
                _deferSelectionChanged = true;

                AssetManager.Instance.Invoke(() =>
                {
                    _deferSelectionChanged = false;

                    DataBindings["SelectedItem"]?.WriteValue();
                });
            }

            base.OnSelectedValueChanged(e);
        }
    }
}
