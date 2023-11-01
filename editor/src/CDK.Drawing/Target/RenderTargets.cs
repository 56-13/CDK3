
namespace CDK.Drawing
{
    public static class RenderTargets
    {
        public static RenderTarget New(object key, int life, bool recycle, RenderTargetDescription desc)
        {
            desc.Validate();

            if (ResourcePool.Instance.Recycle(((IResource candidate) => candidate is RenderTarget target && target.Description == desc), key, life, out var resource))
            {
                return (RenderTarget)resource;
            }
            var newTarget = new RenderTarget(desc);
            ResourcePool.Instance.Add(key, newTarget, life, recycle);
            return newTarget;
        }

        public static RenderTarget NewTemporary(in RenderTargetDescription desc) => New(null, 1, true, desc);
    }
}
