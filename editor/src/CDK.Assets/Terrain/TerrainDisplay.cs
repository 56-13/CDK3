using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

using CDK.Drawing;

using Buffer = CDK.Drawing.Buffer;

namespace CDK.Assets.Terrain
{
    public class TerrainDisplay : IDisposable
    {
        public TerrainAsset Asset { private set; get; }
        private VertexArray _vertices;
        private Buffer _vertexBuffer;
        private Texture _ambientOcclusionMap;

        private struct SurfaceDisplay
        {
            public Texture IntensityMap;
            public VertexArray Vertices;
        }
        private Dictionary<TerrainSurface, SurfaceDisplay> _surfaceVertices;
        private Buffer _waterVertexBuffer;
        private Dictionary<TerrainWater, VertexArray> _waterVertices;

        public TerrainDisplay(TerrainAsset asset)
        {
            Asset = asset;

            _vertexBuffer = new Buffer(BufferTarget.ArrayBuffer);
            _vertices = new VertexArray(0, true, new VertexLayout[]
            {
                new VertexLayout(_vertexBuffer, 0, 3, VertexAttribType.Float, false, 18, 0, 0, true)
            });

            _surfaceVertices = new Dictionary<TerrainSurface, SurfaceDisplay>();

            _waterVertexBuffer = new Buffer(BufferTarget.ArrayBuffer);

            _waterVertices = new Dictionary<TerrainWater, VertexArray>();

            Reset();
        }

        public void Dispose()
        {
            _vertices.Dispose();
            _vertexBuffer.Dispose();
            _ambientOcclusionMap.Dispose();

            ClearSurfaces();

            _waterVertexBuffer.Dispose();
            ClearWaters();
        }

        internal void Reset()
        {
            UpdateAltitude(false);

            var swidth = Asset.Width * Asset.VertexCell * Asset.SurfaceCell;
            var sheight = Asset.Height * Asset.VertexCell * Asset.SurfaceCell;

            _ambientOcclusionMap?.Dispose();
            _ambientOcclusionMap = new Texture(new TextureDescription()
            {
                Width = swidth + 1,
                Height = sheight + 1,
                Format = RawFormat.R8,
                MinFilter = TextureMinFilter.Linear,
                MagFilter = TextureMagFilter.Linear
            });
            UpdateAmbientOcclusion(0, 0, swidth, sheight);

            ClearSurfaces();

            foreach (var surface in Asset.Surfaces)
            {
                AddSurface(surface);
            }

            ClearWaters();

            foreach (var water in Asset.Waters)
            {
                AddWater(water);
            }
            
            UpdateWater();
        }

        private bool CheckSurfaceFill(TerrainSurfaceInstances surfaceInstances, int vx, int vy)
        {
            var minsx = vx * Asset.SurfaceCell;
            var minsy = vy * Asset.SurfaceCell;
            var maxsx = minsx + Asset.SurfaceCell;
            var maxsy = minsy + Asset.SurfaceCell;
            for (var sy = minsy; sy <= maxsy; sy++)
            {
                for (var sx = minsx; sx <= maxsx; sx++)
                {
                    if (surfaceInstances[sx, sy].TextureValue != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void AddIndex(VertexIndexData indexUpdate, int vx, int vy, bool fill)
        {
            var vwidth = Asset.Width * Asset.VertexCell;

            if (fill)
            {
                var vi = vy * (vwidth + 1) + vx;

                if (Asset.IsZQuad(vx, vy))
                {
                    indexUpdate.Add(vi + 0);
                    indexUpdate.Add(vi + 1);
                    indexUpdate.Add(vi + vwidth + 1);
                    indexUpdate.Add(vi + 1);
                    indexUpdate.Add(vi + vwidth + 2);
                    indexUpdate.Add(vi + vwidth + 1);
                }
                else
                {
                    indexUpdate.Add(vi + 0);
                    indexUpdate.Add(vi + 1);
                    indexUpdate.Add(vi + vwidth + 2);
                    indexUpdate.Add(vi + 0);
                    indexUpdate.Add(vi + vwidth + 2);
                    indexUpdate.Add(vi + vwidth + 1);
                }
            }
            else
            {
                for (var i = 0; i < 6; i++) indexUpdate.Add(0);
            }
        }

        internal void UpdateAltitude(bool withSurface = true)
        {
            var vwidth = Asset.Width * Asset.VertexCell;
            var vheight = Asset.Height * Asset.VertexCell;

            var vertices = new BufferData<VertexN>((vwidth + 1) * (vheight + 1));

            for (var vy = 0; vy <= vheight; vy++)
            {
                for (var vx = 0; vx <= vwidth; vx++)
                {
                    vertices.Add(new VertexN(new Vector3(vx, vy, Asset.GetAltitude(vx, vy)), Asset.GetNormal(vx, vy)));
                }
            }
            _vertexBuffer.Upload(vertices, BufferUsageHint.DynamicDraw);

            var indices = new VertexIndexData(_vertexBuffer.Count, 6 * vwidth * vheight);

            for (var vy = 0; vy < vheight; vy++)
            {
                for (var vx = 0; vx < vwidth; vx++)
                {
                    AddIndex(indices, vx, vy, true);
                }
            }

            _vertices.IndexBuffer.Upload(indices, BufferUsageHint.DynamicDraw);

            if (withSurface)
            {
                foreach (var i in _surfaceVertices)
                {
                    indices.Clear();

                    var surfaceInstances = Asset.GetSurfaceInstances(i.Key);

                    for (var vy = 0; vy < vheight; vy++)
                    {
                        for (var vx = 0; vx < vwidth; vx++)
                        {
                            var fill = CheckSurfaceFill(surfaceInstances, vx, vy);

                            AddIndex(indices, vx, vy, fill);
                        }
                    }
                    i.Value.Vertices.IndexBuffer.Upload(indices, BufferUsageHint.DynamicDraw);
                }
            }
        }

        internal void UpdateAltitude(int vminx, int vminy, int vmaxx, int vmaxy, bool withSurface = true)
        {
            var vwidth = Asset.Width * Asset.VertexCell;
            var vheight = Asset.Height * Asset.VertexCell;

            //노멀은 주변 1칸에 영향
            vminx = Math.Max(vminx - 1, 0);
            vminy = Math.Max(vminy - 1, 0);
            vmaxx = Math.Min(vmaxx + 1, vwidth);
            vmaxy = Math.Min(vmaxy + 1, vheight);

            var vertices = new BufferData<VertexN>(vmaxx - vminx + 1);

            for (var vy = vminy; vy <= vmaxy; vy++)
            {
                vertices.Clear();

                for (var vx = vminx; vx <= vmaxx; vx++)
                {
                    vertices.Add(new VertexN(new Vector3(vx, vy, Asset.GetAltitude(vx, vy)), Asset.GetNormal(vx, vy)));
                }

                _vertexBuffer.UploadSub(vertices, vy * (vwidth + 1) + vminx);
            }

            var indices = new VertexIndexData(_vertexBuffer.Count, 6 * (vmaxx - vminx));

            for (var vy = vminy; vy < vmaxy; vy++)
            {
                indices.Clear();

                for (var vx = vminx; vx < vmaxx; vx++)
                {
                    AddIndex(indices, vx, vy, true);
                }

                _vertices.IndexBuffer.UploadSub(indices, 6 * (vy * vwidth + vminx));
            }
            if (withSurface)
            {
                foreach (var i in _surfaceVertices)
                {
                    var surfaceInstances = Asset.GetSurfaceInstances(i.Key);

                    for (var vy = vminy; vy < vmaxy; vy++)
                    {
                        indices.Clear();

                        for (var vx = vminx; vx < vmaxx; vx++)
                        {
                            var fill = CheckSurfaceFill(surfaceInstances, vx, vy);

                            AddIndex(indices, vx, vy, fill);
                        }

                        i.Value.Vertices.IndexBuffer.UploadSub(indices, 6 * (vy * vwidth + vminx));
                    }
                }
            }
        }

        internal void UpdateAmbientOcclusion(int sminx, int sminy, int smaxx, int smaxy)
        {
            var raw = new byte[(smaxy - sminy + 1) * (smaxx - sminx + 1)];

            var i = 0;
            for (var sy = sminy; sy <= smaxy; sy++)
            {
                for (var sx = sminx; sx <= smaxx; sx++)
                {
                    raw[i++] = Asset.AmbientOcclusions[sx, sy];
                }
            }

            _ambientOcclusionMap.UploadSub(raw, 0, sminx, sminy, 0, smaxx - sminx + 1, smaxy - sminy + 1, 0);
        }

        internal void ClearSurfaces()
        {
            foreach (var i in _surfaceVertices)
            {
                i.Value.IntensityMap.Dispose();
                i.Value.Vertices.Dispose();
            }
            _surfaceVertices.Clear();
        }

        internal void RemoveSurface(TerrainSurface surface)
        {
            if (_surfaceVertices.TryGetValue(surface, out var display))
            {
                display.IntensityMap.Dispose();
                display.Vertices.Dispose();
                _surfaceVertices.Remove(surface);
            }
        }
        
        internal void AddSurface(TerrainSurface surface)
        {
            var vwidth = Asset.Width * Asset.VertexCell;
            var vheight = Asset.Height * Asset.VertexCell;
            var swidth = vwidth * Asset.SurfaceCell;
            var sheight = vheight * Asset.SurfaceCell;

            if (!_surfaceVertices.ContainsKey(surface)) {
                var instances = Asset.GetSurfaceInstances(surface);

                var display = new SurfaceDisplay()
                {
                    IntensityMap = new Texture(new TextureDescription(){
                        Width = swidth + 1, 
                        Height = sheight + 1, 
                        Format = RawFormat.R8,
                        MinFilter = TextureMinFilter.Linear,
                        MagFilter = TextureMagFilter.Linear
                    }),
                    Vertices = new VertexArray(0, true, new VertexLayout[]
                    {
                        new VertexLayout(_vertexBuffer, 0, 3, VertexAttribType.Float, false, 18, 0, 0, true),
                        new VertexLayout(_vertexBuffer, 1, 3, VertexAttribType.HalfFloat, false, 18, 12, 0, true)
                    })
                };

                UpdateSurface(instances, display);

                _surfaceVertices.Add(surface, display);
            }
        }
        private void UpdateSurfaceMap(TerrainSurfaceInstances instances, SurfaceDisplay display, int sminx, int sminy, int smaxx, int smaxy)
        {
            var raw = new byte[(smaxx - sminx + 1) * (smaxy - sminy + 1)];
            var i = 0;
            for (var sy = sminy; sy <= smaxy; sy++)
            {
                for (var sx = sminx; sx <= smaxx; sx++)
                {
                    raw[i++] = instances[sx, sy].TextureValue;
                }
            }

            display.IntensityMap.UploadSub(raw, 0, sminx, sminy, 0, smaxx - sminx + 1, smaxy - sminy + 1, 0);
        }

        private void UpdateSurface(TerrainSurfaceInstances instances, SurfaceDisplay display)
        {
            var vwidth = Asset.Width * Asset.VertexCell;
            var vheight = Asset.Height * Asset.VertexCell;
            var swidth = vwidth * Asset.SurfaceCell;
            var sheight = vheight * Asset.SurfaceCell;

            var indices = new VertexIndexData(_vertexBuffer.Count, 6 * vwidth * vheight);

            UpdateSurfaceMap(instances, display, 0, 0, swidth, sheight);

            for (var vy = 0; vy < vheight; vy++)
            {
                for (var vx = 0; vx < vwidth; vx++)
                {
                    var fill = CheckSurfaceFill(instances, vx, vy);

                    AddIndex(indices, vx, vy, fill);
                }
            }
            display.Vertices.IndexBuffer.Upload(indices, BufferUsageHint.DynamicDraw);
        }

        internal void UpdateSurfaces()
        {
            foreach (var i in _surfaceVertices)
            {
                var instances = Asset.GetSurfaceInstances(i.Key);

                UpdateSurface(instances, i.Value);
            }
        }

        internal void UpdateSurfaces(int sminx, int sminy, int smaxx, int smaxy)
        {
            var vwidth = Asset.Width * Asset.VertexCell;
            var vheight = Asset.Height * Asset.VertexCell;

            var vminx = Math.Max(sminx / Asset.SurfaceCell - 1, 0);
            var vminy = Math.Max(sminy / Asset.SurfaceCell - 1, 0);
            var vmaxx = Math.Min(smaxx / Asset.SurfaceCell + 1, vwidth);
            var vmaxy = Math.Min(smaxy / Asset.SurfaceCell + 1, vheight);

            var indices = new VertexIndexData(_vertexBuffer.Count, 6 * (vmaxx - vminx));
            
            foreach (var i in _surfaceVertices)
            {
                var instances = Asset.GetSurfaceInstances(i.Key);

                UpdateSurfaceMap(instances, i.Value, sminx, sminy, smaxx, smaxy);

                for (var vy = vminy; vy < vmaxy; vy++)
                {
                    indices.Clear();
                    for (var vx = vminx; vx < vmaxx; vx++)
                    {
                        var fill = CheckSurfaceFill(instances, vx, vy);

                        AddIndex(indices, vx, vy, fill);
                    }
                    i.Value.Vertices.IndexBuffer.UploadSub(indices, 6 * (vy * vwidth + vminx));
                }
            }
        }

        internal void ClearWaters()
        {
            foreach (var i in _waterVertices)
            {
                i.Value.Dispose();
            }
            _waterVertices.Clear();
        }

        internal void RemoveWater(TerrainWater water)
        {
            if (_waterVertices.TryGetValue(water, out var vertices))
            {
                vertices.Dispose();
                _waterVertices.Remove(water);
            }
        }

        internal void AddWater(TerrainWater water)
        {
            if (!_waterVertices.ContainsKey(water))
            {
                var vertexLayouts = new VertexLayout[]
                {
                    new VertexLayout(_waterVertexBuffer, 0, 3, VertexAttribType.Float, false, 12, 0, 0, true)
                };
                var vertices = new VertexArray(0, true, vertexLayouts);

                _waterVertices.Add(water, vertices);
            }
        }

        internal void UpdateWater()
        {
            {
                var vertices = new BufferData<Vector3>(Asset.Width * Asset.Height * 4);

                for (var y = 0; y < Asset.Height; y++)
                {
                    for (var x = 0; x < Asset.Width; x++)
                    {
                        var i = Asset.GetWaterInstance(x, y);

                        var z = i != null ? i.Altitude : 0;

                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z));
                    }
                }
                _waterVertexBuffer.Upload(vertices, BufferUsageHint.DynamicDraw);
            }
            
            var indices = new VertexIndexData(_waterVertexBuffer.Count, Asset.Width * Asset.Height * 6);

            foreach (var vertices in _waterVertices)
            {
                indices.Clear();

                for (var y = 0; y < Asset.Height; y++)
                {
                    for (var x = 0; x < Asset.Width; x++)
                    {
                        var wi = Asset.GetWaterInstance(x, y);

                        if (wi != null && Asset.WaterEnabled(wi, x, y) && vertices.Key == wi.Water)
                        {
                            var vi = (y * Asset.Width + x) * 4;

                            indices.Add(vi);
                            indices.Add(vi + 1);
                            indices.Add(vi + 2);
                            indices.Add(vi + 1);
                            indices.Add(vi + 3);
                            indices.Add(vi + 2);
                        }
                        else
                        {
                            for (var j = 0; j < 6; j++)
                            {
                                indices.Add(0);
                            }
                        }
                    }
                }
                vertices.Value.IndexBuffer.Upload(indices, BufferUsageHint.DynamicDraw);
            }
        }

        internal void UpdateWater(int minx, int miny, int maxx, int maxy)
        {
            {
                var vertices = new BufferData<Vector3>((maxx - minx) * 4);

                for (var y = miny; y < maxy; y++)
                {
                    vertices.Clear();

                    for (var x = minx; x < maxx; x++)
                    {
                        var i = Asset.GetWaterInstance(x, y);

                        var z = i != null ? i.Altitude : 0;

                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z));
                    }

                    _waterVertexBuffer.UploadSub(vertices, (y * Asset.Width + minx) * 4);
                }
            }
            var indices = new VertexIndexData(_waterVertexBuffer.Count, (maxx - minx) * 6);

            foreach (var vertices in _waterVertices)
            {
                for (var y = miny; y < maxy; y++)
                {
                    indices.Clear();

                    for (var x = minx; x < maxx; x++)
                    {
                        var wi = Asset.GetWaterInstance(x, y);

                        if (wi != null && Asset.WaterEnabled(wi, x, y) && vertices.Key == wi.Water)
                        {
                            var vi = (y * Asset.Width + x) * 4;

                            indices.Add(vi);
                            indices.Add(vi + 1);
                            indices.Add(vi + 2);
                            indices.Add(vi + 1);
                            indices.Add(vi + 3);
                            indices.Add(vi + 2);
                        }
                        else
                        {
                            for (var j = 0; j < 6; j++)
                            {
                                indices.Add(0);
                            }
                        }
                    }
                    vertices.Value.IndexBuffer.UploadSub(indices, (y * Asset.Width + minx) * 6);
                }
            }
        }

        private void DrawShadow(Graphics graphics)
        {
            var target = graphics.Target;
            var state = graphics.State;
            var world = graphics.World;

            graphics.Command((GraphicsApi api) =>
            {
                target.Focus(api);

                api.ApplyBlendMode(BlendMode.None);
                api.ApplyDepthMode(DepthMode.ReadWrite);
                api.ApplyCullMode(CullMode.Back);

                var shader = ResourceManager.Instance.TerrainShader;

                shader.DrawShadow(api, state, world, Asset, _vertices);
            });
        }

        private void DrawSurface(Graphics graphics, float progress, int random)
        {
            var target = graphics.Target;
            var state = graphics.State;
            var world = graphics.World;
            var color = graphics.Color;

            graphics.Command((GraphicsApi api) =>
            {
                target.Focus(api);

                api.ApplyBlendMode(BlendMode.None);
                api.ApplyDepthMode(DepthMode.ReadWrite);
                api.ApplyCullMode(CullMode.Back);

                var shader = ResourceManager.Instance.TerrainShader;

                state.Material.Shader = MaterialShader.NoLight;
                state.Material.Color = Color4.Black;

                shader.DrawSurface(api, state, world, color, Asset, null, _vertices, null, null);

                api.ApplyBlendMode(BlendMode.Add);
                api.ApplyDepthMode(DepthMode.Read);

                foreach (var i in _surfaceVertices)
                {
                    state.Material = i.Key.Material.GetMaterial(progress, random);

                    shader.DrawSurface(api, state, world, color, Asset, i.Key, i.Value.Vertices, _ambientOcclusionMap, i.Value.IntensityMap);
                }
            });
        }

        private void DrawWater(Graphics graphics, float progress, int random)
        {
            if (_waterVertices.Count == 0) return;

            var target = graphics.Target;
            var state = graphics.State;
            var world = graphics.World;
            var color = graphics.Color;

            graphics.Command((GraphicsApi api) =>
            {
                var sampleTargetDesc = new RenderTargetDescription()
                {
                    Width = target.Width,
                    Height = target.Height,
                    Attachments = new RenderTargetAttachmentDescription[]
                {
                    new RenderTargetAttachmentDescription()
                    {
                        Attachment = FramebufferAttachment.ColorAttachment0,
                        Format = target.GetFormat(FramebufferAttachment.ColorAttachment0),
                        Texture = true
                    },
                    new RenderTargetAttachmentDescription()
                    {
                        Attachment = FramebufferAttachment.DepthStencilAttachment,
                        Format = target.GetFormat(FramebufferAttachment.DepthStencilAttachment),
                        Texture = true
                    }
                }
                };
                var sampleTarget = RenderTargets.NewTemporary(sampleTargetDesc);
                target.Blit(sampleTarget, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
                var destMap = sampleTarget.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);
                var depthMap = sampleTarget.GetTextureAttachment(FramebufferAttachment.DepthStencilAttachment);

                target.Focus(api);

                api.ApplyBlendMode(BlendMode.None);
                api.ApplyDepthMode(DepthMode.Read);
                api.ApplyCullMode(CullMode.Back);

                var scale = (float)Math.Sqrt((world.M11 * world.M11) + (world.M22 * world.M22) + (world.M33 * world.M33));      //TODO:CHECK

                var shader = ResourceManager.Instance.TerrainShader;

                foreach (var i in _waterVertices)
                {
                    state.Material = i.Key.Material.GetMaterial(progress, random);
                    shader.DrawWater(api, state, world, color, Asset, i.Key, i.Value, progress, scale, destMap, depthMap);
                }

                ResourcePool.Instance.Remove(sampleTarget);
            });
        }

        private void DrawCursor(Graphics graphics)
        {
            var w = Asset.Width * Asset.Grid;
            var h = Asset.Height * Asset.Grid;
            var z = Asset.Altitude * Asset.Grid;

            graphics.Push();
            graphics.Material.Shader = MaterialShader.NoLight;
            graphics.Material.BlendMode = BlendMode.Alpha;
            graphics.DrawRect(new ZRectangle(0, 0, 0, w, h), false);
            graphics.DrawRect(new ZRectangle(0, 0, z, w, h), false);
            graphics.DrawLine(Vector3.Zero, new Vector3(0, 0, z));
            graphics.DrawLine(new Vector3(w, 0, 0), new Vector3(w, 0, z));
            graphics.DrawLine(new Vector3(0, h, 0), new Vector3(0, h, z));
            graphics.DrawLine(new Vector3(w, h, 0), new Vector3(w, h, z));
            graphics.Pop();
        }

        public void Draw(Graphics graphics, InstanceLayer layer, float progress, int random)
        {
            switch (layer)
            {
                case InstanceLayer.Shadow:
                    DrawShadow(graphics);
                    break;
                case InstanceLayer.None:
                    DrawSurface(graphics, progress, random);
                    DrawWater(graphics, progress, random);
                    break;
                case InstanceLayer.Base:
                    DrawSurface(graphics, progress, random);
                    break;
                case InstanceLayer.BlendBottom:
                    DrawWater(graphics, progress, random);
                    break;
                case InstanceLayer.Cursor:
                    DrawCursor(graphics);
                    return;
            }
            foreach (var wallInstance in Asset.WallInstances)
            {
                wallInstance.Draw(graphics, layer, progress, random);
            }
        }
    }
}
