using System.Linq;
using System.Numerics;
using System.Text;
using System.Collections.Generic;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Animations.Derivations
{
    public class AnimationObjectDerivationEmission : AnimationObjectDerivation
    {
        private AnimationDerivationEmission _origin;
        private List<AnimationObjectFragment> _instances;
        private int _index;
        private float _counter;

        public AnimationObjectDerivationEmission(AnimationObjectFragment parent) : base(parent)
        {
            _origin = (AnimationDerivationEmission)parent.Origin.Derivation;

            if (_origin.Prewarm) _counter = _origin.EmissionCount * _origin.EmissionDelay;

            _instances = new List<AnimationObjectFragment>();
        }

        internal override bool AddAABB(ref ABoundingBox result)
        {
            var flag = false;
            foreach (var obj in _instances) flag |= obj.AddAABB(ref result);
            return flag;
        }

        internal override void AddCollider(ref Collider result) { }

        internal override bool GetTransform(float progress, string name, out Matrix4x4 result)
        {
            result = Matrix4x4.Identity;
            return false;
        }

        internal override float GetDuration(DurationParam type, float duration)
        {
            var rtn = 0f;
            foreach (var obj in _instances)
            {
                var d = obj.GetDuration(type);
                if (rtn < d) rtn = d;
            }
            rtn += duration;
            return rtn;
        }
        internal override void Rewind()
        {
            _instances.Clear();
            _index = 0;
            _counter = _origin.Prewarm ? _origin.EmissionCount * _origin.EmissionDelay : 0;
        }

        internal override UpdateState Update(LightSpace lightSpace, float delta, bool alive, UpdateFlags intrans, ref UpdateFlags outtrans)
        {
            var i = 0;
            while (i < _instances.Count())
            {
                AnimationObjectFragment emission = _instances[i];

                if (emission.Update(lightSpace, delta, alive, intrans, ref outtrans) != UpdateState.Stopped) i++;
                else _instances.RemoveAt(i);
            }

            if (Parent.Children.Count != 0 && alive)
            {
                _counter += delta;

                while (_counter >= _origin.EmissionDelay)
                {
                    _counter -= _origin.EmissionDelay;

                    if ((_origin.EmissionCount == 0 || _instances.Count < _origin.EmissionCount) && Parent.GetTransform(Parent.Progress - _counter, null, out var transform))
                    {
                        if (_index >= Parent.Children.Count) _index = 0;

                        var emission = new AnimationObjectFragment(((AnimationObjectFragment)Parent.Children[_index]).Origin)
                        {
                            Parent = Parent,
                            PostTransform = transform
                        };

                        _instances.Add(emission);

                        emission.Update(lightSpace, _counter, alive, intrans, ref outtrans);
                        _index++;
                    }
                }
            }

            if (alive) return UpdateState.Alive;
            if (_instances.Count != 0) return UpdateState.Finishing;
            return UpdateState.Stopped;
        }

        internal override ShowFlags Show()
        {
            var showFlags = ShowFlags.None;
            foreach (var obj in _instances) showFlags |= obj.Show(true);
            return showFlags;
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            foreach (var instance in _instances) instance.Draw(graphics, layer, true);
        }

        internal override void LogState(StringBuilder strbuf)
        {
            AssetElement p = Parent;
            while (p is AnimationObjectFragment parent)
            {
                strbuf.Append(' ');
                p = parent.Parent;
            }
            strbuf.Append("emission count:");
            strbuf.Append(_instances.Count);
            strbuf.Append('\n');
        }
    }
}
