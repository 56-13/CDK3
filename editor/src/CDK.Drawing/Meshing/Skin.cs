using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace CDK.Drawing.Meshing
{
    public interface ISkin
    {
        MaterialShader Shader { get; }
        InstanceBlendLayer BlendLayer { get; }
        BlendMode BlendMode { get; }
        bool ReceiveShadow { get; }
        bool ReceiveShadow2D { get; }
        Material GetMaterial(float progress, int random);
    }

    public interface ISkinContainer
    {
        ISkin this[int index] { get; }
    }

    public static class SkinUtil
    {
        public static bool Apply(ISkin source, Graphics graphics, InstanceLayer layer, float progress, int random, bool push)
        {
            IEnumerable<VertexArrayInstance> instances = null;
            return Apply(source, graphics, layer, progress, random, ref instances, push);
        }

        public static bool Apply(ISkin source, Graphics graphics, InstanceLayer layer, float progress, int random, ref IEnumerable<VertexArrayInstance> instances, bool push)
        {
            switch (layer)
            {
                case InstanceLayer.None:
                    {
                        var a = graphics.Color.A;
                        if ((source != null && source.Shader == MaterialShader.Distortion) || a <= 0) break;
                        if (push) graphics.Push();
                        var material = source?.GetMaterial(progress, random) ?? Material.Default;
                        material.ReceiveShadow = false;
                        material.ReceiveShadow2D = false;
                        if (a < 1 && material.BlendMode == BlendMode.None) material.BlendMode = BlendMode.Alpha;
                        graphics.Material = material;
                    }
                    return true;
                case InstanceLayer.Shadow:
                    {
                        if (source != null && !source.ReceiveShadow) break;
                        if (push) graphics.Push();
                        graphics.Material = source?.GetMaterial(progress, random) ?? Material.Default;
                    }
                    return true;
                case InstanceLayer.Shadow2D:
                    {
                        if (source == null || !source.ReceiveShadow2D || graphics.Color.A <= 0) break;
                        if (push) graphics.Push();
                        graphics.Material = source?.GetMaterial(progress, random) ?? Material.Default;
                    }
                    return true;
                case InstanceLayer.Base:
                    {
                        var a = graphics.Color.A;
                        if ((source != null && source.Shader == MaterialShader.Distortion) || a <= 0) break;
                        var blending = (source != null && source.BlendMode != BlendMode.None) || a < 1;          //default blend should be none
                        if (blending) break;
                        if (instances != null)
                        {
                            instances = instances.Where(i => i.Color.A >= 1);
                            if (!instances.Any()) break;
                        }
                        if (push) graphics.Push();
                        graphics.Material = source?.GetMaterial(progress, random) ?? Material.Default;
                    }
                    return true;
                case InstanceLayer.BlendBottom:
                case InstanceLayer.BlendMiddle:
                case InstanceLayer.BlendTop:
                    {
                        var a = graphics.Color.A;
                        if ((source != null && source.Shader == MaterialShader.Distortion) || a <= 0) break;
                        var blendLayer = source != null ? source.BlendLayer : InstanceBlendLayer.Middle;
                        if (blendLayer != (InstanceBlendLayer)layer) break;
                        bool blending = (source != null && source.BlendMode != BlendMode.None) || a < 1;          //default blend should be none

                        if (instances != null)
                        {
                            if (!blending)
                            {
                                instances = instances.Where(i => i.Color.A < 1);
                                if (!instances.Any()) break;
                            }
                            var cp = graphics.Camera.Position;
                            instances = instances.OrderByDescending(i => Vector3.DistanceSquared(cp, i.Model.Translation));
                        }
                        else if (!blending) break;

                        if (push) graphics.Push();
                        var material = source?.GetMaterial(progress, random) ?? Material.Default;
                        if (material.BlendMode == BlendMode.None) material.BlendMode = BlendMode.Alpha;
                        graphics.Material = material;
                    }
                    return true;
                case InstanceLayer.Distortion:
                    {
                        if (source == null || source.Shader != MaterialShader.Distortion) break;
                        if (push) graphics.Push();
                        var material = source.GetMaterial(progress, random);
                    }
                    return true;
            }
            return false;
        }
    }
}
