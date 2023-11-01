using System;
using System.IO;


namespace CDK.Drawing
{
    public static class ShaderCode
    {
        public static string Base { private set; get; }
        public static string VSBase { private set; get; }
        public static string FSBase { private set; get; }
        static ShaderCode()
        {
            Base = $"#define MaxBoneCount {GraphicsContext.Instance.MaxUniformBlockSize / 64}\n";
            Base += File.ReadAllText(Path.Combine("Resources", "base.glsl"));
            
            VSBase = Base;
            VSBase += File.ReadAllText(Path.Combine("Resources", "base_vs.glsl"));
            
            FSBase = Base;
            FSBase += File.ReadAllText(Path.Combine("Resources", "base_fs.glsl"));
        }
    }
}
