using System;

using CDK.Drawing;

namespace CDK.Assets
{
    public class DrawEventArgs : EventArgs
    {
        public Graphics Graphics { private set; get; }

        public DrawEventArgs(Graphics graphics)
        {
            Graphics = graphics;
        }
    }
}
