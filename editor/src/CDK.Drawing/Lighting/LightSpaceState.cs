
namespace CDK.Drawing
{
    public class LightSpaceState
    {
        public LightMode Mode { internal set; get; }
        public bool UsingDirectionalLight { internal set; get; }
        public bool UsingPointLight { internal set; get; }
        public bool UsingSpotLight { internal set; get; }
        public bool UsingShadow { internal set; get; }
        public Buffer LightBuffer { internal set; get; }

        public Buffer DirectionalLightBuffer { internal set; get; }

        private Texture[] _DirectionalShadowMaps = new Texture[LightSpace.MaxDirectionalLightCount];
        public Texture GetDirectionalShadowMap(int i) => _DirectionalShadowMaps[i];
        internal void SetDirectionalShadowMap(int i, Texture map) => _DirectionalShadowMaps[i] = map;

        private Texture[] _DirectionalShadow2DMaps = new Texture[LightSpace.MaxDirectionalLightCount];
        public Texture GetDirectionalShadow2DMap(int i) => _DirectionalShadow2DMaps[i];
        internal void SetDirectionalShadow2DMap(int i, Texture map) => _DirectionalShadow2DMaps[i] = map;

        public Buffer PointLightBuffer { internal set; get; }
        public Texture PointLightClusterMap { internal set; get; }
        
        private Texture[] _PointShadowMaps = new Texture[LightSpace.MaxPointShadowCount];
        public Texture GetPointShadowMap(int i) => _PointShadowMaps[i];
        internal void SetPointShadowMap(int i, Texture map) => _PointShadowMaps[i] = map;

        public Buffer SpotLightBuffer { internal set; get; }
        public Texture SpotLightClusterMap { internal set; get; }
        public Buffer SpotShadowBuffer { internal set; get; }

        private Texture[] _SpotShadowMaps = new Texture[LightSpace.MaxSpotShadowCount];
        public Texture GetSpotShadowMap(int i) => _SpotShadowMaps[i];
        internal void SetSpotShadowMap(int i, Texture map) => _SpotShadowMaps[i] = map;
        public Texture EnvMap { internal set; get; }
        public Texture BRDFMap { internal set; get; }

        internal LightSpaceState() { }

        internal void Flush(Graphics graphics)
        {
            var command = graphics.Command((GraphicsApi api) => { });
            if (LightBuffer != null) command.AddFence(LightBuffer, BatchFlag.Read);
            if (DirectionalLightBuffer != null) command.AddFence(DirectionalLightBuffer, BatchFlag.Read);
            for (var i = 0; i < LightSpace.MaxDirectionalLightCount; i++)
            {
                if (_DirectionalShadowMaps[i] != null) command.AddFence(_DirectionalShadowMaps[i], BatchFlag.Read);
                if (_DirectionalShadow2DMaps[i] != null) command.AddFence(_DirectionalShadow2DMaps[i], BatchFlag.Read);
            }
            if (PointLightBuffer != null) command.AddFence(PointLightBuffer, BatchFlag.Read);
            if (PointLightClusterMap != null) command.AddFence(PointLightClusterMap, BatchFlag.Read);
            for (var i = 0; i < LightSpace.MaxPointShadowCount; i++)
            {
                if (_PointShadowMaps[i] != null) command.AddFence(_PointShadowMaps[i], BatchFlag.Read);
            }
            if (SpotLightBuffer != null) command.AddFence(SpotLightBuffer, BatchFlag.Read);
            if (SpotLightClusterMap != null) command.AddFence(SpotLightClusterMap, BatchFlag.Read);
            if (SpotShadowBuffer != null) command.AddFence(SpotShadowBuffer, BatchFlag.Read);
            for (var i = 0; i < LightSpace.MaxSpotShadowCount; i++)
            {
                if (_SpotShadowMaps[i] != null) command.AddFence(_SpotShadowMaps[i], BatchFlag.Read);
            }
            if (EnvMap != null) command.AddFence(EnvMap, BatchFlag.Read);
            if (BRDFMap != null) command.AddFence(BRDFMap, BatchFlag.Read);
        }
    }
}
