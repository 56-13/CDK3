using System.Numerics;
using System.Text;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Animations.Derivations
{
    public class AnimationObjectDerivationRandom : AnimationObjectDerivation
    {
        private AnimationDerivationRandom _origin;

        private AnimationObjectFragment _selection;

        public AnimationObjectDerivationRandom(AnimationObjectFragment parent) : base(parent)
        {
            _origin = (AnimationDerivationRandom)parent.Origin.Derivation;

            Select();
        }

        internal override bool AddAABB(ref ABoundingBox result) => _selection?.AddAABB(ref result) ?? false;
        internal override void AddCollider(ref Collider result) => _selection?.AddCollider(ref result);
        internal override bool GetTransform(float progress, string name, out Matrix4x4 result)
        {
            if (_selection != null)
            {
                return _selection.GetTransform(progress + _selection.Progress - Parent.Progress, name, out result);
            }
            result = Matrix4x4.Identity;
            return false;
        }
        
        private bool Select()
        {
            var totalWeight = 0;
            foreach (AnimationObjectFragment obj in Parent.Children)
            {
                totalWeight += obj.Origin.RandomWeight;
            }
            if (totalWeight != 0)
            {
                var randomWeight = RandomUtil.Next(0, totalWeight);
                totalWeight = 0;
                foreach (AnimationObjectFragment obj in Parent.Children)
                {
                    totalWeight += obj.Origin.RandomWeight;

                    if (randomWeight <= totalWeight)
                    {
                        _selection = obj;
                        break;
                    }
                }
                return true;
            }
            return false;
        }

        internal override float GetDuration(DurationParam param, float duration)
        {
            if (_origin.Loop) return 0;
            var rtn = 0f;
            foreach (AnimationObjectFragment obj in Parent.Children)
            {
                var d = obj.GetDuration(param);
                if (rtn < d) rtn = d;
            }
            rtn += duration;
            return rtn;
        }

        internal override void Rewind()
        {
            foreach (AnimationObjectFragment obj in Parent.Children) obj.Rewind();

            Select();
        }

        internal override UpdateState Update(LightSpace lightSpace, float delta, bool alive, UpdateFlags intrans, ref UpdateFlags outtrans)
        {
            if (_selection != null)
            {
                var state = _selection.Update(lightSpace, delta, alive, intrans, ref outtrans);

                if (state == UpdateState.Stopped)
                {
                    if (alive && _origin.Loop && Select())
                    {
                        _selection.Rewind();
                        return UpdateState.None;
                    }
                    _selection = null;
                }
                else return _origin.Loop ? UpdateState.None : state;
            }
            return UpdateState.Stopped;
        }

        internal override ShowFlags Show() => _selection?.Show(true) ?? ShowFlags.None;
        internal override void Draw(Graphics graphics, InstanceLayer layer) => _selection?.Draw(graphics, layer);
        internal override void LogState(StringBuilder strbuf) => _selection?.LogState(strbuf);
    }
}

