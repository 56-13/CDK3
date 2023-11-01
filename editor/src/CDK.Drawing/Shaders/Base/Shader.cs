using System;

namespace CDK.Drawing
{
    public class Shader : GraphicsResource
    {
        public ShaderType Type { private set; get; }
        public int Object { private set; get; }

        public Shader(ShaderType type, params string[] sources)
        {
            Type = type;

            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => Object = api.CreateShader(type, sources));

            command?.AddFence(this, BatchFlag.ReadWrite);
        }

        ~Shader()
        {
            if (GraphicsContext.IsCreated) GraphicsContext.Instance.Invoke(false, (GraphicsApi api) => api.DeleteShader(Object));
        }

        public override void Dispose()
        {
            GraphicsContext.Instance.Invoke(false, (GraphicsApi api) => api.DeleteShader(Object));

            GC.SuppressFinalize(this);
        }

        public override int Cost => 0;
    }
}

