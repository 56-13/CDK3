using System;

namespace CDK.Assets
{
    public class AssetException : Exception
    {
        public Asset Asset { private set; get; }

        public AssetException(Asset asset, string message)
            : base(message)
        {
            Asset = asset;
        }
    }
}
