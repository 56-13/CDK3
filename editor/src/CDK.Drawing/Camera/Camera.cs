using System;
using System.Numerics;

namespace CDK.Drawing
{
    public struct Camera : IEquatable<Camera>
    {
        private Vector3 _Position;
        private Vector3 _Target;
        private Vector3 _Up;
        private float _Fov;
        private float _Width;
        private float _Height;
        private float _Near;
        private float _Far;

        private Matrix4x4? _ViewProjection;
        private Matrix4x4? _Projection;
        private Matrix4x4? _View;

        public Camera(float fov, float width, float height, float near, float far)
        {
            _Fov = fov;
            _Width = width;
            _Height = height;
            _Near = near;
            _Far = far;

            _Position = new Vector3(0, 0, GetDefaultDistance(fov, height));
            _Target = Vector3.Zero;
            _Up = -Vector3.UnitY;

            _ViewProjection = null;
            _Projection = null;
            _View = null;
        }

        public Matrix4x4 Projection
        {
            get
            {
                if (_Projection == null) _Projection = _Fov != 0 ? MatrixUtil.CreatePerspectiveFovLH(_Fov, _Width / _Height, _Near, _Far) : MatrixUtil.CreateOrthoLH(_Width, _Height, _Near, _Far);
                return _Projection.Value;
            }
        }

        public Matrix4x4 View
        {
            get
            {
                if (_View == null) _View = MatrixUtil.CreateLookAtLH(_Position, _Target, _Up);
                return _View.Value;
            }
        }

        public Matrix4x4 ViewProjection
        {
            get
            {
                if (_ViewProjection == null) _ViewProjection = View * Projection;
                return _ViewProjection.Value;
            }
        }

        public bool ProjectionUpdated => _Projection == null;
        public bool ViewUpdated => _View == null;

        private void ReleaseProjection()
        {
            _ViewProjection = null;
            _Projection = null;
        }

        private void ReleaseView()
        {
            _ViewProjection = null;
            _View = null;
        }

        public float Fov
        {
            set
            {
                if (_Fov != value)
                {
                    _Fov = value;
                    ReleaseProjection();
                }
            }
            get => _Fov;
        }

        public float Width
        {
            set
            {
                if (_Width != value)
                {
                    _Width = value;
                    ReleaseProjection();
                }
            }
            get => _Width;
        }
        public float Height
        {
            set
            {
                if (_Height != value)
                {
                    _Height = value;
                    ReleaseProjection();
                }
            }
            get => _Height;
        }

        public float Near
        {
            set
            {
                if (_Near != value)
                {
                    _Near = value;
                    ReleaseProjection();
                }
            }
            get => _Near;
        }

        public float Far
        {
            set
            {
                if (_Far != value)
                {
                    _Far = value;
                    ReleaseProjection();
                }
            }
            get => _Far;
        }
        public Vector3 Position
        {
            set
            {
                if (_Position != value)
                {
                    _Position = value;
                    ReleaseView();
                }
            }
            get => _Position;
        }

        public Vector3 Target
        {
            set
            {
                if (_Target != value)
                {
                    _Target = value;
                    ReleaseView();
                }
            }
            get => _Target;
        }

        public Vector3 Up
        {
            set
            {
                if (_Up != value)
                {
                    _Up = value;
                    ReleaseView();
                }
            }
            get => _Up;
        }

        public Vector3 Forward
        {
            get
            {
                var view = View;
                return new Vector3(view.M13, view.M23, view.M33);     //TODO:CHECK
            }
        }

        public Vector3 Right
        {
            get
            {
                var view = View;
                return new Vector3(view.M11, view.M21, view.M31);     //TODO:CHECK
            }
        }

        public void SetProjection(float fov, float width, float height, float znear, float zfar)
        {
            if (_Fov != fov || _Width != width || _Height != height || _Near != znear || _Far != zfar)
            {
                _Fov = fov;
                _Width = width;
                _Height = height;
                _Near = znear;
                _Far = zfar;
                ReleaseProjection();
            }
        }

        public void SetView(in Vector3 position, in Vector3 target, in Vector3 up)
        {
            if (_Position != position || _Target != target || _Up != up)
            {
                _Position = position;
                _Target = target;
                _Up = up;
                ReleaseView();
            }
        }

        public void ResetView()
        {
            SetView(new Vector3(0, 0, GetDefaultDistance(_Fov, _Height)), Vector3.Zero, -Vector3.UnitY);
        }

        public void Move(in Vector3 dir)
        {
            _Position += dir;
            _Target += dir;
            ReleaseView();
        }

        public void Rotate(in Vector3 axis, float angle)
        {
            var t = _Target - _Position;
            var rotation = Quaternion.CreateFromAxisAngle(axis, angle);
            t = Vector3.Transform(t, rotation);
            _Up = Vector3.Transform(_Up, rotation);
            _Target = t + _Position;
            ReleaseView();
        }

        public void Orbit(in Vector3 axis, float angle)
        {
            var p = _Position - _Target;
            var rotation = Quaternion.CreateFromAxisAngle(axis, angle);
            p = Vector3.Transform(p, rotation);
            _Up = Vector3.Transform(_Up, rotation);
            _Position = p + _Target;
            ReleaseView();
        }

        public static float GetDefaultDistance(float fov, float height)
        {
            return fov != 0 ? height / (2.0f * (float)Math.Tan(fov * 0.5f)) : height;
        }

        public float GetDefaultDistance() => GetDefaultDistance(_Fov, _Height);

        private Vector3 Unproject(in Vector3 v, in Matrix4x4 invWorldViewProjection)
        {
            var result = new Vector3(
                ((v.X / _Width) * 2) - 1,
                -(((v.Y / _Height) * 2) - 1),
                (v.Z - _Near) / (_Far - _Near)
            );

            result = VectorUtil.TransformCoordinate(result, invWorldViewProjection);

            return result;
        }

        public Ray PickRay(in Vector2 screenPosition)
        {
            var nearSource = new Vector3(screenPosition.X, screenPosition.Y, _Near);
            var farSource = new Vector3(screenPosition.X, screenPosition.Y, _Far);

            if (!Matrix4x4.Invert(ViewProjection, out var invViewProjection)) throw new InvalidOperationException();
            var nearPoint = Unproject(nearSource, invViewProjection);
            var farPoint = Unproject(farSource, invViewProjection);
            var direction = Vector3.Normalize(farPoint - nearPoint);

            return new Ray(nearPoint, direction);
        }

        public BoundingFrustum BoundingFrustum(in Rectangle screenRect)
        {
            var yScale = (float)(1.0 / Math.Tan(_Fov * 0.5f));
            var xScale = yScale / (_Width / _Height);

            var nearWidth = _Near * 2.0f / xScale;
            var nearHeight = _Near * 2.0f / yScale;

            var left = (screenRect.Left / screenRect.Width - 0.5f) * nearWidth;
            var right = (screenRect.Right / screenRect.Width - 0.5f) * nearWidth;
            var top = (screenRect.Top / screenRect.Height - 0.5f) * nearHeight;
            var bottom = (screenRect.Bottom / screenRect.Height - 0.5f) * nearHeight;

            MatrixUtil.CreatePerspectiveOffCenterLH(left, right, top, bottom, _Near, _Far, out var projection);
            return new BoundingFrustum(View * projection);
        }

        public BoundingFrustum BoundingFrustum() => new BoundingFrustum(ViewProjection);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(_Position.GetHashCode());
            hash.Combine(_Target.GetHashCode());
            hash.Combine(_Up.GetHashCode());
            hash.Combine(_Fov.GetHashCode());
            hash.Combine(_Width.GetHashCode());
            hash.Combine(_Height.GetHashCode());
            hash.Combine(_Near.GetHashCode());
            hash.Combine(_Far.GetHashCode());
            return hash;
        }

        public static bool operator ==(in Camera left, in Camera right) => left.Equals(right);
        public static bool operator !=(in Camera left, in Camera right) => !left.Equals(right);

        public bool Equals(Camera other)
        {
            return _Position == other._Position &&
                _Target == other._Target &&
                _Up == other._Up &&
                _Fov == other._Fov &&
                _Width == other._Width &&
                _Height == other._Height &&
                _Near == other._Near &&
                _Far == other._Far;
        }

        public override bool Equals(object obj) => obj is Camera other && Equals(other);
    }
}
