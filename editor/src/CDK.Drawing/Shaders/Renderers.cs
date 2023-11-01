namespace CDK.Drawing
{
    public static class Renderers
    {
        private static StandardRenderer _Standard;
        public static StandardRenderer Standard
        {
            get
            {
                if (_Standard == null) _Standard = new StandardRenderer();

                return _Standard;
            }
        }

        private static ShadowRenderer _Shadow;
        public static ShadowRenderer Shadow
        {
            get
            {
                if (_Shadow == null) _Shadow = new ShadowRenderer();

                return _Shadow;
            }
        }

        private static Shadow2DRenderer _Shadow2D;
        public static Shadow2DRenderer Shadow2D
        {
            get
            {
                if (_Shadow2D == null) _Shadow2D = new Shadow2DRenderer();

                return _Shadow2D;
            }
        }

        private static DistortionRenderer _Distortion;
        public static DistortionRenderer Distortion
        {
            get
            {
                if (_Distortion == null) _Distortion = new DistortionRenderer();

                return _Distortion;
            }
        }

        internal static void DisposeShared()
        {
            _Standard?.Dispose();
            _Standard = null;
            _Shadow?.Dispose();
            _Shadow = null;
            _Shadow2D?.Dispose();
            _Shadow2D = null;
            _Distortion?.Dispose();
            _Distortion = null;
        }
    }
}
