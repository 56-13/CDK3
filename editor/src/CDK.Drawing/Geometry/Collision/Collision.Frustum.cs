using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
	public static partial class Collision
    {
		public static CollisionResult FrustumIntersectsPoint(in BoundingFrustum frustum, in Vector3 point)
		{
			var result = CollisionResult.Back;

			for (var i = 0; i < 6; i++)
			{
				switch (PlaneIntersectsPoint(frustum.GetPlane(i), point))
				{
					case CollisionResult.Back:
						return CollisionResult.Front;
					case CollisionResult.Intersects:
						result = CollisionResult.Intersects;
						break;
				}
			}
			return result;
		}

		public static CollisionResult FrustumIntersectsSegment(in BoundingFrustum frustum, in Segment seg)
		{
			var result = CollisionResult.Back;

			for (var i = 0; i < 6; i++)
			{
				switch (PlaneIntersectsSegment(frustum.GetPlane(i), seg, out _))
				{
					case CollisionResult.Back:
						return CollisionResult.Front;
					case CollisionResult.Intersects:
						result = CollisionResult.Intersects;
						break;
				}
			}
			return result;
		}

		public static CollisionResult FrustumIntersectsTriangle(in BoundingFrustum frustum, in Triangle tri)
		{
			var result = CollisionResult.Back;

			for (var i = 0; i < 6; i++)
			{
				switch (PlaneIntersectsTriangle(frustum.GetPlane(i), tri))
				{
					case CollisionResult.Back:
						return CollisionResult.Front;
					case CollisionResult.Intersects:
						result = CollisionResult.Intersects;
						break;
				}
			}
			return result;
		}

		public static CollisionResult FrustumIntersectsSphere(in BoundingFrustum frustum, in BoundingSphere sphere)
		{
			var result = CollisionResult.Back;

			for (var i = 0; i < 6; i++)
			{
				switch (PlaneIntersectsSphere(frustum.GetPlane(i), sphere))
				{
					case CollisionResult.Back:
						return CollisionResult.Front;
					case CollisionResult.Intersects:
						result = CollisionResult.Intersects;
						break;
				}
			}
			return result;
		}

		public static CollisionResult FrustumIntersectsCapsule(in BoundingFrustum frustum, in BoundingCapsule capsule)
		{
			var result = CollisionResult.Back;

			for (var i = 0; i < 6; i++)
			{
				switch (PlaneIntersectsCapsule(frustum.GetPlane(i), capsule, out _))
				{
					case CollisionResult.Back:
						return CollisionResult.Front;
					case CollisionResult.Intersects:
						result = CollisionResult.Intersects;
						break;
				}
			}
			return result;
		}

		public static CollisionResult FrustumIntersectsABox(in BoundingFrustum frustum, in ABoundingBox box)
		{
			var result = CollisionResult.Back;

			for (var i = 0; i < 6; i++)
			{
				switch (PlaneIntersectsABox(frustum.GetPlane(i), box))
				{
					case CollisionResult.Back:
						return CollisionResult.Front;
					case CollisionResult.Intersects:
						result = CollisionResult.Intersects;
						break;
				}
			}
			return result;
		}

		public static CollisionResult FrustumIntersectsOBox(in BoundingFrustum frustum, in OBoundingBox box)
		{
			var result = CollisionResult.Back;

			for (var i = 0; i < 6; i++)
			{
				switch (PlaneIntersectsOBox(frustum.GetPlane(i), box))
				{
					case CollisionResult.Back:
						return CollisionResult.Front;
					case CollisionResult.Intersects:
						result = CollisionResult.Intersects;
						break;
				}
			}
			return result;
		}

		public static CollisionResult FrustumIntersectsMesh(in BoundingFrustum frustum, BoundingMesh mesh)
		{
			var result = CollisionResult.Back;

			for (var i = 0; i < 6; i++)
			{
				switch (PlaneIntersectsMesh(frustum.GetPlane(i), mesh))
				{
					case CollisionResult.Back:
						return CollisionResult.Front;
					case CollisionResult.Intersects:
						result = CollisionResult.Intersects;
						break;
				}
			}
			return result;
		}

		public static CollisionResult FrustumIntersectsFrustum(in BoundingFrustum frustum0, in BoundingFrustum frustum1)
		{
			var frustumCorners1 = frustum1.GetCorners();

			var result = FrustumIntersectsPoint(frustum0, frustumCorners1[0]);
			for (var i = 1; i < 8; i++)
			{
				if (FrustumIntersectsPoint(frustum0, frustumCorners1[i]) != result) return CollisionResult.Intersects;
			}
			return result;
		}
	}
}
