using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Assets.Scenes;

namespace CDK.Assets.Animations
{
    public interface IAnimationSubstance
    {
        SceneComponentType Type { get; }
        SceneObject CreateObject(AnimationObjectFragment parent);
        IAnimationSubstance Clone(AnimationFragment parent);
        void GetTransformNames(ICollection<string> names);
        void AddRetains(ICollection<string> retains);
        bool IsRetaining(AssetElement element, out AssetElement from);
        void Save(XmlWriter writer);
        void Load(XmlNode node);
        void Build(BinaryWriter writer);
        void GetLocaleStrings(ICollection<LocaleString> strings);
    }

    public static class AnimationSubstance
    {
        public static readonly SceneComponentType[] SubstanceTypes =
        {
            SceneComponentType.Box,
            SceneComponentType.Sphere,
            SceneComponentType.Capsule,
            SceneComponentType.Mesh,
            SceneComponentType.Image,
            SceneComponentType.Particle,
            SceneComponentType.Trail,
            SceneComponentType.Sprite,
            SceneComponentType.DirectionalLight,
            SceneComponentType.PointLight,
            SceneComponentType.SpotLight,
            SceneComponentType.AnimationReference,
            SceneComponentType.Camera
        };

        public static IAnimationSubstance Create(AnimationFragment parent, SceneComponentType type)
        {
            switch (type)
            {
                case SceneComponentType.Box:
                    return new BoxObject() { Parent = parent };
                case SceneComponentType.Sphere:
                    return new SphereObject() { Parent = parent };
                case SceneComponentType.Capsule:
                    return new CapsuleObject() { Parent = parent };
                case SceneComponentType.Mesh:
                    return new Meshing.MeshSelection(parent);
                case SceneComponentType.Image:
                    return new Texturing.ImageObject() { Parent = parent };
                case SceneComponentType.Particle:
                    return new Particle.Particle(parent);
                case SceneComponentType.Trail:
                    return new Trail.Trail(parent);
                case SceneComponentType.Sprite:
                    return new Sprite.Sprite(parent);
                case SceneComponentType.DirectionalLight:
                    return new DirectionalLightObject() { Parent = parent };
                case SceneComponentType.PointLight:
                    return new PointLightObject() { Parent = parent };
                case SceneComponentType.SpotLight:
                    return new SpotLightObject() { Parent = parent };
                case SceneComponentType.AnimationReference:
                    return new AnimationReferenceObject() { Parent = parent };
                case SceneComponentType.Camera:
                    return new CameraObject() { Parent = parent };
            }
            return null;
        }
    }
}
