using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Meshing
{
    public partial class MeshAnimationControl : UserControl
    {
        private MeshAnimationComponent _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MeshAnimationComponent Object
        {
            set
            {
                if (_Object != value)
                {
                    _Object = value;

                    if (_Object != null)
                    {
                        var duration = (int)(_Object.Animation.Duration * 1000);

                        var min = duration / 60000;
                        var sec = (duration / 1000) % 60;
                        var msec = duration % 1000;

                        var text = $"{min:D2}:{sec:D2}:{msec:D3}";

                        durationTextBox.Text = text;
                    }

                    ResetPlay();

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }
        public event EventHandler ObjectChanged;

        private bool _playChaging;

        public MeshAnimationControl()
        {
            InitializeComponent();
        }

        private void ResetPlay()
        {
            _playChaging = true;

            var selected = false;

            if (_Object != null)
            {
                var mesh = _Object.GetAncestor<MeshObject>(false);

                selected = mesh != null && mesh.Selection.Animation == _Object.Animation;
            }

            playCheckBox.Checked = selected;

            _playChaging = false;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible) ResetPlay();
        }

        private void PlayCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_playChaging) return;

            if (_Object != null)
            {
                var mesh = _Object.GetAncestor<MeshObject>(false);

                if (mesh != null) mesh.Selection.Animation = playCheckBox.Checked ? _Object.Animation : null;
            }
        }
    }
}
