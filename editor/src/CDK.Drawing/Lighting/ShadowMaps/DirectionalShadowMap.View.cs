using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
    internal partial class DirectionalShadowMap
    {
        private void UpdateView()
        {
            //==================================================================================================
            /*
            _camera.Width = 800;
            _camera.Height = 600;
            _camera.Far = 70;

            _camera.Near = 1;
            _camera.Up = new Vector3(0, 1, 0);
            _camera.Position = new Vector3(13.306376f, 7.213725f, -16.445047f);
            _camera.Target = _camera.Position + new Vector3(-0.786640f, -0.421410f, -0.451232f);
            _light.Direction = new Vector3(0.014217f, -0.703279f, -0.710771f);
            _scene = new BoundingBox(new Vector3(-100, -4, -100), new Vector3(100, 11, 100));
            */
            //==================================================================================================
            var points = GetFocusedLightVolumePoints();

            if ((Visible = points.Count != 0))
            {
                UpdateView(points);
                UpdateView2D(points);
            }
        }

        private void UpdateView2D(List<Vector3> points)
        {
            GetAABB(points, out var min, out var max);

            var lightViewProj = MatrixUtil.CreateOrthoOffCenterLH(min.X, max.X, min.Y, max.Y, min.Z, max.Z);

            ViewProjection2D = lightViewProj;
        }

        private void UpdateView(List<Vector3> points)
        {
            var left = Vector3.Cross(_light.Direction, _camera.Forward);
            var up = Vector3.Normalize(Vector3.Cross(left, _light.Direction));

            var lightView = MatrixUtil.CreateLookAtRH(_camera.Position, _camera.Position + _light.Direction, up);

            GetAABB(points, lightView, out var min, out var max);

            var cosGamma = (double)Vector3.Dot(_camera.Forward, _light.Direction);
            var sinGamma = Math.Sqrt(1.0 - cosGamma * cosGamma);
            var factor = 1.0 / sinGamma;
            var z_n = factor * _camera.Near;
            var d = max.Y - min.Y; //perspective transform depth //light space y extents
            var z_f = z_n + d * sinGamma;
            var n = (z_n + Math.Sqrt(z_f * z_n)) / sinGamma;
            var f = n + d;

            //var pos = _camera.Position - up * (n - _camera.Near);
            var pos = _camera.Position - up * Math.Max((float)(n - _camera.Near), -min.Y + 1);

            lightView = MatrixUtil.CreateLookAtRH(pos, pos + _light.Direction, up);

            var a = (f + n) / (f - n);
            var b = -2 * f * n / (f - n);

            var lspmtx = Matrix4x4.Identity;
            lspmtx.M22 = (float)a;      // [ 1 0 0 0] 
            lspmtx.M42 = (float)b;      // [ 0 a 0 1]
            lspmtx.M24 = 1;             // [ 0 0 1 0]
            lspmtx.M44 = 0;             // [ 0 b 0 0]

            var lightViewProj = lightView * lspmtx;

            GetAABB(points, lightViewProj, out min, out max);

            lightViewProj *= MatrixUtil.CreateOrthoOffCenterLH(min.X, max.X, max.Y, min.Y, max.Z, min.Z);     //LH, 컬링

            ViewProjection = lightViewProj;

            //=========================================================
            /*
            {
                Console.WriteLine($"d:{d} minz:{min.Z} maxz:{max.Z} up:{up} pos:{pos} posd:{n - _camera.Near} / {posd}");

                Vector4 vp;
                for (var i = 0; i <= 10; i++)
                {
                    var vp = VectorUtil.TransformPerspective(new Vector3(40 * i, 40 * i, 0), ViewProjection);
                    Console.WriteLine($"{40 * i} => view:X:{vp.X / vp.W} Y:{vp.Y / vp.W} Z:{vp.Z / vp.W} W:{vp.W}");
                }
            }
            Console.WriteLine("================================");
            */
            //=========================================================
        }

        private void GetAABB(List<Vector3> points, out Vector3 min, out Vector3 max)
        {
            min = max = points[0];

            for (var i = 1; i < points.Count; i++)
            {
                var p = points[i];
                if (p.X < min.X) min.X = p.X;
                if (p.Y < min.Y) min.Y = p.Y;
                if (p.Z < min.Z) min.Z = p.Z;
                if (p.X > max.X) max.X = p.X;
                if (p.Y > max.Y) max.Y = p.Y;
                if (p.Z > max.Z) max.Z = p.Z;
            }
        }

        private void GetAABB(List<Vector3> points, in Matrix4x4 view, out Vector3 min, out Vector3 max)
        {
            var vp = VectorUtil.TransformCoordinate(points[0], view);
            min = vp;
            max = vp;

            for (var i = 1; i < points.Count; i++)
            {
                vp = VectorUtil.TransformCoordinate(points[i], view);
                if (vp.X < min.X) min.X = vp.X;
                if (vp.Y < min.Y) min.Y = vp.Y;
                if (vp.Z < min.Z) min.Z = vp.Z;
                if (vp.X > max.X) max.X = vp.X;
                if (vp.Y > max.Y) max.Y = vp.Y;
                if (vp.Z > max.Z) max.Z = vp.Z;
            }
        }

        private List<Vector3> GetFocusedLightVolumePoints()
        {
            var obj = GetFrustumObject();

            obj = ClipObjectByScene(obj);

            return GetObjectLightVolumePoints(obj);
        }

        private List<List<Vector3>> GetFrustumObject()
        {
            if (!Matrix4x4.Invert(_camera.ViewProjection, out var inv)) throw new InvalidOperationException();

            var p = new Vector3[8];
            p[0] = VectorUtil.TransformCoordinate(new Vector3(1, -1, -1), inv);
            p[1] = VectorUtil.TransformCoordinate(new Vector3(-1, -1, -1), inv);
            p[2] = VectorUtil.TransformCoordinate(new Vector3(-1, 1, -1), inv);
            p[3] = VectorUtil.TransformCoordinate(new Vector3(1, 1, -1), inv);
            p[4] = VectorUtil.TransformCoordinate(new Vector3(1, -1, 1), inv);
            p[5] = VectorUtil.TransformCoordinate(new Vector3(-1, -1, 1), inv);
            p[6] = VectorUtil.TransformCoordinate(new Vector3(-1, 1, 1), inv);
            p[7] = VectorUtil.TransformCoordinate(new Vector3(1, 1, 1), inv);

            var obj = new List<List<Vector3>>(6);

            for (var i = 0; i < 6; i++)
            {
                obj.Add(new List<Vector3>(4));
            }
            //near poly ccw
            var ps = obj[0];
            for (var i = 0; i < 4; i++)
            {
                ps.Add(p[i]);
            }
            //far poly ccw
            ps = obj[1];
            for (var i = 4; i < 8; i++)
            {
                ps.Add(p[11 - i]);
            }
            //left poly ccw
            ps = obj[2];
            ps.Add(p[0]);
            ps.Add(p[3]);
            ps.Add(p[7]);
            ps.Add(p[4]);
            //right poly ccw
            ps = obj[3];
            ps.Add(p[1]);
            ps.Add(p[5]);
            ps.Add(p[6]);
            ps.Add(p[2]);
            //bottom poly ccw
            ps = obj[4];
            ps.Add(p[4]);
            ps.Add(p[5]);
            ps.Add(p[1]);
            ps.Add(p[0]);
            //top poly ccw
            ps = obj[5];
            ps.Add(p[6]);
            ps.Add(p[7]);
            ps.Add(p[3]);
            ps.Add(p[2]);

            return obj;
        }

        private List<List<Vector3>> ClipObjectByScene(List<List<Vector3>> obj)
        {
            var planes = new Plane[6];

            planes[0].Normal = new Vector3(0, -1, 0);
            planes[0].D = _space.Minimum.Y;
            planes[1].Normal = new Vector3(0, 1, 0);
            planes[1].D = -_space.Maximum.Y;
            planes[2].Normal = new Vector3(-1, 0, 0);
            planes[2].D = _space.Minimum.X;
            planes[3].Normal = new Vector3(1, 0, 0);
            planes[3].D = -_space.Maximum.X;
            planes[4].Normal = new Vector3(0, 0, -1);
            planes[4].D = _space.Minimum.Z;
            planes[5].Normal = new Vector3(0, 0, 1);
            planes[5].D = -_space.Maximum.Z;

            foreach (var plane in planes) obj = ClipObjectByPlane(obj, plane);

            return obj;
        }

        private List<List<Vector3>> ClipObjectByPlane(List<List<Vector3>> obj, in Plane plane)
        {
            var inter = new List<List<Vector3>>(obj.Count);
            var result = new List<List<Vector3>>(obj.Count);

            foreach (var objp in obj)
            {
                var interp = new List<Vector3>(objp.Count);
                var resultp = new List<Vector3>(objp.Count);

                ClipPointsByPlane(objp, plane, resultp, interp);

                if (resultp.Count != 0)
                {
                    result.Add(resultp);
                    inter.Add(interp);
                }
            }
            AppendIntersectionPoints(result, inter);

            return result;
        }

        private void ClipPointsByPlane(List<Vector3> objp, in Plane plane, List<Vector3> resultp, List<Vector3> interp)
        {
            if (objp.Count < 3) return;

            var outside = new bool[objp.Count];

            for (var i = 0; i < outside.Length; i++)
            {
                outside[i] = plane.Intersects(objp[i]) == CollisionResult.Front;
            }
            for (var i = 0; i < objp.Count; i++)
            {
                var next = (i + 1) % objp.Count;

                if (outside[i] && outside[next])
                {
                    continue;
                }
                if (outside[i] || outside[next])
                {
                    var diff = objp[next] - objp[i];
                    var length = diff.Length();
                    diff /= length;

                    var ray = new Ray(objp[i], diff);

                    if (ray.Intersects(plane, out float distance) && distance <= length)
                    {
                        var inter = objp[i] + diff * distance;

                        resultp.Add(inter);
                        interp.Add(inter);
                    }
                    if (outside[next]) continue;
                }
                resultp.Add(objp[next]);
            }
        }

        private void AppendIntersectionPoints(List<List<Vector3>> obj, List<List<Vector3>> inter)
        {
            if (inter.Count < 3) return;

            int i;
            for (i = inter.Count; i > 0; i--)
            {
                if (inter[i - 1].Count == 2)
                {
                    break;
                }
            }
            while (inter.Count > i) inter.RemoveAt(inter.Count - 1);

            if (inter.Count < 3) return;

            var objp = new List<Vector3>();
            obj.Add(objp);

            var interp = inter.Last();

            objp.Add(interp[0]);
            objp.Add(interp[1]);

            inter.RemoveAt(inter.Count - 1);

            while (inter.Count > 0)
            {
                var lastp = objp.Last();

                var nr = FindSamePointInObjectAndSwapWithLast(inter, lastp);

                if (nr >= 0)
                {
                    interp = inter.Last();

                    objp.Add(interp[(nr + 1) % 2]);
                }
                inter.RemoveAt(inter.Count - 1);
            }

            objp.RemoveAt(objp.Count - 1);
        }
        
        private float GetRelativeEpsilon(float a, float epsilon)
        {
            return Math.Max(Math.Abs(a * epsilon), epsilon);
        }

        private bool NearEqual(float a, float b)
        {
            const float Epsilon = 0.001f;

            var releps = GetRelativeEpsilon(a, Epsilon);

            return a - releps <= b && b <= a + releps;
        }
        
        private int FindSamePointInVecPoint(List<Vector3> poly, in Vector3 p)
        {
            for (var i = 0; i < poly.Count; i++)
            {
                var pp = poly[i];

                if (NearEqual(pp.X, p.X) && NearEqual(pp.Y, p.Y) && NearEqual(pp.Z, p.Z))
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindSamePointInObjectAndSwapWithLast(List<List<Vector3>> inter, in Vector3 p)
        {
            for (var i = inter.Count - 1; i >= 0; i--)
            {
                var interp = inter[i];

                if (interp.Count == 2)
                {
                    var nr = FindSamePointInVecPoint(interp, p);

                    if (nr >= 0)
                    {
                        inter[i] = inter[inter.Count - 1];
                        inter[inter.Count - 1] = interp;
                        return nr;
                    }
                }
            }
            return -1;
        }

        private List<Vector3> GetObjectLightVolumePoints(List<List<Vector3>> obj)
        {
            var points = new List<Vector3>(256);

            foreach (var objp in obj) points.AddRange(objp);

            var count = points.Count;
            var ld = -_light.Direction;
            for (var i = 0; i < count; i++)
            {
                if (LineIntersectsBox(points[i], ld, _space, out var pt)) points.Add(pt);
            }
            return points;
        }

        private static bool ClipTest(float p, float q, ref float u1, ref float u2)
        {
            if (p < 0)
            {
                var r = q / p;
                if (r > u2) return false;
                else
                {
                    if (r > u1) u1 = r;
                    return true;
                }
            }
            else if (p > 0)
            {
                var r = q / p;
                if (r < u1) return false;
                else
                {
                    if (r < u2) u2 = r;
                    return true;
                }
            }
            else
            {
                return q >= 0;
            }
        }

        private static bool LineIntersectsBox(in Vector3 p, in Vector3 dir, in ABoundingBox b, out Vector3 v)
        {
            var t1 = 0.0f;
            var t2 = float.MaxValue;

            var intersect =
                ClipTest(-dir.Z, p.Z - b.Minimum.Z, ref t1, ref t2) && ClipTest(dir.Z, b.Maximum.Z - p.Z, ref t1, ref t2) &&
                ClipTest(-dir.Y, p.Y - b.Minimum.Y, ref t1, ref t2) && ClipTest(dir.Y, b.Maximum.Y - p.Y, ref t1, ref t2) &&
                ClipTest(-dir.X, p.X - b.Minimum.X, ref t1, ref t2) && ClipTest(dir.X, b.Maximum.X - p.X, ref t1, ref t2);

            v = p;

            if (!intersect)
            {
                return false;
            }

            intersect = false;

            if (t1 >= 0)
            {
                v += dir * t1;
                intersect = true;
            }
            if (t2 >= 0)
            {
                v += dir * t2;
                intersect = true;
            }

            return intersect;
        }
    }
}
