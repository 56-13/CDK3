using System.ComponentModel;

namespace CDK.Assets.Components
{
    public class NumericUpDown : System.Windows.Forms.NumericUpDown
    {
        [DefaultValue(typeof(decimal), "0")]
        public new decimal Value
        {
            set
            {
                if (value >= Minimum && value <= Maximum) base.Value = value;
            }
            get => base.Value;
        }
    }
}
