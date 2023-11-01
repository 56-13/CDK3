using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class MeshSpriteElementControl : UserControl
    {
        private SpriteElementMesh _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteElementMesh Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        loopControl.DataBindings.Clear();

                        _Element.Selection.PropertyChanged -= Selection_PropertyChanged;
                    }

                    _Element = value;

                    selectionControl.Selection = _Element?.Selection;

                    if (_Element != null)
                    {
                        loopControl.DataBindings.Add("Loop", _Element, "Loop", false, DataSourceUpdateMode.OnPropertyChanged);
                        loopPanel.Visible = _Element.Selection.Animation != null;

                        _Element.Selection.PropertyChanged += Selection_PropertyChanged;
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public MeshSpriteElementControl()
        {
            InitializeComponent();

            Disposed += MeshSpriteElementControl_Disposed;
        }

        private void MeshSpriteElementControl_Disposed(object sender, EventArgs e)
        {
            if (_Element != null)
            {
                _Element.Selection.PropertyChanged -= Selection_PropertyChanged;
            }
        }

        private void Selection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Animation") loopPanel.Visible = _Element.Selection.Animation != null;
        }

        private void FitDurationButton_Click(object sender, EventArgs e)
        {
            if (_Element != null && _Element.Selection.Animation != null)
            { 
                if (MessageBox.Show(this, "타임라인의 길이를 메시 애니메이션의 길이로 맞추시겠습니까?", string.Empty, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var duration = _Element.Selection.Animation.Duration;
                    if (_Element.Selection.Loop.Count != 0) duration *= _Element.Selection.Loop.Count;

                    var timeline = _Element.Parent;
                    timeline.EndTime = timeline.StartTime + duration;
                }
            }
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = panel.Height;
        }
    }
}
