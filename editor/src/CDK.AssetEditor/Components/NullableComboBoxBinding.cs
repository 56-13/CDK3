using System.Windows.Forms;

namespace CDK.Assets.Components
{
    public class NullableComboBoxBinding : Binding
    {
        public NullableComboBoxBinding(object dataSource, string dataMember) : 
            base("SelectedIndex", dataSource, dataMember, true, DataSourceUpdateMode.OnPropertyChanged)
        {
            
        }

        protected override void OnFormat(ConvertEventArgs e)
        {
            e.Value = e.Value != null ? ((ComboBox)Control).Items.IndexOf(e.Value) : -1;
        }

        protected override void OnParse(ConvertEventArgs e)
        {
            int i = (int)e.Value;

            e.Value = i >= 0 ? ((ComboBox)Control).Items[i] : null;
        }
    }
}
