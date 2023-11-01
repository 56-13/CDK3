using System;

namespace CDK.Drawing
{
    public class Program : GraphicsResource
    {
        public int Object { private set; get; }

        public Program()
        {
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => Object = api.CreateProgram());
            command?.AddFence(this, BatchFlag.ReadWrite);
        }

        ~Program()
        {
            if (GraphicsContext.IsCreated) GraphicsContext.Instance.Invoke(false, (GraphicsApi api) => api.DeleteProgram(Object));
        }

        public override void Dispose()
        {
            GraphicsContext.Instance.Invoke(false, (GraphicsApi api) => api.DeleteProgram(Object));

            GC.SuppressFinalize(this);
        }

        public override int Cost => 0;

        public void Attach(Shader shader)
        {
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => api.AttachShader(Object, shader.Object));

            if (command != null)
            {
                command.AddFence(shader, BatchFlag.Read);
                command.AddFence(this, BatchFlag.Write);
            }
        }

        public void Detach(Shader shader)
        {
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => api.DetachShader(Object, shader.Object));

            if (command != null)
            {
                command.AddFence(shader, BatchFlag.Read);
                command.AddFence(this, BatchFlag.Write);
            }
        }

        public void Link()
        {
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => api.LinkProgram(Object));

            if (command != null)
            {
                command.AddFence(this, BatchFlag.ReadWrite);

                Flush();
            }
        }

        public void Use(GraphicsApi api)
        {
            api.UseProgram(Object);
        }
    }
}


