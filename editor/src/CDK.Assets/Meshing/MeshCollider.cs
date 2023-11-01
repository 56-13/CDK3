using System;
using System.Numerics;
using System.Xml;
using System.IO;

using CDK.Drawing;
using CDK.Drawing.Meshing;

namespace CDK.Assets.Meshing
{
    public enum ColliderShape
    {
        Sphere,
        Box,
        Capsule
    }

    public class MeshCollider : AssetElement
    {
        public MeshGeometry Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        private string _Name;
        public string Name
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = null;

                SetProperty(ref _Name, value);
            }
            get => _Name ?? _NodeName ?? "Root";
        }

        private string _NodeName;
        public string NodeName
        {
            set
            {
                if (SetProperty(ref _NodeName, value)) OnPropertyChanged("Name");
            }
            get => _NodeName;
        }

        private Vector3 _Position;
        public Vector3 Position
        {
            set => SetProperty(ref _Position, value);
            get => _Position;
        }

        private Quaternion _Rotation;
        public Quaternion Rotation
        {
            set => SetProperty(ref _Rotation, value);
            get => _Rotation;
        }

        private ColliderShape _Shape;
        public ColliderShape Shape
        {
            set => SetProperty(ref _Shape, value);
            get => _Shape;
        }

        private float _Radius0;
        public float Radius0
        {
            set => SetProperty(ref _Radius0, value);
            get => _Radius0;
        }

        private float _Radius1;
        public float Radius1
        {
            set => SetProperty(ref _Radius1, value);
            get => _Radius1;
        }

        private float _Radius2;
        public float Radius2
        {
            set => SetProperty(ref _Radius2, value);
            get => _Radius2;
        }

        private bool _Inclusive;
        public bool Inclusive
        {
            set => SetProperty(ref _Inclusive, value);
            get => _Inclusive || _NodeName == null;
        }

        public MeshCollider(MeshGeometry parent)
        {
            Parent = parent;

            _Rotation = Quaternion.Identity;

            _Inclusive = true;
        }

        public MeshCollider(MeshGeometry parent, MeshCollider other)
        {
            Parent = parent;

            _Name = other._Name;
            _NodeName = other._NodeName;
            _Position = other._Position;
            _Rotation = other._Rotation;
            _Shape = other._Shape;
            _Radius0 = other._Radius0;
            _Radius1 = other._Radius1;
            _Radius2 = other._Radius2;
            _Inclusive = other._Inclusive;
        }

        internal MeshCollider(MeshGeometry parent, XmlNode node)
        {
            Parent = parent;

            if (node.LocalName != "collider") throw new XmlException();

            _Name = node.ReadAttributeString("name");
            _NodeName = node.ReadAttributeString("nodeName");
            _Position = node.ReadAttributeVector3("position");
            _Rotation = node.ReadAttributeQuaternion("rotation");
            _Shape = node.ReadAttributeEnum<ColliderShape>("shape");
            _Radius0 = node.ReadAttributeFloat("radius0");
            _Radius1 = node.ReadAttributeFloat("radius1");
            _Radius2 = node.ReadAttributeFloat("radius2");
            _Inclusive = node.ReadAttributeBool("inclusive", true);
        }

        private int GetCapsuleAxis(in ABoundingBox aabb)
        {
            var dx = aabb.Maximum.X - aabb.Minimum.X;
            var dy = aabb.Maximum.Y - aabb.Minimum.Y;
            var dz = aabb.Maximum.Z - aabb.Minimum.Z;

            if (dx > dy)
            {
                if (dx > dz) return 0;
                else return 2;
            }
            else
            {
                if (dy > dz) return 1;
                else return 2;
            }
        }

        public void Reset()
        {
            ABoundingBox aabb;
            if (_NodeName != null)
            {
                if (!Parent.Origin.GetNodeAABB(_NodeName, null, 0, _Inclusive, out _, out aabb)) return;
            }
            else aabb = Parent.Origin.AABB;

            Position = aabb.Center;

            switch (_Shape)
            {
                case ColliderShape.Box:
                    {
                        var radius = (aabb.Maximum - aabb.Minimum) * 0.5f;
                        Rotation = Quaternion.Identity;
                        Radius0 = radius.X;
                        Radius1 = radius.Y;
                        Radius2 = radius.Z;
                    }
                    break;
                case ColliderShape.Sphere:
                    Rotation = Quaternion.Identity;
                    Radius0 = aabb.Radius;
                    Radius1 = 0;
                    Radius2 = 0;
                    break;
                case ColliderShape.Capsule:
                    switch (GetCapsuleAxis(aabb))
                    {
                        case 0:
                            Rotation = new Quaternion(0, 1, 0, 1);
                            Radius0 = Vector2.Distance(new Vector2(aabb.Minimum.Y, aabb.Minimum.Z), new Vector2(aabb.Maximum.Y, aabb.Maximum.Z)) * 0.5f;
                            Radius1 = (aabb.Maximum.X - aabb.Minimum.X) * 0.5f;
                            break;
                        case 1:
                            Rotation = new Quaternion(1, 0, 0, 1);
                            Radius0 = Vector2.Distance(new Vector2(aabb.Minimum.X, aabb.Minimum.Z), new Vector2(aabb.Maximum.X, aabb.Maximum.Z)) * 0.5f;
                            Radius1 = (aabb.Maximum.Y - aabb.Minimum.Y) * 0.5f;
                            break;
                        case 2:
                            Rotation = Quaternion.Identity;
                            Radius0 = Vector2.Distance(new Vector2(aabb.Minimum.X, aabb.Minimum.Y), new Vector2(aabb.Maximum.X, aabb.Maximum.Y)) * 0.5f;
                            Radius1 = (aabb.Maximum.Z - aabb.Minimum.Z) * 0.5f;
                            break;
                    }
                    Radius2 = 0;
                    break;
            }
        }

        private void GetTransform(Instance instance, out Matrix4x4 transform)
        {
            var node = _NodeName != null ? instance.Geometry.FindNode(_NodeName) : null;

            transform = node != null ? instance.GetNodeTransform(node) : Matrix4x4.Identity;
            if (_Position != Vector3.Zero) transform.Translation += _Position;
            if (!_Rotation.IsIdentity) transform = Matrix4x4.CreateFromQuaternion(_Rotation) * transform;
        }

        internal void AddCollider(Instance instance, in Matrix4x4 instanceTransform, Drawing.Collider result)
        {
            GetTransform(instance, out var transform);
            transform *= instanceTransform;

            switch (_Shape)
            {
                case ColliderShape.Sphere:
                    {
                        var sphere = new BoundingSphere(Vector3.Zero, _Radius0);
                        BoundingSphere.Transform(sphere, transform, out sphere);
                        result.Add(sphere);
                    }
                    break;
                case ColliderShape.Capsule:
                    {
                        var capsule = new BoundingCapsule(new Vector3(0, 0, -_Radius1), new Vector3(0, 0, _Radius1), _Radius0);
                        BoundingCapsule.Transform(capsule, transform, out capsule); 
                        result.Add(capsule);
                    }
                    break;
                case ColliderShape.Box:
                    {
                        var box = new OBoundingBox(Vector3.Zero, new Vector3(_Radius0, _Radius0, _Radius2), Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ);
                        OBoundingBox.Transform(box, transform, out box);
                        result.Add(box);
                    }
                    break;
            }
        }
        
        internal void Draw(Graphics graphics, Instance instance)
        {
            GetTransform(instance, out var transform);

            graphics.PushTransform();
            graphics.Transform(transform);

            switch (_Shape)
            {
                case ColliderShape.Sphere:
                    {
                        var rect = new Rectangle(-_Radius0, -_Radius0, _Radius0 * 2, _Radius0 * 2);

                        graphics.DrawCircle(rect, false);
                        graphics.RotateX(MathUtil.PiOverTwo);
                        graphics.DrawCircle(rect, false);
                        graphics.RotateY(MathUtil.PiOverTwo);
                        graphics.DrawCircle(rect, false);
                    }
                    break;
                case ColliderShape.Capsule:
                    {
                        var s0 = new Vector3(0, 0, -_Radius1);
                        var s1 = new Vector3(0, 0, _Radius1);
                        var rect = new Rectangle(-_Radius0, -_Radius0, _Radius0 * 2, _Radius0 * 2);

                        graphics.PushTransform();
                        graphics.Translate(s0);
                        graphics.DrawCircle(rect, false);
                        graphics.RotateX(-MathUtil.PiOverTwo);
                        graphics.DrawArc(rect, 0, MathUtil.Pi, false);
                        graphics.RotateY(MathUtil.PiOverTwo);
                        graphics.DrawArc(rect, 0, MathUtil.Pi, false);
                        graphics.ResetTransform();

                        graphics.Translate(s1);
                        graphics.DrawCircle(rect, false);
                        graphics.RotateX(MathUtil.PiOverTwo);
                        graphics.DrawArc(rect, 0, MathUtil.Pi, false);
                        graphics.RotateY(MathUtil.PiOverTwo);
                        graphics.DrawArc(rect, 0, MathUtil.Pi, false);
                        graphics.ResetTransform();

                        graphics.PopTransform();

                        graphics.DrawLine(new Vector3(-_Radius0, 0, s0.Z), new Vector3(-_Radius0, 0, s1.Z));
                        graphics.DrawLine(new Vector3(_Radius0, 0, s0.Z), new Vector3(_Radius0, 0, s1.Z));
                        graphics.DrawLine(new Vector3(0, -_Radius0, s0.Z), new Vector3(0, -_Radius0, s1.Z));
                        graphics.DrawLine(new Vector3(0, _Radius0, s0.Z), new Vector3(0, _Radius0, s1.Z));
                    }
                    break;
                case ColliderShape.Box:
                    {
                        var extent = new Vector3(_Radius0, _Radius1, _Radius2);

                        var box = new ABoundingBox(-extent, extent);

                        var command = new StreamRenderCommand(graphics, PrimitiveMode.Lines);

                        foreach (var corner in box.GetCorners()) command.AddVertex(new FVertex(corner));

                        command.AddIndex(0);
                        command.AddIndex(1);
                        command.AddIndex(1);
                        command.AddIndex(2);
                        command.AddIndex(2);
                        command.AddIndex(3);
                        command.AddIndex(3);
                        command.AddIndex(0);
                        command.AddIndex(4);
                        command.AddIndex(5);
                        command.AddIndex(5);
                        command.AddIndex(6);
                        command.AddIndex(6);
                        command.AddIndex(7);
                        command.AddIndex(7);
                        command.AddIndex(4);
                        command.AddIndex(0);
                        command.AddIndex(4);
                        command.AddIndex(1);
                        command.AddIndex(5);
                        command.AddIndex(2);
                        command.AddIndex(6);
                        command.AddIndex(3);
                        command.AddIndex(7);

                        graphics.Command(command);
                    }
                    break;
            }
            graphics.PopTransform();
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("collider");
            writer.WriteAttribute("name", _Name);
            writer.WriteAttribute("nodeName", _NodeName);
            writer.WriteAttribute("position", _Position);
            writer.WriteAttribute("rotation", _Rotation);
            writer.WriteAttribute("shape", _Shape);
            writer.WriteAttribute("radius0", _Radius0);
            writer.WriteAttribute("radius1", _Radius1);
            writer.WriteAttribute("radius2", _Radius2);
            writer.WriteAttribute("inclusive", _Inclusive, true);
            writer.WriteEndElement();
        }
    }
}
