using System;

namespace CDK
{
    public struct HashCode
    {
        private int _hash;

        public static readonly HashCode Initializer = new HashCode { _hash = 5381 };

        public void Combine(int hash)
        {
            _hash =  ((_hash << 5) + _hash) ^ hash;
        }

        public static implicit operator int(in HashCode hash) => hash._hash;
    }
}
