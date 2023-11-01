using System.Linq;
using System.IO;

using Assimp;
using Assimp.Configs;

namespace CDK.Drawing.Meshing
{
    public class Loader
    {
        public static void Import(string name, string path, int xrotation, float scale, bool flipuv, out Geometry geometry, out Animation[] animations)
        {
            var importer = new AssimpContext();
            
            importer.SetConfig(new NormalSmoothingAngleConfig(66));
            importer.SetConfig(new MaxBoneCountConfig(256));

            importer.XAxisRotation = xrotation;

            importer.Scale = scale;
            
            PostProcessSteps ppstep =
                PostProcessSteps.CalculateTangentSpace |
                PostProcessSteps.JoinIdenticalVertices |
                PostProcessSteps.MakeLeftHanded |
                PostProcessSteps.Triangulate |
                PostProcessSteps.GenerateSmoothNormals |
                PostProcessSteps.LimitBoneWeights |
                PostProcessSteps.ValidateDataStructure |
                PostProcessSteps.ImproveCacheLocality |
                PostProcessSteps.RemoveRedundantMaterials |
                PostProcessSteps.SortByPrimitiveType |
                PostProcessSteps.FindDegenerates |
                PostProcessSteps.FindInvalidData |
                PostProcessSteps.GenerateUVCoords |
                PostProcessSteps.TransformUVCoords |
                PostProcessSteps.FindInstances |
                PostProcessSteps.OptimizeMeshes |
                PostProcessSteps.OptimizeGraph |
                PostProcessSteps.FlipWindingOrder |
                PostProcessSteps.SplitByBoneCount;

            if (flipuv) ppstep |= PostProcessSteps.FlipUVs;

            var scene = importer.ImportFile(path, ppstep);

            if (scene == null) throw new IOException();

            geometry = new Geometry(name, scene);

            animations = scene.Animations.Select(a => new Animation(a)).ToArray();
        }
    }
}
