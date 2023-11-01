using System;
using System.Collections.Generic;
using System.IO;

namespace CDK.Assets.Terrain
{
    partial class TerrainAsset
    {
        private static int _surfaceInstancesRecord;

        private static string GetSurfaceInstancesRecordPath(int record)
        {
            return Path.Combine(ResourceManager.Instance.DirectoryPath, $"surface-instances-record-{record}.tmp");
        }

        private static void DeleteSurfaceInstancesRecord(int record)
        {
            var path = GetSurfaceInstancesRecordPath(record);

            if (File.Exists(path)) File.Delete(path);
        }

        private int RecordSurfaceInstances()
        {
            return RecordSurfaceInstances(0, 0, _Width * _VertexCell * _SurfaceCell, _Height * _VertexCell * _SurfaceCell);
        }

        private int RecordSurfaceInstances(int sminx, int sminy, int smaxx, int smaxy)
        {
            Directory.CreateDirectory(ResourceManager.Instance.DirectoryPath);

            var path = GetSurfaceInstancesRecordPath(++_surfaceInstancesRecord);

            using (var fs = new FileStream(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(fs))
                {
                    writer.Write(smaxx - sminx + 1);
                    writer.Write(smaxy - sminy + 1);
                    writer.Write(_Surfaces.Count);

                    foreach (var surface in _Surfaces)
                    {
                        var surfaceInstances = _surfaceInstances[surface];

                        writer.Write(surface.Key);

                        writer.Write(!surfaceInstances.IsEmpty);

                        if (!surfaceInstances.IsEmpty)
                        {
                            for (var sy = sminy; sy <= smaxy; sy++)
                            {
                                for (var sx = sminx; sx <= smaxx; sx++)
                                {
                                    writer.Write(surfaceInstances[sx, sy].Intermediate);
                                    writer.Write(surfaceInstances[sx, sy].Current);
                                }
                            }
                        }
                    }
                }
            }
            return _surfaceInstancesRecord;
        }

        private void RestoreSurfaceInstances(int record, int sminx, int sminy, int smaxx, int smaxy)
        {
            var path = GetSurfaceInstancesRecordPath(record);

            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(fs))
                {
                    var swidth = reader.ReadInt32();
                    var sheight = reader.ReadInt32();
                    var surfaceCount = reader.ReadInt32();

                    for (var i = 0; i < surfaceCount; i++)
                    {
                        TerrainSurface surface = null;

                        var surfaceKey = reader.ReadInt32();

                        foreach (var otherSurface in _Surfaces)
                        {
                            if (otherSurface.Key == surfaceKey)
                            {
                                surface = otherSurface;
                                break;
                            }
                        }

                        var exists = reader.ReadBoolean();

                        if (surface != null)
                        {
                            var surfaceInstances = _surfaceInstances[surface];

                            for (var sy = sminy; sy <= smaxy; sy++)
                            {
                                for (var sx = sminx; sx <= smaxx; sx++)
                                {
                                    if (exists)
                                    {
                                        var intermediate = reader.ReadDouble();
                                        var current = reader.ReadDouble();

                                        surfaceInstances[sx, sy] = new TerrainSurfaceInstance(intermediate, current);
                                    }
                                    else
                                    {
                                        surfaceInstances[sx, sy] = TerrainSurfaceInstance.Empty;
                                    }
                                }
                                if (exists) reader.BaseStream.Position += (swidth - (smaxx - sminx + 1)) * 16;
                            }
                            if (exists) reader.BaseStream.Position += (sheight - (smaxy - sminy + 1)) * swidth * 16;
                        }
                        else
                        {
                            if (exists) reader.BaseStream.Position += swidth * sheight * 16;
                        }
                    }
                }
            }
        }

        private void RestoreSurfaceInstances(int record)
        {
            var path = GetSurfaceInstancesRecordPath(record);

            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(fs))
                {
                    var swidth = reader.ReadInt32();
                    var sheight = reader.ReadInt32();

                    if (swidth != _Width * _VertexCell * _SurfaceCell + 1 || sheight != _Height * _VertexCell * _SurfaceCell + 1)
                    {
                        throw new InvalidOperationException();
                    }
                    _surfaceInstances = new Dictionary<TerrainSurface, TerrainSurfaceInstances>();
                    foreach (var surface in _Surfaces)
                    {
                        _surfaceInstances.Add(surface, new TerrainSurfaceInstances(swidth, sheight));
                    }
                    var surfaceCount = reader.ReadInt32();

                    for (var i = 0; i < surfaceCount; i++)
                    {
                        TerrainSurface surface = null;

                        var surfaceKey = reader.ReadInt32();

                        foreach (var otherSurface in _Surfaces)
                        {
                            if (otherSurface.Key == surfaceKey)
                            {
                                surface = otherSurface;
                                break;
                            }
                        }
                        var exists = reader.ReadBoolean();

                        if (surface != null)
                        {
                            var surfaceInstances = _surfaceInstances[surface];

                            for (var sy = 0; sy < sheight; sy++)
                            {
                                for (var sx = 0; sx < swidth; sx++)
                                {
                                    if (exists)
                                    {
                                        var intermediate = reader.ReadDouble();
                                        var current = reader.ReadDouble();

                                        surfaceInstances[sx, sy] = new TerrainSurfaceInstance(intermediate, current);
                                    }
                                    else
                                    {
                                        surfaceInstances[sx, sy] = TerrainSurfaceInstance.Empty;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (exists) reader.BaseStream.Position += swidth * sheight * 16;
                        }
                    }
                }
            }
        }
    }
}
