using System;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Assets.Components;
using CDK.Assets.Scenes;

namespace CDK.Assets.Animations
{
    public class AnimationFragmentControl : Control, ICollapsibleControl, ISplittableControl
    {
        private AnimationObjectFragment _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationObjectFragment Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        _Object.PropertyChanged -= Object_PropertyChanged;
                    }

                    _Object = value;

                    _viewControl.Object = value;
                    _substanceViewControl.Object = value;

                    if (_Object != null)
                    {
                        _Object.PropertyChanged += Object_PropertyChanged;
                    }

                    ResetView();

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        private AnimationFragmentViewControl _viewControl;
        private AnimationSubstanceViewControl _substanceViewControl;
        
        public AnimationFragmentControl()
        {
            _viewControl = new AnimationFragmentViewControl();
            _substanceViewControl = new AnimationSubstanceViewControl();

            _viewControl.Anchor = _substanceViewControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            _viewControl.SizeChanged += SubControl_SizeChanged;
            _substanceViewControl.SizeChanged += SubControl_SizeChanged;

            Disposed += AnimationControl_Disposed;
        }

        private void AnimationControl_Disposed(object sender, EventArgs e)
        {
            _viewControl.Dispose();
            _substanceViewControl.Dispose();

            if (_Object != null)
            {
                _Object.PropertyChanged -= Object_PropertyChanged;
            }
        }

        private void SubControl_SizeChanged(object sender, EventArgs e)
        {
            var control = (Control)sender;

            if (control.Parent != null)
            {
                var action = (Action)(() => { Height = control.Height; });
                //서브컨트롤 크기변화로 인해 높이가 변화된 후 스크롤바가 생겼다면 서브컨트롤이 스크롤바의 너비에 맞게 조정되지 않음
                //스크롤바에 추가되는 컨트롤에만 이 코드 추가

                if (IsHandleCreated) BeginInvoke(action);
                else action.Invoke();
            }
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SubstanceView":
                    ResetView();

                    SceneControl.ValidateBottomControl(this);
                    break;
            }
        }

        private void ResetView()
        {
            if (_Object != null)
            {
                var subControl = _Object.SubstanceView ? (Control)_substanceViewControl : _viewControl;

                if (subControl.Parent == null)
                {
                    SuspendLayout();
                    Controls.Clear();
                    Height = subControl.Height;
                    subControl.Width = Width;
                    Controls.Add(subControl);
                    ResumeLayout();
                }
            }
            else Controls.Clear();
        }

        public bool Splitted
        {
            set
            {
                _viewControl.Splitted = value;
                _substanceViewControl.Splitted = value;
            }
            get => _viewControl.Splitted;
        }

        public void CollapseAll()
        {
            if (_Object != null)
            {
                var subControl = _Object.SubstanceView ? (ICollapsibleControl)_substanceViewControl : _viewControl;

                subControl.CollapseAll();
            }
        }

        public void CollapseDefault()
        {
            if (_Object != null)
            {
                var subControl = _Object.SubstanceView ? (ICollapsibleControl)_substanceViewControl : _viewControl;

                subControl.CollapseDefault();
            }
        }
    }
}
