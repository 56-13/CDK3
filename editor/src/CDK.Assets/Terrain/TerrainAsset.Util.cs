using CDK.Drawing;

namespace CDK.Assets.Terrain
{
    partial class TerrainAsset
    {
        
        private static readonly float AttenuationMinEdge = 0.5f;
        private static readonly float AttenuationMaxEdgeRate = 0.5f;

        public static float AttenuateRate(float distance, float limit, float attenuation)
        {
            if (distance >= limit) return 0;

            var t = limit - distance;
            var centerv = MathUtil.SmoothStep(t / limit);

            if (attenuation < 0.5f)
            {
                var maxv = t < AttenuationMinEdge ? MathUtil.SmoothStep(t / AttenuationMinEdge) : 1.0f;

                return MathUtil.Lerp(maxv, centerv, attenuation * 2.0f);
            }
            else if (attenuation > 0.5f)
            {
                var maxe = limit * AttenuationMaxEdgeRate;

                var minv = t > limit - maxe ? MathUtil.SmoothStep((t + maxe - limit) / maxe) : 0.0f;

                return MathUtil.Lerp(centerv, minv, attenuation * 2.0f - 1.0f);
            }
            else
            {
                return centerv;
            }
        }
    }
}
