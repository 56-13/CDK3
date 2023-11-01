namespace CDK.Drawing.Meshing
{
    internal struct BoneIndex4
    {
        public byte X;
        public byte Y;
        public byte Z;
        public byte W;

        public BoneIndex4(byte x, byte y, byte z, byte w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static readonly BoneIndex4 Zero = new BoneIndex4();
    }
}

