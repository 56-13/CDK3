using System.Numerics;
using System.Text;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Animations
{
    public abstract class AnimationObjectDerivation : NotifyPropertyChanged
    {
        public AnimationObjectFragment Parent { private set; get; }
        protected AnimationObjectDerivation(AnimationObjectFragment parent)
        {
            Parent = parent;
        }

        internal abstract bool AddAABB(ref ABoundingBox result);
        internal abstract void AddCollider(ref Collider result);
        internal abstract bool GetTransform(float progress, string name, out Matrix4x4 result);
        internal abstract float GetDuration(DurationParam param, float duration);
        internal abstract void Rewind();
        internal abstract UpdateState Update(LightSpace lightSpace, float delta, bool alive, UpdateFlags inflags, ref UpdateFlags outflags);
        internal abstract ShowFlags Show();
        internal abstract void Draw(Graphics graphics, InstanceLayer layer);
        internal abstract void LogState(StringBuilder strbuf);
    }
}
