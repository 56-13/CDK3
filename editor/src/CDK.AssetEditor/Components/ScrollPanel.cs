using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Components
{
    public class ScrollPanel : Panel
    {
        [DefaultValue(true)]
        public new bool AutoScroll
        {
            set => base.AutoScroll = value;
            get => base.AutoScroll;
        }

        public ScrollPanel()
        {
            AutoScroll = true;
        }

        protected override Point ScrollToControl(Control activeControl)
        {
            return DisplayRectangle.Location;
        }
    }
}
