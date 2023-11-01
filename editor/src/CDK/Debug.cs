using System;

namespace CDK
{
    public class Debug
    {
        public static void Assert(bool condition)
        {
#if DEBUG
            if (!condition) throw new InvalidOperationException();
#endif
        }

        public static void Assert(bool condition, string message)
        {
#if DEBUG
            if (!condition) throw new InvalidOperationException(message);
#endif
        }

    }
}
