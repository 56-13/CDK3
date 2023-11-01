using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using CDK.Assets.Scenes;

namespace CDK.Assets
{
    public class AssetControl
    {
        public ImageList LargeImageList { private set; get; }
        public ImageList SmallImageList { private set; get; }

        private List<Control> _controls;
        private Dictionary<object, Scene> _scenes;

        public const int MaxControlSize = 8192;
        
        private AssetControl()
        {
            LargeImageList = new ImageList
            {
                ImageSize = new Size(64, 64)
            };

            LargeImageList.Images.Add(AssetType.Project.ToString(), Properties.Resources.projectIcon);
            LargeImageList.Images.Add(AssetType.Package.ToString(), Properties.Resources.packageIcon);
            LargeImageList.Images.Add(AssetType.List.ToString(), Properties.Resources.listIcon);
            LargeImageList.Images.Add(AssetType.Block.ToString(), Properties.Resources.blockIcon);
            LargeImageList.Images.Add(AssetType.BlockList.ToString(), Properties.Resources.blockListIcon);
            LargeImageList.Images.Add(AssetType.File.ToString(), Properties.Resources.fileIcon);
            LargeImageList.Images.Add(AssetType.Folder.ToString(), Properties.Resources.folderIcon);
            LargeImageList.Images.Add(AssetType.Binary.ToString(), Properties.Resources.binaryIcon);
            LargeImageList.Images.Add(AssetType.Media.ToString(), Properties.Resources.mediaIcon);
            LargeImageList.Images.Add(AssetType.String.ToString(), Properties.Resources.stringIcon);
            LargeImageList.Images.Add(AssetType.Attribute.ToString(), Properties.Resources.dataIcon);
            LargeImageList.Images.Add(AssetType.RootImage.ToString(), Properties.Resources.imageIcon);
            LargeImageList.Images.Add(AssetType.SubImage.ToString(), Properties.Resources.imageIcon);
            LargeImageList.Images.Add(AssetType.Material.ToString(), Properties.Resources.materialIcon);
            LargeImageList.Images.Add(AssetType.Skybox.ToString(), Properties.Resources.skyboxIcon);
            LargeImageList.Images.Add(AssetType.Animation.ToString(), Properties.Resources.animationIcon);
            LargeImageList.Images.Add(AssetType.TriggerFormat.ToString(), Properties.Resources.formatIcon);
            LargeImageList.Images.Add(AssetType.Trigger.ToString(), Properties.Resources.dataIcon);
            LargeImageList.Images.Add(AssetType.Mesh.ToString(), Properties.Resources.meshIcon);
            LargeImageList.Images.Add(AssetType.Reference.ToString(), Properties.Resources.referenceIcon);
            LargeImageList.Images.Add(AssetType.Independent.ToString(), Properties.Resources.independentIcon);
            LargeImageList.Images.Add(AssetType.Version.ToString(), Properties.Resources.versionIcon);
            LargeImageList.Images.Add(AssetType.Unused.ToString(), Properties.Resources.unusedIcon);
            LargeImageList.Images.Add(AssetType.Reference.ToString(), Properties.Resources.dataIcon);

            LargeImageList.Images.Add(SceneComponentType.Object.ToString(), Properties.Resources.packageIcon);
            LargeImageList.Images.Add(SceneComponentType.Box.ToString(), Properties.Resources.packageIcon);     //TODO
            LargeImageList.Images.Add(SceneComponentType.Sphere.ToString(), Properties.Resources.packageIcon);     //TODO
            LargeImageList.Images.Add(SceneComponentType.Capsule.ToString(), Properties.Resources.packageIcon);     //TODO
            LargeImageList.Images.Add(SceneComponentType.Image.ToString(), Properties.Resources.imageIcon);
            LargeImageList.Images.Add(SceneComponentType.Particle.ToString(), Properties.Resources.particleIcon);
            LargeImageList.Images.Add(SceneComponentType.Trail.ToString(), Properties.Resources.trailIcon);
            LargeImageList.Images.Add(SceneComponentType.Sprite.ToString(), Properties.Resources.spriteIcon);
            LargeImageList.Images.Add(SceneComponentType.DirectionalLight.ToString(), Properties.Resources.lightIcon);
            LargeImageList.Images.Add(SceneComponentType.PointLight.ToString(), Properties.Resources.lightIcon);
            LargeImageList.Images.Add(SceneComponentType.SpotLight.ToString(), Properties.Resources.lightIcon);
            LargeImageList.Images.Add(SceneComponentType.Animation.ToString(), Properties.Resources.animationIcon);
            LargeImageList.Images.Add(SceneComponentType.AnimationReference.ToString(), Properties.Resources.animationIcon);
            LargeImageList.Images.Add(SceneComponentType.Camera.ToString(), Properties.Resources.cameraIcon);
            LargeImageList.Images.Add(SceneComponentType.Spawn.ToString(), Properties.Resources.spawnIcon);
            LargeImageList.Images.Add(SceneComponentType.Environment.ToString(), Properties.Resources.dataIcon);
            LargeImageList.Images.Add(SceneComponentType.Ground.ToString(), Properties.Resources.worldIcon);
            LargeImageList.Images.Add(SceneComponentType.Terrain.ToString(), Properties.Resources.worldIcon);
            LargeImageList.Images.Add(SceneComponentType.TerrainAltitude.ToString(), Properties.Resources.constructIcon);
            LargeImageList.Images.Add(SceneComponentType.TerrainSurface.ToString(), Properties.Resources.imageIcon);
            LargeImageList.Images.Add(SceneComponentType.TerrainWall.ToString(), Properties.Resources.wallIcon);
            LargeImageList.Images.Add(SceneComponentType.TerrainWater.ToString(), Properties.Resources.waterIcon);
            LargeImageList.Images.Add(SceneComponentType.TerrainTile.ToString(), Properties.Resources.tileIcon);
            LargeImageList.Images.Add(SceneComponentType.TerrainRegion.ToString(), Properties.Resources.regionIcon);
            LargeImageList.Images.Add(SceneComponentType.MeshGeometries.ToString(), Properties.Resources.meshIcon);
            LargeImageList.Images.Add(SceneComponentType.MeshGeometry.ToString(), Properties.Resources.meshIcon);
            LargeImageList.Images.Add(SceneComponentType.MeshAnimations.ToString(), Properties.Resources.animationIcon);
            LargeImageList.Images.Add(SceneComponentType.MeshAnimation.ToString(), Properties.Resources.animationIcon);
            LargeImageList.Images.Add(SceneComponentType.AnimationFragment.ToString(), Properties.Resources.animationIcon);

            SmallImageList = new ImageList
            {
                ImageSize = new Size(16, 16)
            };

            SmallImageList.Images.Add(AssetType.Project.ToString(), Properties.Resources.projectIcon_s);
            SmallImageList.Images.Add(AssetType.Package.ToString(), Properties.Resources.packageIcon_s);
            SmallImageList.Images.Add(AssetType.List.ToString(), Properties.Resources.listIcon_s);
            SmallImageList.Images.Add(AssetType.Block.ToString(), Properties.Resources.blockIcon_s);
            SmallImageList.Images.Add(AssetType.BlockList.ToString(), Properties.Resources.blockListIcon_s);
            SmallImageList.Images.Add(AssetType.File.ToString(), Properties.Resources.fileIcon_s);
            SmallImageList.Images.Add(AssetType.Folder.ToString(), Properties.Resources.folderIcon_s);
            SmallImageList.Images.Add(AssetType.Binary.ToString(), Properties.Resources.binaryIcon_s);
            SmallImageList.Images.Add(AssetType.Media.ToString(), Properties.Resources.mediaIcon_s);
            SmallImageList.Images.Add(AssetType.String.ToString(), Properties.Resources.stringIcon_s);
            SmallImageList.Images.Add(AssetType.Attribute.ToString(), Properties.Resources.dataIcon_s);
            SmallImageList.Images.Add(AssetType.RootImage.ToString(), Properties.Resources.imageIcon_s);
            SmallImageList.Images.Add(AssetType.SubImage.ToString(), Properties.Resources.imageIcon_s);
            SmallImageList.Images.Add(AssetType.Material.ToString(), Properties.Resources.materialIcon_s);
            SmallImageList.Images.Add(AssetType.Skybox.ToString(), Properties.Resources.skyboxIcon_s);
            SmallImageList.Images.Add(AssetType.Animation.ToString(), Properties.Resources.animationIcon_s);
            SmallImageList.Images.Add(AssetType.TriggerFormat.ToString(), Properties.Resources.formatIcon_s);
            SmallImageList.Images.Add(AssetType.Trigger.ToString(), Properties.Resources.dataIcon_s);
            SmallImageList.Images.Add(AssetType.Mesh.ToString(), Properties.Resources.meshIcon_s);
            SmallImageList.Images.Add(AssetType.Reference.ToString(), Properties.Resources.dataIcon_s);
            SmallImageList.Images.Add(AssetType.Independent.ToString(), Properties.Resources.independentIcon_s);
            SmallImageList.Images.Add(AssetType.Version.ToString(), Properties.Resources.versionIcon_s);
            SmallImageList.Images.Add(AssetType.Unused.ToString(), Properties.Resources.unusedIcon_s);

            SmallImageList.Images.Add(SceneComponentType.Object.ToString(), Properties.Resources.packageIcon_s);
            SmallImageList.Images.Add(SceneComponentType.Box.ToString(), Properties.Resources.packageIcon_s);     //TODO
            SmallImageList.Images.Add(SceneComponentType.Sphere.ToString(), Properties.Resources.packageIcon_s);     //TODO
            SmallImageList.Images.Add(SceneComponentType.Capsule.ToString(), Properties.Resources.packageIcon_s);     //TODO
            SmallImageList.Images.Add(SceneComponentType.Image.ToString(), Properties.Resources.imageIcon_s);
            SmallImageList.Images.Add(SceneComponentType.Particle.ToString(), Properties.Resources.particleIcon_s);
            SmallImageList.Images.Add(SceneComponentType.Trail.ToString(), Properties.Resources.trailIcon_s);
            SmallImageList.Images.Add(SceneComponentType.Sprite.ToString(), Properties.Resources.spriteIcon_s);
            SmallImageList.Images.Add(SceneComponentType.DirectionalLight.ToString(), Properties.Resources.lightIcon_s);
            SmallImageList.Images.Add(SceneComponentType.PointLight.ToString(), Properties.Resources.lightIcon_s);
            SmallImageList.Images.Add(SceneComponentType.SpotLight.ToString(), Properties.Resources.lightIcon_s);
            SmallImageList.Images.Add(SceneComponentType.Animation.ToString(), Properties.Resources.animationIcon_s);
            SmallImageList.Images.Add(SceneComponentType.AnimationReference.ToString(), Properties.Resources.animationIcon_s);
            SmallImageList.Images.Add(SceneComponentType.Camera.ToString(), Properties.Resources.cameraIcon_s);
            SmallImageList.Images.Add(SceneComponentType.Spawn.ToString(), Properties.Resources.spawnIcon_s);
            SmallImageList.Images.Add(SceneComponentType.Environment.ToString(), Properties.Resources.dataIcon_s);
            SmallImageList.Images.Add(SceneComponentType.Ground.ToString(), Properties.Resources.worldIcon_s);
            SmallImageList.Images.Add(SceneComponentType.Terrain.ToString(), Properties.Resources.worldIcon_s);
            SmallImageList.Images.Add(SceneComponentType.TerrainAltitude.ToString(), Properties.Resources.constructIcon_s);
            SmallImageList.Images.Add(SceneComponentType.TerrainSurface.ToString(), Properties.Resources.imageIcon_s);
            SmallImageList.Images.Add(SceneComponentType.TerrainWall.ToString(), Properties.Resources.wallIcon_s);
            SmallImageList.Images.Add(SceneComponentType.TerrainWater.ToString(), Properties.Resources.waterIcon_s);
            SmallImageList.Images.Add(SceneComponentType.TerrainTile.ToString(), Properties.Resources.tileIcon_s);
            SmallImageList.Images.Add(SceneComponentType.TerrainRegion.ToString(), Properties.Resources.regionIcon_s);
            SmallImageList.Images.Add(SceneComponentType.MeshGeometries.ToString(), Properties.Resources.meshIcon_s);
            SmallImageList.Images.Add(SceneComponentType.MeshGeometry.ToString(), Properties.Resources.meshIcon_s);
            SmallImageList.Images.Add(SceneComponentType.MeshAnimations.ToString(), Properties.Resources.animationIcon_s);
            SmallImageList.Images.Add(SceneComponentType.MeshAnimation.ToString(), Properties.Resources.animationIcon_s);
            SmallImageList.Images.Add(SceneComponentType.AnimationFragment.ToString(), Properties.Resources.animationIcon_s);

            _controls = new List<Control>();
            _scenes = new Dictionary<object, Scene>();
        }

        private void Dispose()
        {
            foreach (var control in _controls) control.Dispose();
            
            LargeImageList.Dispose();
            SmallImageList.Dispose();
        }

        public static string GetFilter(AssetType type)
        {
            switch (type)
            {
                case AssetType.Binary:
                    return FileFilters.Binary;
                case AssetType.Media:
                    return FileFilters.Media;
                case AssetType.String:
                    return FileFilters.String;
                case AssetType.RootImage:
                case AssetType.Skybox:
                    return FileFilters.Image;
                case AssetType.Material:
                    return FileFilters.Archive;
                case AssetType.Mesh:
                    return FileFilters.Mesh;
                case AssetType.TriggerFormat:
                    return FileFilters.Xml;
            }
            return null;
        }

        public T GetControl<T>() where T : Control, new()
        {
            Control control = null;

            foreach (var otherControl in _controls)
            {
                if (otherControl.Parent == null && otherControl is T)
                {
                    control = otherControl;
                    break;
                }
            }
            if (control == null)
            {
                control = new T();
                _controls.Add(control);
            }
            return (T)control;
        }

        public void RemoveControlData(object dataKey)
        {
            _scenes.Remove(dataKey);
        }

        public Control GetControl(Asset asset, object dataKey = null)           //TODO:씬을 유지하는 방법이 맘에 안듬
        {
            switch (asset.Type)
            {
                case AssetType.Attribute:
                    {
                        var control = GetControl<Attributes.AttributeAssetControl>();
                        control.Asset = (Attributes.AttributeAsset)asset;
                        return control;
                    }
                case AssetType.RootImage:
                    {
                        var control = GetControl<Texturing.ImageAssetControl>();
                        control.Asset = (Texturing.RootImageAsset)asset;
                        return control;
                    }
                case AssetType.SubImage:
                    {
                        var control = GetControl<Texturing.ImageAssetControl>();
                        control.SubAsset = (Texturing.SubImageAsset)asset;
                        return control;
                    }
                    /*
                case AssetType.Spawn:
                    {
                        var control = GetControl<Scenes.SpawnAssetControl>();
                        control.Asset = (Scenes.SpawnAsset)asset;
                        return control;
                    }
                    */
                    //TODO
                case AssetType.Material:
                case AssetType.Skybox:
                case AssetType.Mesh:
                case AssetType.Animation:
                case AssetType.Terrain:
                    {
                        var control = GetControl<SceneControl>();

                        Scene scene;

                        if (dataKey != null)
                        {
                            if (!_scenes.TryGetValue(dataKey, out scene))
                            {
                                scene = asset.NewScene();
                                _scenes.Add(dataKey, scene);
                            }
                        }
                        else scene = asset.NewScene();

                        control.Scene = scene;
                        return control;
                    }
                default:
                    {
                        var control = GetControl<Containers.ContainerAssetControl>();
                        control.Asset = asset;
                        return control;
                    }
            }
        }

        public Control GetControl(SceneComponent comp)
        {
            /*
            switch (comp.Type)
            {
                case SceneComponentType.Environment:
                    {
                        var control = GetControl<EnvironmentControl>();
                        control.Object = (Scenes.Environment)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.Ground:
                    {
                        var control = GetControl<GroundControl>();
                        control.Object = (Ground)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.Object:
                    {
                        var control = GetControl<SceneObjectControl>();
                        control.Object = (SceneObject)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.Box:
                    {
                        var control = GetControl<BoxControl>();
                        control.Object = (BoxObject)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.Sphere:
                    {
                        var control = GetControl<SphereControl>();
                        control.Object = (SphereObject)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.Spawn:
                    {
                        var control = GetControl<SpawnControl>();
                        control.Object = (Spawn)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.Camera:
                    {
                        var control = GetControl<CameraControl>();
                        control.Object = (Scenes.CameraObject)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.DirectionalLight:
                    {
                        var control = GetControl<DirectionalLightControl>();
                        control.Object = (Scenes.DirectionalLightObject)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.PointLight:
                    {
                        var control = GetControl<PointLightControl>();
                        control.Object = (Scenes.PointLightObject)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.SpotLight:
                    {
                        var control = GetControl<SpotLightControl>();
                        control.Object = (Scenes.SpotLightObject)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.Material:
                    {
                        var control = GetControl<Texturing.MaterialAssetControl>();
                        control.Asset = ((Texturing.MaterialSceneObject)comp).Asset;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.Skybox:
                    {
                        var control = GetControl<Texturing.SkyboxAssetControl>();
                        control.Asset = ((Texturing.SkyboxComponent)comp).Asset;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.Mesh:
                    {
                        var control = GetControl<Meshing.MeshControl>();
                        control.Object = (Meshing.MeshObject)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.MeshGeometry:
                    {
                        var control = GetControl<Meshing.MeshGeometryControl>();
                        control.Object = (Meshing.MeshGeometryComponent)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.MeshAnimation:
                    {
                        var control = GetControl<Meshing.MeshAnimationControl>();
                        control.Object = (Meshing.MeshAnimationComponent)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.AnimationRoot:
                    {
                        var control = GetControl<Animations.AnimationControl>();
                        control.Object = (Animations.AnimationObject)comp;
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
                case SceneComponentType.Animation:
                    {
                        var control = GetControl<Animations.AnimationFragmentControl>();
                        control.Object = (Animations.AnimationObjectFragment)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.Terrain:
                    {
                        var control = GetControl<Terrain.TerrainAssetControl>();
                        control.Asset = ((Terrain.TerrainSceneObject)comp).Asset;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.TerrainAltitude:
                    {
                        var control = GetControl<Terrain.TerrainAltitudeControl>();
                        control.Object = (Terrain.TerrainAltitudeComponent)comp;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.TerrainSurface:
                    {
                        var control = GetControl<Terrain.TerrainSurfaceControl>();
                        control.Object = (Terrain.TerrainSurfaceComponent)comp;
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
                case SceneComponentType.TerrainWall:
                    {
                        var control = GetControl<Terrain.TerrainWallControl>();
                        control.Object = (Terrain.TerrainWallComponent)comp;
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
                case SceneComponentType.TerrainWater:
                    {
                        var control = GetControl<Terrain.TerrainWaterControl>();
                        control.Object = (Terrain.TerrainWaterComponent)comp;
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
                case SceneComponentType.TerrainTile:
                    {
                        var control = GetControl<Terrain.TerrainTileControl>();
                        control.Object = (Terrain.TerrainTileComponent)comp;
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
            }
            */
            return null;
        }

        //TODO
        /*
        public Control GetSceneObjectReferenceControl(SceneObject obj)
        {
            switch (obj.Type)
            {
                case SceneComponentType.Mesh:
                    {
                        var control = GetControl<Meshing.MeshSelectionControl>();
                        control.Selection = ((Meshing.MeshObject)obj).Selection;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.AnimationRoot:
                    {
                        var control = GetControl<Animations.AnimationControl>();
                        control.Object = (Animations.AnimationObject)obj;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
                case SceneComponentType.SpawnObject:
                    {
                        var control = GetControl<Scenes.SpawnControl>();
                        control.Object = (Scenes.SpawnObject)obj;
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                        control.Dock = DockStyle.None;
                        return control;
                    }
            }
            return null;
        }
        */
         
        public static void ShowRetained(Control owner, AssetElement from, AssetElement to)
        {
            if (from.Owner != null)
            {
                var msg = $"{from.GetLocation()} 이 \r\n{to.GetLocation()} 을 \r\n참조하고 있습니다. 해당 리소스로 이동하시겠습니까?";

                if (MessageBox.Show(owner, msg, "이동", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    AssetManager.Instance.Open(from.Owner);
                }
            }
            else
            {
                var msg = $"{from.GetLocation()} 이 \r\n{to.GetLocation()} 을 \r\n참조하고 있습니다.";
                MessageBox.Show(owner, msg);
            }
        }

        public static AssetControl Instance { private set; get; }
        public static bool IsCreated => Instance != null;
        public static void CreateShared()
        {
            if (Instance == null) Instance = new AssetControl();
        }
        public static void DisposeShared()
        {
            if (Instance != null)
            {
                Instance.Dispose();
                Instance = null;
            }
        }
    }
}
