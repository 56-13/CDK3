using System;
using System.Text;
using System.Numerics;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Animations.Derivations
{
    public class AnimationObjectDerivationMulti : AnimationObjectDerivation
    {
        public AnimationObjectDerivationMulti(AnimationObjectFragment parent) : base(parent)
        {

        }

        internal override bool AddAABB(ref ABoundingBox result)
        {
            var flag = false;
            foreach (AnimationObjectFragment obj in Parent.Children) flag |= obj.AddAABB(ref result);
            return flag;
        }

        internal override void AddCollider(ref Collider result)
        {
            foreach (AnimationObjectFragment obj in Parent.Children) obj.AddCollider(ref result);
        }

        internal override bool GetTransform(float progress, string name, out Matrix4x4 result)
        {
            foreach (AnimationObjectFragment obj in Parent.Children)
            {
                if (obj.GetTransform(progress + obj.Progress - Parent.Progress, name, out result)) return true;
            }
            result = Matrix4x4.Identity;
            return false;
        }

        internal override float GetDuration(DurationParam param, float duration)
        {
            var min = 0f;
            var closing = 0f;

            if (param == DurationParam.Min)
            {
                foreach (AnimationObjectFragment obj in Parent.Children)
                {
                    var cmin = obj.GetDuration(DurationParam.Min);

                    if (obj.Origin.Closing)
                    {
                        if (closing < cmin) closing = cmin;
                    }
                    else
                    {
                        if (min < cmin) min = cmin;
                    }
                }
                return min + closing;
            }
            else
            {
                var max = 0f;

                foreach (AnimationObjectFragment obj in Parent.Children)
                {
                    var cmax = obj.GetDuration(param);

                    if (obj.Origin.Closing)
                    {
                        if (closing < cmax) closing = cmax;
                    }
                    else
                    {
                        var cmin = obj.GetDuration(DurationParam.Min);

                        if (min < cmin) min = cmin;
                        if (max < cmax) max = cmax;
                    }
                }
                return Math.Max(min + closing, max);
            }
        }

        internal override void Rewind()
        {
            foreach (AnimationObjectFragment obj in Parent.Children) obj.Rewind();
        }

        internal override UpdateState Update(LightSpace lightSpace, float delta, bool alive, UpdateFlags intrans, ref UpdateFlags outtrans)
        {
            var rtn = UpdateState.Stopped;

            foreach (AnimationObjectFragment obj in Parent.Children)
            {
                switch (obj.Update(lightSpace, delta, alive, intrans, ref outtrans))
                {
                    case UpdateState.Alive:
                        rtn = UpdateState.Alive;
                        break;
                    case UpdateState.Finishing:
                        if (rtn == UpdateState.Stopped) rtn = UpdateState.Finishing;
                        break;
                }
            }

            return rtn;
        }

        internal override ShowFlags Show()
        {
            var showFlags = ShowFlags.None;
            foreach (AnimationObjectFragment obj in Parent.Children) showFlags |= obj.Show(true);
            return showFlags;
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            foreach (AnimationObjectFragment obj in Parent.Children) obj.Draw(graphics, layer, true);
        }

        internal override void LogState(StringBuilder strbuf)
        {
            foreach (AnimationObjectFragment obj in Parent.Children) obj.LogState(strbuf);
        }
    }
}
