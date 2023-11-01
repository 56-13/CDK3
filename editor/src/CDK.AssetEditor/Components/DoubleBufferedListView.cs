using System.Windows.Forms;

namespace CDK.Assets.Components
{
    public class DoubleBufferedListView : ListView
    {
        public DoubleBufferedListView()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
        }
    }
}
