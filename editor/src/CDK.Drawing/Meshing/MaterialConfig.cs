namespace CDK.Drawing.Meshing
{
    public class MaterialConfig
    {
        public string Name => _internal.Name;
        public Color4 Ambient => _internal.ColorAmbient.ToColor4();
        public Color4 Diffuse => _internal.ColorDiffuse.ToColor4();
        public Color4 Specular => _internal.ColorSpecular.ToColor4();
        public Color4 Reflective => _internal.ColorReflective.ToColor4();
        public Color4 Emissive => _internal.ColorEmissive.ToColor4();
        public float Shininess => _internal.Shininess;
        public float BumpScaling => _internal.BumpScaling;

        private Assimp.Material _internal;

        internal MaterialConfig(Assimp.Material material)
        {
            _internal = material;
        }
    }
}
