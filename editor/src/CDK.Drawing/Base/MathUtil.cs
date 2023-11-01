using System;

namespace CDK.Drawing
{
    public static class MathUtil
    {
        public const float ZeroTolerance = 1e-6f;
        public const float Pi = (float)Math.PI;
        public const float TwoPi = (float)(2 * Math.PI);
        public const float PiOverTwo = (float)(Math.PI / 2);
        public const float PiOverFour = (float)(Math.PI / 4);
        public const float ToRadians = Pi / 180;
        public const float ToDegrees = 180 / Pi;

        public unsafe static bool NearEqual(float a, float b)
        {
            if (NearZero(a - b)) return true;

            var aInt = *(int*)&a;
            var bInt = *(int*)&b;

            if ((aInt < 0) != (bInt < 0)) return false;

            var ulp = Math.Abs(aInt - bInt);

            const int maxUlp = 4;
            return (ulp <= maxUlp);
        }

        public static bool NearZero(float a) => Math.Abs(a) < ZeroTolerance;
        public static bool NearOne(float a) => NearZero(a - 1.0f);
        public static float Clamp(float value, float min, float max) => value < min ? min : value > max ? max : value;
        public static int Clamp(int value, int min, int max) => value < min ? min : value > max ? max : value;
        public static float Lerp(float from, float to, float amount) => (1 - amount) * from + amount * to;
        public static double Clamp(double value, double min, double max) => value < min ? min : value > max ? max : value;
        public static double Lerp(double from, double to, double amount) => (1 - amount) * from + amount * to;
        public static int Lerp(int from, int to, float amount) => (int)Lerp((float)from, (float)to, amount);
        public static byte Lerp(byte from, byte to, float amount) => (byte)Lerp((float)from, (float)to, amount);
        public static float SmoothStep(float amount)
        {
            return (amount <= 0) ? 0
                : (amount >= 1) ? 1
                : amount * amount * (3 - (2 * amount));
        }

        public static float SmootherStep(float amount)
        {
            return (amount <= 0) ? 0
                : (amount >= 1) ? 1
                : amount * amount * amount * (amount * ((amount * 6) - 15) + 10);
        }

        public static double SmoothStep(double amount)
        {
            return (amount <= 0) ? 0
                : (amount >= 1) ? 1
                : amount * amount * (3 - (2 * amount));
        }

        public static double SmootherStep(double amount)
        {
            return (amount <= 0) ? 0
                : (amount >= 1) ? 1
                : amount * amount * amount * (amount * ((amount * 6) - 15) + 10);
        }

        public static float Gauss(float amplitude, float x, float y, float centerX, float centerY, float sigmaX, float sigmaY) => (float)Gauss((double)amplitude, x, y, centerX, centerY, sigmaX, sigmaY);
        public static double Gauss(double amplitude, double x, double y, double centerX, double centerY, double sigmaX, double sigmaY)
        {
            var cx = x - centerX;
            var cy = y - centerY;

            var componentX = (cx * cx) / (2 * sigmaX * sigmaX);
            var componentY = (cy * cy) / (2 * sigmaY * sigmaY);

            return amplitude * Math.Exp(-(componentX + componentY));
        }

        public static float[] GaussKernel(int radius, float sigma)
        {
            var c = 1.0 / (2.0 * Math.PI * sigma);
            var kernel = new float[radius + 1];
            var sum = kernel[0] = (float)c;
            for (var i = 1; i <= radius; i++)
            {
                var e = -(i * i) / (2.0 * sigma);
                var v = (float)(c * Math.Exp(e));
                kernel[i] = v;
                sum += v * 2;
            }
            for (var i = 0; i < kernel.Length; i++)
            {
                kernel[i] /= sum;
            }
            return kernel;
        }

        public static byte FloatToSNorm(float value) => (byte)Clamp((value * 0.5f + 0.5f) * 255, 0, 255);
        public static byte FloatToUNorm(float value) => (byte)Clamp(value * 255, 0, 255);
        public static float SNormToFloat(byte value) => value / 255.0f * 2.0f - 1.0f;
        public static float UNormToFloat(byte value) => value / 255.0f;
    }
}
