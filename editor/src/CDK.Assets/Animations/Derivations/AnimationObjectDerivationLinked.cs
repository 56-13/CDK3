using System;
using System.Numerics;
using System.Text;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Animations.Derivations
{
    public class AnimationObjectDerivationLinked : AnimationObjectDerivation
    {
        private AnimationDerivationLinked _origin;
        private int _loop;
        private int _current;
        private int _count;

        public AnimationObjectDerivationLinked(AnimationObjectFragment parent) : base(parent)
        {
            _origin = (AnimationDerivationLinked)parent.Origin.Derivation;

            _count = parent.Children.Count != 0 ? 1 : 0;
        }

        internal override bool AddAABB(ref ABoundingBox result)
        {
            var flag = false;
            var max = Math.Min(_current + _count, Parent.Children.Count);
            for (var i = _current; i < max; i++)
            {
                var obj = (AnimationObjectFragment)Parent.Children[i];
                flag |= obj.AddAABB(ref result);
            }
            return flag;
        }

        internal override void AddCollider(ref Collider result)
        {
            var max = Math.Min(_current + _count, Parent.Children.Count);
            for (var i = _current; i < max; i++)
            {
                var obj = (AnimationObjectFragment)Parent.Children[i];
                obj.AddCollider(ref result);
            }
        }

        internal override bool GetTransform(float progress, string name, out Matrix4x4 result)
        {
            var max = Math.Min(_current + _count, Parent.Children.Count);
            for (var i = _current; i < max; i++)
            {
                var obj = (AnimationObjectFragment)Parent.Children[i];
                if (obj.GetTransform(progress + obj.Progress - Parent.Progress, name, out result)) return true;
            }
            result = Matrix4x4.Identity;
            return false;
        }

        internal override float GetDuration(DurationParam param, float duration)
        {
            var rtn = 0f;

            if (param == DurationParam.Min)
            {
                foreach (AnimationObjectFragment child in Parent.Children)
                {
                    rtn += child.GetDuration(DurationParam.Min);
                }
            }
            else
            {
                var min = 0f;
                foreach (AnimationObjectFragment child in Parent.Children)
                {
                    var max = min + child.GetDuration(param);
                    if (max > rtn) rtn = max;
                    min += child.GetDuration(DurationParam.Min);
                }
            }

            if (_origin.LoopCount != 0) rtn *= _origin.LoopCount; 
            else rtn += duration;

            return rtn;
        }

        internal override void Rewind()
        {
            RewindProgress();
            _loop = 0;
        }

        private void RewindProgress()
        {
            foreach (AnimationObjectFragment obj in Parent.Children) obj.Rewind();
            _current = 0;
            _count = Parent.Children.Count != 0 ? 1 : 0;
        }

        internal override UpdateState Update(LightSpace lightSpace, float delta, bool alive, UpdateFlags intrans, ref UpdateFlags outtrans)
        {
            _count = 0;

            for (var i = _current; i < Parent.Children.Count; i++)
            {
                var obj = (AnimationObjectFragment)Parent.Children[i];

                switch (obj.Update(lightSpace, delta, alive, intrans, ref outtrans))
                {
                    case UpdateState.Stopped:
                        if (i == _current) _current++;
                        break;
                    case UpdateState.Finishing:
                        _count++;
                        break;
                    case UpdateState.Alive:
                        return UpdateState.Alive;
                }
            }
            if (_current >= Parent.Children.Count)
            {
                _loop++;

                if (_origin.LoopCount == 0)
                {
                    if (alive)
                    {
                        RewindProgress();
                        return UpdateState.None;
                    }
                }
                else if (_loop < _origin.LoopCount)
                {
                    RewindProgress();
                    return UpdateState.Alive;
                }
                return UpdateState.Stopped;
            }
            return UpdateState.Finishing;
        }

        internal override ShowFlags Show()
        {
            var showFlags = ShowFlags.None;
            var max = Math.Min(_current + _count, Parent.Children.Count);
            for (var i = _current; i < max; i++)
            {
                var obj = (AnimationObjectFragment)Parent.Children[i];
                showFlags |= obj.Show(true);
            }
            return showFlags;
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            var max = Math.Min(_current + _count, Parent.Children.Count);
            for (var i = _current; i < max; i++)
            {
                var obj = (AnimationObjectFragment)Parent.Children[i];
                obj.Draw(graphics, layer);
            }
        }

        internal override void LogState(StringBuilder strbuf)
        {
            var max = Math.Min(_current + _count, Parent.Children.Count);
            for (var i = _current; i < max; i++)
            {
                var obj = (AnimationObjectFragment)Parent.Children[i];
                obj.LogState(strbuf);
            }
        }
    }
}
