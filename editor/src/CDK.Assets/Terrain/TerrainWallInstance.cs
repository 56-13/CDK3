using System;
using System.Numerics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Terrain
{
    public class TerrainWallInstance : AssetElement
    {
        public TerrainWall Wall { private set; get; }
        public override AssetElement GetParent() => Wall;

        private int _Loop;
        public int Loop
        {
            set
            {
                if (SetProperty(ref _Loop, value)) Update();
            }
            get => _Loop;
        }

        public AssetElementList<TerrainWallInstancePoint> Points { private set; get; }

        private List<Drawing.Meshing.Instance> _meshInstances;
        private bool _updateReserved;

        public TerrainWallInstance(TerrainWall wall)
        {
            Wall = wall;

            Points = new AssetElementList<TerrainWallInstancePoint>(this);

            _meshInstances = new List<Drawing.Meshing.Instance>();

            Wall.Parent.PropertyChanged += Asset_PropertyChanged;
        }

        internal TerrainWallInstance(TerrainAsset asset, XmlNode node)
        {
            Wall = asset.Walls[node.ReadAttributeInt("wall")];

            Points = new AssetElementList<TerrainWallInstancePoint>(this);

            _Loop = node.ReadAttributeInt("loop");

            foreach (XmlNode subnode in node.ChildNodes) Points.Add(new TerrainWallInstancePoint(this, subnode));

            _meshInstances = new List<Drawing.Meshing.Instance>();
            Update();

            Wall.Parent.PropertyChanged += Asset_PropertyChanged;
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Grid") Update();
        }

        public void MoveZ(float delta)
        {
            _updateReserved = true;
            foreach (var p in Points) p.Z += delta;
            _updateReserved = false;
            Update();
        }

        public void Flip()
        {
            var points = new TerrainWallInstancePoint[Points.Count];
            Points.CopyTo(points, 0);
            Points.Clear();
            for (var i = points.Length - 1; i >= 0; i--) Points.Add(points[i]);
            Update();
        }

        private void GetBoneTransform(int i, bool roundTrip, ref Matrix4x4 m)
        {
            Vector3 p, dx;

            var grid = Wall.Parent.Grid;

            if (Points.Count == 1)
            {
                var ip0 = Points[0];
                p = new Vector3(ip0.X * grid, ip0.Y * grid, ip0.Z);
                if (i == 0) p.X -= grid * 0.5f;
                else p.X += grid * 0.5f;
                dx = Vector3.UnitX;
            }
            else if (!roundTrip && i == 0)
            {
                var ip0 = Points[0];
                var ip1 = Points[1];
                var p0 = new Vector3(ip0.X * grid, ip0.Y * grid, ip0.Z);
                var p1 = new Vector3(ip1.X * grid, ip1.Y * grid, ip1.Z);
                p = p0 * 1.5f - p1 * 0.5f;
                dx = Vector3.Normalize(p1 - p0);
            }
            else if (!roundTrip && i == Points.Count)
            {
                var ip0 = Points[Points.Count - 2];
                var ip1 = Points[Points.Count - 1];
                var p0 = new Vector3(ip0.X * grid, ip0.Y * grid, ip0.Z);
                var p1 = new Vector3(ip1.X * grid, ip1.Y * grid, ip1.Z);
                p = p1 * 1.5f - p0 * 0.5f;
                dx = Vector3.Normalize(p1 - p0);
            }
            else
            {
                var ip0 = Points[(i + Points.Count - 1) % Points.Count];
                var ip1 = Points[i % Points.Count];
                var p0 = new Vector3(ip0.X * grid, ip0.Y * grid, ip0.Z);
                var p1 = new Vector3(ip1.X * grid, ip1.Y * grid, ip1.Z);
                p = (p0 + p1) * 0.5f;
                dx = Vector3.Normalize(p1 - p0);
            }
            
            var dy = Vector3.Cross(Vector3.UnitZ, dx);
            var dz = Vector3.Cross(dx, dy);

            m.M11 = dx.X;
            m.M12 = dx.Y;
            m.M13 = dx.Z;
            m.M21 = dy.X;
            m.M22 = dy.Y;
            m.M23 = dy.Z;
            m.M31 = dz.X;
            m.M32 = dz.Y;
            m.M33 = dz.Z;
            m.M41 = p.X + grid * 0.5f;
            m.M42 = p.Y + grid * 0.5f;
            m.M43 = p.Z;
        }

        internal void Update()
        {
            if (Points.Count == 0) throw new InvalidOperationException();

            if (_updateReserved) return;

            if (Wall.Bones.Count < 2 || Wall.Selection.Geometry == null)
            {
                _meshInstances.Clear();
                return;
            }

            var denom = Wall.Bones.Count - 1;
            var first = _Loop % denom;
            var num = Points.Count + first;
            var meshCount = num / denom;
            if (num % denom != 0) meshCount++;

            while (_meshInstances.Count > meshCount) _meshInstances.RemoveAt(_meshInstances.Count - 1);
            for (var i = 0; i < _meshInstances.Count; i++)
            {
                var meshInstance = _meshInstances[i];
                Wall.Selection.UpdateInstance(ref meshInstance);
                _meshInstances[i] = meshInstance;
            }
            for (var i = _meshInstances.Count; i < meshCount; i++)
            {
                Drawing.Meshing.Instance meshInstance = null;
                Wall.Selection.UpdateInstance(ref meshInstance);
                _meshInstances.Add(meshInstance);
            }

            var roundTrip = false;
            if (Points.Count >= 4)
            {
                var p0 = Points[Points.Count - 1];
                var p1 = Points[0];
                var d = Math.Max(Math.Abs(p1.X - p0.X), Math.Abs(p1.Y - p0.Y));
                roundTrip = d == 1;
            }

            var meshIndex = 0;
            var boneIndex = 0;
            var m = Matrix4x4.Identity;

            GetBoneTransform(0, roundTrip, ref m);
            for (var i = 0; i <= first; i++)
            {
                _meshInstances[meshIndex].SetCustomTransform(Wall.Bones[boneIndex], m);
                boneIndex++;
            }
            for (var i = 1; i <= Points.Count; i++)
            {
                GetBoneTransform(i, roundTrip, ref m);

                _meshInstances[meshIndex].SetCustomTransform(Wall.Bones[boneIndex], m);

                if (++boneIndex >= Wall.Bones.Count)
                {
                    boneIndex = 0;
                    if (++meshIndex < meshCount)
                    {
                        _meshInstances[meshIndex].SetCustomTransform(Wall.Bones[boneIndex], m);
                        boneIndex++;
                    }
                }
            }
            if (boneIndex != 0)
            {
                for (var i = boneIndex; i < Wall.Bones.Count; i++)
                {
                    _meshInstances[meshIndex].SetCustomTransform(Wall.Bones[i], m);
                }
            }
        }

        internal void Draw(Graphics graphics, InstanceLayer layer, float progress, int random)
        {
            foreach (var meshInstance in _meshInstances)
            {
                var duration = meshInstance.Duration;

                if (duration != 0) meshInstance.Progress = progress % duration;

                meshInstance.Draw(graphics, layer, progress, random);
            }
        }

        internal void DrawCursor(Graphics graphics, TerrainWallInstancePoint selectedPoint, in Color4 color0, in Color4 color1, in Color4 color2, (int x, int y)[] inter)
        {
            graphics.Material.DepthTest = false;

            var command = new StreamRenderCommand(graphics, PrimitiveMode.Lines);
            command.State.Material.Shader = MaterialShader.NoLight;

            var capacity = Points.Count;
            if (inter != null) capacity += inter.Length;
            var pointInstances = new List<VertexArrayInstance>(capacity);

            var grid = Wall.Parent.Grid;

            var m = Matrix4x4.CreateScale(0.5f);

            for (var i = 0; i < Points.Count; i++)
            {
                var p = Points[i];

                Color4 color;
                if (p == selectedPoint) color = color2;
                else if (i == 0 || i == Points.Count - 1) color = color1;
                else color = color0;

                var x = (p.X + 0.5f) * grid;
                var y = (p.Y + 0.5f) * grid;

                command.AddVertex(new FVertex(new Vector3(x, y, p.Z), Color4.Black));

                m.M41 = x;
                m.M42 = y;
                m.M43 = p.Z;

                pointInstances.Add(new VertexArrayInstance(m, color));
            }
            if (inter != null)
            {
                foreach (var p in inter)
                {
                    var x = (p.x + 0.5f) * grid;
                    var y = (p.y + 0.5f) * grid;
                    command.AddVertex(new FVertex(new Vector3(x, y, selectedPoint.Z), Color4.Black));

                    m.M41 = x;
                    m.M42 = y;
                    m.M43 = selectedPoint.Z;

                    pointInstances.Add(new VertexArrayInstance(m, color2));
                }
            }
            if (command.VertexCount >= 2)
            {
                for (var i = 0; i < command.VertexCount - 1; i++)
                {
                    command.AddIndex(i);
                    command.AddIndex(i + 1);
                }
                graphics.Command(command);
            }

            var sphere = VertexArrays.GetSphere(0, Vector3.Zero, 8, Rectangle.ZeroToOne, out var aabb);
            graphics.DrawVertices(sphere, PrimitiveMode.Triangles, aabb, pointInstances);

            graphics.Material.DepthTest = true;
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("wallInstance");
            writer.WriteAttribute("wall", Wall.Index);
            writer.WriteAttribute("loop", _Loop);
            foreach (var p in Points) p.Save(writer);
            writer.WriteEndElement();
        }

        internal void Build(BinaryWriter writer)
        {
            writer.Write((byte)_Loop);
            foreach (var p in Points) p.Build(writer);
        }
    }
}
