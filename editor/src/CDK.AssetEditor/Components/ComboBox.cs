using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Components
{
    public class ComboBox : System.Windows.Forms.ComboBox
    {
        [DefaultValue(ComboBoxStyle.DropDownList)]
        public new ComboBoxStyle DropDownStyle
        {
            set => base.DropDownStyle = value;
            get => base.DropDownStyle;
        }

        public ComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!DroppedDown) ((HandledMouseEventArgs)e).Handled = true;
        }

        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            DataBindings["SelectedItem"]?.WriteValue();

            base.OnSelectionChangeCommitted(e);
        }
    }
}
