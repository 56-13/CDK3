namespace CDK.Drawing
{
    public static class Shaders
    {

        private static EmptyShader _Empty;
        public static EmptyShader Empty
        {
            get
            {
                if (_Empty == null) _Empty = new EmptyShader();

                return _Empty;
            }
        }

        private static BlitShader _Blit;
        public static BlitShader Blit
        {
            get
            {
                if (_Blit == null) _Blit = new BlitShader();

                return _Blit;
            }
        }

        private static BlurShader _Blur;
        public static BlurShader Blur
        {
            get
            {
                if (_Blur == null) _Blur = new BlurShader();

                return _Blur;
            }
        }


        private static LensShader _Lens;
        public static LensShader Lens
        {
            get
            {
                if (_Lens == null) _Lens = new LensShader();

                return _Lens;
            }
        }

        private static WaveShader _Wave;
        public static WaveShader Wave
        {
            get
            {
                if (_Lens == null) _Wave = new WaveShader();

                return _Wave;
            }
        }

        private static StrokeShader _Stroke;
        public static StrokeShader Stroke
        {
            get
            {
                if (_Stroke == null) _Stroke = new StrokeShader();

                return _Stroke;
            }
        }

        private static SkyboxShader _Skybox;
        public static SkyboxShader Skybox
        {
            get
            {
                if (_Skybox == null) _Skybox = new SkyboxShader();

                return _Skybox;
            }
        }

        private static PostProcessShader _PostProcess;
        public static PostProcessShader PostProcess
        {
            get
            {
                if (_PostProcess == null) _PostProcess = new PostProcessShader();

                return _PostProcess;
            }
        }

        internal static void DisposeShared()
        {
            _Empty?.Dispose();
            _Empty = null;
            _Blit?.Dispose();
            _Blit = null;
            _Blur?.Dispose();
            _Blur = null;
            _Lens?.Dispose();
            _Lens = null;
            _Wave?.Dispose();
            _Wave = null;
            _Stroke?.Dispose();
            _Stroke = null;
            _Skybox?.Dispose();
            _Skybox = null;
            _PostProcess?.Dispose();
            _PostProcess = null;
        }
    }
}
