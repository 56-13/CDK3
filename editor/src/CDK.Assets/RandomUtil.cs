using System;

using MathUtil = CDK.Drawing.MathUtil;

namespace CDK.Assets
{
    public static class RandomUtil
    {
        private static Random _random = new Random();

        public static int Next() => _random.Next();
        public static int Next(int min, int max) => _random.Next(min, max);
        public static float NextFloat() => _random.NextFloat();
        public static float NextFloat(float min, float max) => _random.NextFloat(min, max);
        public static float NextFloat(this Random random) => (float)random.NextDouble();
        public static float NextFloat(this Random random, float min, float max) => min + random.NextFloat() * (max - min);
        public static float ToFloat(int random) => (float)((double)(uint)random / uint.MaxValue);
        public static float ToFloat(int random, float min, float max) => min + ToFloat(random) * (max - min);
        public static float ToFloatSequenced(int random, int seq)
        {
            var s = Math.Abs(seq) % 32;
            var r = (uint)random;
            r = (r >> s) | (r << (32 - s));
            return (float)Math.Round((double)r / uint.MaxValue, 6);
        }
        public static float ToFloatSequenced(int random, int seq0, int seq1, float rate)
        {
            return MathUtil.Lerp(ToFloatSequenced(random, seq0), ToFloatSequenced(random, seq1), rate);
        }
        
    }
}
