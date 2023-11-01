using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Sprite
{
    public partial class SpriteTimelineControl : UserControl
    {
        private SpriteObject _Sprite;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteObject Sprite
        {
            set
            {
                if (_Sprite != value)
                {
                    _Sprite = value;

                    screenControl.Sprite = value;

                    Invalidate();

                    SpriteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Sprite;
        }
        public event EventHandler SpriteChanged;

        public SpriteTimelineControl()
        {
            InitializeComponent();
        }
    }
}
