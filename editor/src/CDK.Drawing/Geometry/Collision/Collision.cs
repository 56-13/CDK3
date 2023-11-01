using System;
using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
    public enum CollisionResult
    {
        Front,
        Back,
        Intersects
    }

    [Flags]
    public enum CollisionFlags
    {
        None = 0,
        Back = 1,
        Near = 2,
        Hit = 4,
        All = 7
    }

    public static partial class Collision
    {
        private static void Project(in Vector3 axis, IEnumerable<Vector3> points, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            foreach (var p in points)
            {
                var val = Vector3.Dot(axis, p);
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }

        private static void Project(in Vector3 axis, IEnumerable<Vector3> points, in Vector3 offset, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            foreach (var p in points)
            {
                var val = Vector3.Dot(axis, p - offset);
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }
    }
}
