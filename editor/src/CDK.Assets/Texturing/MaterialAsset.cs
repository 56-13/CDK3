using System;
using System.Numerics;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Xml;
using System.IO;
using System.IO.Compression;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;
using CDK.Assets.Updaters;

using Bitmap = System.Drawing.Bitmap;

namespace CDK.Assets.Texturing
{
    public enum TextureAssetColorEncoding
    {
        HighSample,
        HighLinear,
        Low,
        CompressedSample,
        CompressedLinear
    }

    public enum TextureAssetComponentEncoding
    {
        High,
        Low,
        Compressed
    }

    public class MaterialAsset : Asset
    {
        private TextureSlotDescription _TextureDescription;
        public TextureSlotDescription TextureDescription
        {
            get
            {
                Load();
                return _TextureDescription;
            }
        }

        public TextureSlot _ColorMap;
        public TextureSlot ColorMap
        {
            get
            {
                Load();
                return _ColorMap;
            }
        }
        public TextureSlot _NormalMap;
        public TextureSlot NormalMap
        {
            get
            {
                Load();
                return _NormalMap;
            }
        }

        public TextureSlot _MaterialMap;
        public TextureSlot MaterialMap
        {
            get
            {
                Load();
                return _MaterialMap;
            }
        }

        public TextureSlot _EmissiveMap;
        public TextureSlot EmissiveMap
        {
            get
            {
                Load();
                return _EmissiveMap;
            }
        }

        private MaterialAsset _ColorReference;
        public MaterialAsset ColorReference
        {
            set
            {
                Load();

                var prev = _ColorReference;
                if (SetProperty(ref _ColorReference, value))
                {
                    if (value != null && AssetManager.Instance.CommandEnabled) _ColorMap.Bitmap = null;
                    LinkReference(prev, value);
                    TextureRefresh?.Invoke(this, EventArgs.Empty);
                    OnPropertyChanged("HasColor");
                    OnPropertyChanged("HasOpacity");
                }
            }
            get
            {
                Load();
                return _ColorReference;
            }
        }

        private MaterialAsset _NormalReference;
        public MaterialAsset NormalReference
        {
            set
            {
                Load();

                var prev = _NormalReference;
                if (SetProperty(ref _NormalReference, value))
                {
                    if (value != null && AssetManager.Instance.CommandEnabled) _NormalMap.Bitmap = null;
                    LinkReference(prev, value);
                    TextureRefresh?.Invoke(this, EventArgs.Empty);
                    OnPropertyChanged("HasNormal");
                    OnPropertyChanged("HasDisplacement");
                }
            }
            get
            {
                Load();
                return _NormalReference;
            }
        }


        private MaterialAsset _MaterialReference;
        public MaterialAsset MaterialReference
        {
            set
            {
                Load();

                var prev = _MaterialReference;
                if (SetProperty(ref _MaterialReference, value))
                {
                    if (value != null && AssetManager.Instance.CommandEnabled) _MaterialMap.Bitmap = null;
                    LinkReference(prev, value);
                    TextureRefresh?.Invoke(this, EventArgs.Empty);
                    OnPropertyChanged("HasMaterial");
                    OnPropertyChanged("HasMetallic");
                    OnPropertyChanged("HasRoughness");
                    OnPropertyChanged("HasAmbientOcclusion");
                }
            }
            get
            {
                Load();
                return _MaterialReference;
            }
        }

        private MaterialAsset _EmissiveReference;
        public MaterialAsset EmissiveReference
        {
            set
            {
                Load();

                var prev = _EmissiveReference;
                if (SetProperty(ref _EmissiveReference, value))
                {
                    if (value != null && AssetManager.Instance.CommandEnabled) _EmissiveMap.Bitmap = null;
                    LinkReference(prev, value);
                    TextureRefresh?.Invoke(this, EventArgs.Empty);
                    OnPropertyChanged("HasEmissive");
                }
            }
            get
            {
                Load();
                return _EmissiveReference;
            }
        }

        private void LinkReference(MaterialAsset prev, MaterialAsset next)
        {
            if (prev != null)
            {
                prev.PropertyChanged -= Reference_PropertyChanged;
                prev.TextureRefresh -= Reference_TextureRefresh;
            }
            if (next != null)
            {
                next.PropertyChanged += Reference_PropertyChanged;
                next.TextureRefresh += Reference_TextureRefresh;
            }
        }

        private void Reference_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "HasColor":
                case "HasOpacity":
                case "HasNormal":
                case "HasDisplacement":
                case "HasMaterial":
                case "HasMetallic":
                case "HasRoughness":
                case "HasAmbientOcclusion":
                case "HasEmissive":
                case "ColorEncoding":
                case "NormalEncoding":
                case "MaterialEncoding":
                case "EmissiveEncoding":
                    OnPropertyChanged(e.PropertyName);
                    break;
            }
        }

        private void Reference_TextureRefresh(object sender, EventArgs e)
        {
            TextureRefresh?.Invoke(this, EventArgs.Empty);
        }

        public int Width
        {
            get
            {
                Load();
                var colorMap = _ColorReference?.ColorMap.Texture ?? _ColorMap.Texture;
                if (colorMap != null) return colorMap.Description.Width;
                var normalMap = _NormalReference?.NormalMap.Texture ?? _NormalMap.Texture;
                if (normalMap != null) return normalMap.Description.Width;
                var materialMap = _MaterialReference?.MaterialMap.Texture ?? _MaterialMap.Texture;
                if (materialMap != null) return materialMap.Description.Width;
                var emissiveMap = _EmissiveReference?.EmissiveMap.Texture ?? _EmissiveMap.Texture;
                if (emissiveMap != null) return emissiveMap.Description.Width;
                return 0;
            }
        }

        public int Height
        {
            get
            {
                Load();
                var colorMap = _ColorReference?.ColorMap.Texture ?? _ColorMap.Texture;
                if (colorMap != null) return colorMap.Description.Height;
                var normalMap = _NormalReference?.NormalMap.Texture ?? _NormalMap.Texture;
                if (normalMap != null) return normalMap.Description.Height;
                var materialMap = _MaterialReference?.MaterialMap.Texture ?? _MaterialMap.Texture;
                if (materialMap != null) return materialMap.Description.Height;
                var emissiveMap = _EmissiveReference?.EmissiveMap.Texture ?? _EmissiveMap.Texture;
                if (emissiveMap != null) return emissiveMap.Description.Height;
                return 0;
            }
        }

        public bool HasColor => ColorReference?.ColorMap.Exists ?? _ColorMap.Exists;

        private bool _HasOpacity;
        public bool HasOpacity
        {
            private set => SetProperty(ref _HasOpacity, value);
            get
            {
                Load();

                _ColorReference?.Load();
                return _ColorReference?._HasOpacity ?? _HasOpacity;
            }
        }

        public bool HasNormal => NormalReference?.NormalMap.Exists ?? _NormalMap.Exists;

        private bool _HasDisplacement;
        public bool HasDisplacement
        {
            private set => SetProperty(ref _HasDisplacement, value);
            get
            {
                Load();
                _NormalReference?.Load();
                return _NormalReference?._HasDisplacement ?? _HasDisplacement;
            }
        }

        public bool HasMaterial => MaterialReference?.MaterialMap.Exists ?? _MaterialMap.Exists;

        private bool _HasMetallic;
        public bool HasMetallic
        {
            private set => SetProperty(ref _HasMetallic, value);
            get
            {
                Load();
                _MaterialReference?.Load();
                return _MaterialReference?._HasMetallic ?? _HasMetallic;
            }
        }

        private bool _HasRoughness;
        public bool HasRoughness
        {
            private set => SetProperty(ref _HasRoughness, value);
            get
            {
                Load();
                _MaterialReference?.Load();
                return _MaterialReference?._HasRoughness ?? _HasRoughness;
            }
        }

        private bool _HasAmbientOcclusion;
        public bool HasAmbientOcclusion
        {
            private set => SetProperty(ref _HasAmbientOcclusion, value);
            get
            {
                Load();
                _MaterialReference?.Load();
                return _MaterialReference?._HasAmbientOcclusion ?? _HasAmbientOcclusion;
            }
        }

        public bool HasEmissive => EmissiveReference?.EmissiveMap.Exists ?? _EmissiveMap.Exists;

        public MaterialMapComponent MaterialComponent
        {
            get
            {
                MaterialMapComponent comp = 0;
                if (HasMetallic) comp |= MaterialMapComponent.Metallic;
                if (HasRoughness) comp |= MaterialMapComponent.Roughness;
                if (HasAmbientOcclusion) comp |= MaterialMapComponent.Occlusion;
                return comp;
            }
        }

        public TextureAssetColorEncoding _ColorEncoding;
        public TextureAssetColorEncoding ColorEncoding
        {
            set
            {
                Load();
                if (SetProperty(ref _ColorEncoding, value)) ResetColorEncoding();
            }
            get
            {
                Load();
                _ColorReference?.Load();
                return _ColorReference?._ColorEncoding ?? _ColorEncoding;
            }
        }

        public TextureAssetComponentEncoding _NormalEncoding;
        public TextureAssetComponentEncoding NormalEncoding
        {
            set
            {
                Load();
                if (SetProperty(ref _NormalEncoding, value)) ResetNormalEncoding();
            }
            get
            {
                Load();
                _NormalReference?.Load();
                return _NormalReference?._NormalEncoding ?? _NormalEncoding;
            }
        }

        public TextureAssetComponentEncoding _MaterialEncoding;
        public TextureAssetComponentEncoding MaterialEncoding
        {
            set
            {
                Load();
                if (SetProperty(ref _MaterialEncoding, value)) ResetMaterialEncoding();
            }
            get
            {
                Load();
                _MaterialReference?.Load();
                return _MaterialReference?._MaterialEncoding ?? _MaterialEncoding;
            }
        }

        public TextureAssetColorEncoding _EmissiveEncoding;
        public TextureAssetColorEncoding EmissiveEncoding
        {
            set
            {
                Load();
                if (SetProperty(ref _EmissiveEncoding, value)) ResetEmissiveEncoding();
            }
            get
            {
                Load();
                _EmissiveReference?.Load();
                return _EmissiveReference?._EmissiveEncoding ?? _EmissiveEncoding;
            }
        }

        private Material _Material;
        public Material Material
        {
            get
            {
                Load();
                return _Material;
            }
        }

        public event EventHandler TextureRefresh;

        public void AddWeakTextureRefresh(EventHandler<EventArgs> handler)
        {
            WeakEventManager<MaterialAsset, EventArgs>.AddHandler(this, "TextureRefresh", handler);
        }

        public void RemoveWeakTextureRefresh(EventHandler<EventArgs> handler)
        {
            WeakEventManager<MaterialAsset, EventArgs>.RemoveHandler(this, "TextureRefresh", handler);
        }

        public MaterialAsset()
        {
            using (new AssetCommandHolder())
            {
                _TextureDescription = new TextureSlotDescription(this, TextureTarget.Texture2D)
                {
                    MinFilter = TextureMinFilter.LinearMipmapLinear,
                    MipmapCount = TextureSlotDescription.MaxMipmapCount
                };            
            }

            _ColorMap = new TextureSlot(_TextureDescription, "color");
            _NormalMap = new TextureSlot(_TextureDescription, "normal");
            _MaterialMap = new TextureSlot(_TextureDescription, "material");
            _EmissiveMap = new TextureSlot(_TextureDescription, "emissive");

            ResetColorEncoding();
            ResetNormalEncoding();
            ResetMaterialEncoding();
            ResetEmissiveEncoding();

            _Material = new Material(this, MaterialUsage.Origin);

            _ColorMap.PropertyChanged += Map_PropertyChanged;
            _NormalMap.PropertyChanged += Map_PropertyChanged;
            _MaterialMap.PropertyChanged += Map_PropertyChanged;
            _EmissiveMap.PropertyChanged += Map_PropertyChanged;
        }

        public MaterialAsset(MaterialAsset other, bool content) : base(other, content)
        {
            other.Load();

            if (content)
            {
                _TextureDescription = new TextureSlotDescription(this, other._TextureDescription);

                _ColorMap = new TextureSlot(_TextureDescription, other._ColorMap);
                _NormalMap = new TextureSlot(_TextureDescription, other._NormalMap);
                _MaterialMap = new TextureSlot(_TextureDescription, other._MaterialMap);
                _EmissiveMap = new TextureSlot(_TextureDescription, other._EmissiveMap);

                AssetManager.Instance.InvokeRedirection(() =>
                {
                    _ColorReference = AssetManager.Instance.GetRedirection(other._ColorReference);
                    _NormalReference = AssetManager.Instance.GetRedirection(other._NormalReference);
                    _MaterialReference = AssetManager.Instance.GetRedirection(other._MaterialReference);
                    _EmissiveReference = AssetManager.Instance.GetRedirection(other._EmissiveReference);

                    LinkReference(null, _ColorReference);
                    LinkReference(null, _NormalReference);
                    LinkReference(null, _MaterialReference);
                    LinkReference(null, _EmissiveReference);
                });

                _ColorEncoding = other._ColorEncoding;
                _NormalEncoding = other._NormalEncoding;
                _MaterialEncoding = other._MaterialEncoding;
                _EmissiveEncoding = other._EmissiveEncoding;

                _HasOpacity = other._HasOpacity;
                _HasDisplacement = other._HasDisplacement;
                _HasMetallic = other._HasMetallic;
                _HasRoughness = other._HasRoughness;
                _HasAmbientOcclusion = other._HasAmbientOcclusion;

                _Material = new Material(this, other._Material, MaterialUsage.Origin);
            }
            else
            {
                using (new AssetCommandHolder())
                { 
                    _TextureDescription = new TextureSlotDescription(this, TextureTarget.Texture2D)
                    {
                        MinFilter = TextureMinFilter.LinearMipmapLinear,
                        MipmapCount = TextureSlotDescription.MaxMipmapCount
                    };
                }

                _ColorMap = new TextureSlot(_TextureDescription, "color");
                _NormalMap = new TextureSlot(_TextureDescription, "normal");
                _MaterialMap = new TextureSlot(_TextureDescription, "material");
                _EmissiveMap = new TextureSlot(_TextureDescription, "emissive");

                ResetColorEncoding();
                ResetNormalEncoding();
                ResetMaterialEncoding();
                ResetEmissiveEncoding();

                _Material = new Material(this, MaterialUsage.Origin);
            }
            _ColorMap.PropertyChanged += Map_PropertyChanged;
            _NormalMap.PropertyChanged += Map_PropertyChanged;
            _MaterialMap.PropertyChanged += Map_PropertyChanged;
            _EmissiveMap.PropertyChanged += Map_PropertyChanged;
        }

        private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Texture")
            {
                TextureRefresh?.Invoke(this, EventArgs.Empty);
            }
            else if (e.PropertyName == "Exists")
            {
                if (sender == _ColorMap) OnPropertyChanged("HasColor");
                else if (sender == _NormalMap) OnPropertyChanged("HasNormal");
                else if (sender == _MaterialMap) OnPropertyChanged("HasMaterial");
                else if (sender == _EmissiveMap) OnPropertyChanged("HasEmissive");
            }
        }

        private void ResetColorEncoding()
        {
            switch (_ColorEncoding)
            {
                case TextureAssetColorEncoding.HighLinear:
                    _ColorMap.Formats = new RawFormat[] { _HasOpacity ? RawFormat.Rgba8 : RawFormat.Rgb8 };
                    break;
                case TextureAssetColorEncoding.HighSample:
                    _ColorMap.Formats = new RawFormat[] { _HasOpacity ? RawFormat.Srgb8Alpha8  : RawFormat.Srgb8 };
                    break;
                case TextureAssetColorEncoding.Low:
                    _ColorMap.Formats = new RawFormat[] { _HasOpacity ? RawFormat.Rgba4 : RawFormat.Rgb5 };
                    break;
                case TextureAssetColorEncoding.CompressedLinear:
                    {
                        var config = AssetManager.Instance.Config.Texture;
                        _ColorMap.Formats = _HasOpacity ? config.CompressedRgba : config.CompressedRgb;
                    }
                    break;
                case TextureAssetColorEncoding.CompressedSample:
                    {
                        var config = AssetManager.Instance.Config.Texture;
                        _ColorMap.Formats = _HasOpacity ? config.CompressedSrgbA : config.CompressedSrgb;
                    }
                    break;
            }
        }

        private void ResetNormalEncoding()
        {
            switch (_NormalEncoding)
            {
                case TextureAssetComponentEncoding.High:
                    _NormalMap.Formats = new RawFormat[] { _HasDisplacement ? RawFormat.Rgba8 : RawFormat.Rgb8 };
                    break;
                case TextureAssetComponentEncoding.Low:
                    _NormalMap.Formats = new RawFormat[] { _HasDisplacement ? RawFormat.Rgba4 : RawFormat.Rgb5 };
                    break;
                case TextureAssetComponentEncoding.Compressed:
                    {
                        var config = AssetManager.Instance.Config.Texture;
                        _NormalMap.Formats = _HasDisplacement ? config.CompressedRgb : config.CompressedRgba;
                    }
                    break;
            }
        }

        private void ResetMaterialEncoding()
        {
            var component = 0;
            if (_HasMetallic) component++;
            if (_HasRoughness) component++;
            if (_HasAmbientOcclusion) component++;

            switch (_MaterialEncoding)
            {
                case TextureAssetComponentEncoding.High:
                    {
                        switch (component)
                        {
                            case 1:
                                _MaterialMap.Formats = new RawFormat[] { RawFormat.R8 };
                                break;
                            case 2:
                                _MaterialMap.Formats = new RawFormat[] { RawFormat.Rg8 };
                                break;
                            default:
                                _MaterialMap.Formats = new RawFormat[] { RawFormat.Rgb8 };
                                break;
                        }
                    }
                    break;
                case TextureAssetComponentEncoding.Low:
                    {
                        switch (component)
                        {
                            case 1:
                                _MaterialMap.Formats = new RawFormat[] { RawFormat.R8 };
                                break;
                            case 2:
                                _MaterialMap.Formats = new RawFormat[] { RawFormat.Rg8 };
                                break;
                            default:
                                _MaterialMap.Formats = new RawFormat[] { RawFormat.Rgb5 };
                                break;
                        }
                    }
                    break;
                case TextureAssetComponentEncoding.Compressed:
                    _MaterialMap.Formats = AssetManager.Instance.Config.Texture.CompressedRgb;
                    break;
            }
        }

        private void ResetEmissiveEncoding()
        {
            switch (_EmissiveEncoding)
            {
                case TextureAssetColorEncoding.HighLinear:
                    _EmissiveMap.Formats = new RawFormat[] { RawFormat.Rgb8 };
                    break;
                case TextureAssetColorEncoding.HighSample:
                    _EmissiveMap.Formats = new RawFormat[] { RawFormat.Srgb8 };
                    break;
                case TextureAssetColorEncoding.Low:
                    _EmissiveMap.Formats = new RawFormat[] { RawFormat.Rgb5 };
                    break;
                case TextureAssetColorEncoding.CompressedLinear:
                    {
                        var config = AssetManager.Instance.Config.Texture;
                        _EmissiveMap.Formats = config.CompressedRgb;
                    }
                    break;
                case TextureAssetColorEncoding.CompressedSample:
                    {
                        var config = AssetManager.Instance.Config.Texture;
                        _EmissiveMap.Formats = config.CompressedSrgb;
                    }
                    break;
            }
        }

        public void SetColorMap(string colorPath, string opacityPath)
        {
            AssetManager.Instance.Purge();

            Load();

            var color = colorPath != null ? BitmapTexture.Load(colorPath) : null;
            var opacity = opacityPath != null ? BitmapTexture.Load(opacityPath) : null;

            if (opacity != null)
            {
                var newColor = BitmapTexture.Compose31(color, opacity);
                color.Dispose();
                opacity.Dispose();
                color = newColor;
            }
            HasOpacity = color != null && BitmapTexture.HasOpacity(color);

            _ColorMap.Bitmap = color;

            ResetColorEncoding();

            ColorReference = null;
        }

        public void SetNormalMap(string normalPath, string displacementPath)
        {
            AssetManager.Instance.Purge();

            Load();

            var normal = normalPath != null ? BitmapTexture.Load(normalPath) : null;
            var displacement = displacementPath != null ? BitmapTexture.Load(displacementPath) : null;

            if (HasDisplacement = (normal != null && displacement != null))
            {
                var bitmap = BitmapTexture.Compose31(normal, displacement);
                normal.Dispose();
                displacement.Dispose();
                _NormalMap.Bitmap = bitmap;
            }
            else
            {
                _NormalMap.Bitmap = normal;
            }

            ResetNormalEncoding();

            NormalReference = null;
        }

        public void SetMaterialMap(string metallicPath, string roughnessPath, string ambientOcclusionPath)
        {
            AssetManager.Instance.Purge();

            Load();

            var metallic = metallicPath != null ? BitmapTexture.Load(metallicPath) : null;
            var roughness = roughnessPath != null ? BitmapTexture.Load(roughnessPath) : null;
            var ambientOcclusion = ambientOcclusionPath != null ? BitmapTexture.Load(ambientOcclusionPath) : null;

            var bitmaps = new Bitmap[3];
            var i = 0;

            if (HasMetallic = (metallic != null)) bitmaps[i++] = metallic;
            if (HasRoughness = (roughness != null)) bitmaps[i++] = roughness;
            if (HasAmbientOcclusion = (ambientOcclusion != null)) bitmaps[i++] = ambientOcclusion;
            if (i == 1) _MaterialMap.Bitmap = bitmaps[0];
            else if (i == 2)
            {
                var bitmap = BitmapTexture.Compose1111(bitmaps[0], bitmaps[1]);
                bitmaps[0].Dispose();
                bitmaps[1].Dispose();
                _MaterialMap.Bitmap = bitmap;
            }
            else if (i == 3)
            {
                var bitmap = BitmapTexture.Compose1111(bitmaps[0], bitmaps[1], bitmaps[2]);
                bitmaps[0].Dispose();
                bitmaps[1].Dispose();
                bitmaps[2].Dispose();
                _MaterialMap.Bitmap = bitmap;
            }
            if (_HasMetallic) Material.Metallic = 0.5f;
            if (_HasRoughness) Material.Roughness = 0.5f;
            if (_HasAmbientOcclusion) Material.AmbientOcclusion = 1;

            ResetMaterialEncoding();

            MaterialReference = null;
        }

        public void SetEmissiveMap(string path)
        {
            AssetManager.Instance.Purge();

            Load();

            var bitmap = path != null ? BitmapTexture.Load(path) : null;

            _EmissiveMap.Bitmap = bitmap;

            Material.Emission.Type = AnimationColorType.Constant;
            ((AnimationColorConstant)Material.Emission.Impl).Color = Color4.White;

            ResetEmissiveEncoding();

            EmissiveReference = null;
        }

        public override AssetType Type => AssetType.Material;
        public override Asset Clone(bool content) => new MaterialAsset(this, content);
        public override bool Spawnable => true;
        public override SceneComponent NewSceneComponent() => new SphereObject(Material, false);
        public override Scene NewScene()
        {
            var obj = new SphereObject(Material, true);
            obj.Located = true;
            obj.UsingTransform = true;
            obj.Transform.Position = new Vector3(0, 0, SphereObject.DefaultRadius);
            var scene = NewDefaultScene(obj);
            return scene;
        }

        public override void Import(string path)
        {
            string dirpath;
            string prefix;

            if (FileFilters.Contains(path, FileFilters.ArchiveExtensions))
            {
                var filename = Path.GetFileNameWithoutExtension(path);

                dirpath = Path.Combine(ResourceManager.Instance.DirectoryPath, filename);
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                    ZipFile.ExtractToDirectory(path, dirpath);
                }
                prefix = string.Empty;
            }
            else if (FileFilters.Contains(path, FileFilters.ImageExtensions))
            {
                dirpath = Path.GetDirectoryName(path);

                prefix = Path.GetFileNameWithoutExtension(path);
                var i = prefix.LastIndexOf('_');
                if (i < 0) return;
                prefix = prefix.Substring(0, i);
            }
            else return;

            string colorPath = null;
            string opacityPath = null;
            string metallicPath = null;
            string roughnessPath = null;
            string ambientOcclusionPath = null;
            string normalPath = null;
            string normalglPath = null;
            string displacementPath = null;

            var color = Color4.White;

            foreach (var file in Directory.GetFiles(dirpath, "*.*", SearchOption.AllDirectories))
            {
                var f = Path.GetFileName(file).ToLower();

                if (FileFilters.Contains(f, FileFilters.ImageExtensions) && f.StartsWith(prefix))
                {
                    if (f.Contains("_color") || f.Contains("_base") || f.Contains("_diff"))
                    {
                        colorPath = file;
                        if (GetSingleColor(colorPath, out var rgb))
                        {
                            colorPath = null;
                            color.R = rgb.R;
                            color.G = rgb.G;
                            color.B = rgb.B;
                        }
                    }
                    else if (f.Contains("_opacity"))
                    {
                        opacityPath = file;
                        if (GetSingleColor(opacityPath, out var a))
                        {
                            opacityPath = null;
                            color.A = a.A;
                        }
                    }
                    else if (f.Contains("_metal"))
                    {
                        metallicPath = file;
                        if (GetSingleColor(metallicPath, out var m))
                        {
                            metallicPath = null;
                            Material.Metallic = m.R;
                        }
                    }
                    else if (f.Contains("_rough"))
                    {
                        roughnessPath = file;
                        if (GetSingleColor(roughnessPath, out var r))
                        {
                            roughnessPath = null;
                            Material.Roughness = r.R;
                        }
                    }
                    else if (f.Contains("_ambient") || f.Contains("_ao"))
                    {
                        ambientOcclusionPath = file;
                    }
                    else if (f.Contains("_normalgl") || f.Contains("_normal_gl") || f.Contains("_nor_gl"))
                    {
                        normalglPath = file;
                    }
                    else if (f.Contains("_nor"))
                    {
                        normalPath = file;
                    }
                    else if (f.Contains("_disp") || f.Contains("_height"))
                    {
                        displacementPath = file;
                    }
                }
            }

            if (color != Color4.White)
            {
                Material.Color.Type = AnimationColorType.Constant;
                ((AnimationColorConstant)Material.Emission.Impl).Color = color;
            }

            SetColorMap(colorPath, opacityPath);
            SetNormalMap(normalglPath ?? normalPath, displacementPath);
            SetMaterialMap(metallicPath, roughnessPath, ambientOcclusionPath);

            ColorReference = null;
            NormalReference = null;
            MaterialReference = null;
            EmissiveReference = null;
        }

        private bool GetSingleColor(string path, out Color4 color)
        {
            using (var bitmap = BitmapTexture.Load(path))
            {
                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                unsafe
                {
                    var ptr = (byte*)bitmapData.Scan0;

                    var sr = ptr[2];
                    var sg = ptr[1];
                    var sb = ptr[0];
                    var sa = ptr[3];

                    color = new Color4(sr, sg, sb, sa);

                    for (var y = 0; y < bitmap.Height; y++)
                    {
                        for (var x = 0; x < bitmap.Width; x++)
                        {
                            var r = ptr[2];
                            var g = ptr[1];
                            var b = ptr[0];
                            var a = ptr[3];

                            if (r != sr || g != sg || b != sb || a != sa)
                            {
                                bitmap.UnlockBits(bitmapData);
                                return false;
                            }
                            ptr += 4;
                        }
                    }
                }
                bitmap.UnlockBits(bitmapData);
            }
            return true;
        }

        public override void Export(string dirpath)
        {
            Load();

            var colorPath = Path.Combine(dirpath, Name, "diffuse.png");
            var normalPath = Path.Combine(dirpath, Name, "normal.png");
            var displacementPath = Path.Combine(dirpath, Name, "displacement.png");
            var metallicPath = Path.Combine(dirpath, Name, "metallic.png");
            var roughnessPath = Path.Combine(dirpath, Name, "roughness.png");
            var ambientOcclusionPath = Path.Combine(dirpath, Name, "ambientOcclusion.png");
            var emissivePath = Path.Combine(dirpath, Name, "emissive.png");

            _ColorMap.Bitmap?.Save(colorPath);
            
            if (_NormalMap.Bitmap != null)
            {
                if (_HasDisplacement)
                {
                    BitmapTexture.Seperate31(NormalMap.Bitmap, out var normal, out var displacement);
                    normal.Save(normalPath);
                    displacement.Save(displacementPath);
                    normal.Dispose();
                    displacement.Dispose();
                }
                else _NormalMap.Bitmap.Save(normalPath);
            }
            if (_MaterialMap.Bitmap != null)
            {
                var i = 0;
                if (_HasMetallic) i++;
                if (_HasRoughness) i++;
                if (_HasAmbientOcclusion) i++;
                var bitmaps = new Bitmap[i];
                BitmapTexture.Seperate1111(MaterialMap.Bitmap, bitmaps);
                
                i = 0;
                if (_HasMetallic) bitmaps[i++].Save(metallicPath);
                if (_HasRoughness) bitmaps[i++].Save(roughnessPath);
                if (_HasAmbientOcclusion) bitmaps[i++].Save(ambientOcclusionPath);
                foreach (var bitmap in bitmaps) bitmap.Dispose();
            }
            _EmissiveMap.Bitmap?.Save(emissivePath);
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            if (_ColorReference != null) retains.Add(_ColorReference.Key);
            if (_NormalReference != null) retains.Add(_NormalReference.Key);
            if (_MaterialReference != null) retains.Add(_MaterialReference.Key);
            if (_EmissiveReference != null) retains.Add(_EmissiveReference.Key);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (element == _ColorReference || element == _NormalReference || element == _MaterialReference || element == _EmissiveReference)
            {
                from = this;
                return true;
            }

            from = null;
            return false;
        }

        private void BuildMap(BinaryWriter writer, AssetBuildPlatform platform, MaterialAsset reference, TextureSlot map)
        {
            writer.Write(reference != null);
            if (reference != null) BuildReference(writer, reference);
            else map.Build(writer, platform);
        }

        private void BuildImpl(BinaryWriter writer, AssetBuildPlatform platform)
        {
            BuildMap(writer, platform, _ColorReference, _ColorMap);
            BuildMap(writer, platform, _NormalReference, _NormalMap);
            BuildMap(writer, platform, _MaterialReference, _MaterialMap);
            BuildMap(writer, platform, _EmissiveReference, _EmissiveMap);

            Material.Build(writer);
        }

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            Load();

            if (writer == null)
            {
                var filePath = Path.Combine(path, GetBuildPlatformDirPath(platform), BuildPath);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    using (writer = new BinaryWriter(fs))
                    {
                        BuildImpl(writer, platform);
                    }
                }
            }
            else BuildImpl(writer, platform);
        }

        protected override bool SaveContent()
        {
            Load();

            Directory.CreateDirectory(Parent.ContentPath);

            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                writer.WriteStartElement("materialAsset");
                writer.WriteAttribute("version", Updater.AssetVersion);
                _TextureDescription.Save(writer, "format");
                writer.WriteAttribute("colorEncoding", _ColorEncoding);
                writer.WriteAttribute("normalEncoding", _NormalEncoding);
                writer.WriteAttribute("materialEncoding", _MaterialEncoding);
                writer.WriteAttribute("emissiveEncoding", _EmissiveEncoding);
                writer.WriteAttribute("hasOpacity", _HasOpacity);
                writer.WriteAttribute("hasDisplacement", _HasDisplacement);
                writer.WriteAttribute("hasMetallic", _HasMetallic);
                writer.WriteAttribute("hasRoughness", _HasRoughness);
                writer.WriteAttribute("hasAmbientOcclusion", _HasAmbientOcclusion);

                Material.Save(writer);

                writer.WriteEndElement();
            }
            _ColorMap.Save();
            _NormalMap.Save();
            _MaterialMap.Save();
            _EmissiveMap.Save();

            return true;
        }

        protected override void LoadContent()
        {
            var path = ContentXmlPath;

            if (File.Exists(path))
            {
                var doc = new XmlDocument();

                doc.Load(path);

                var node = doc.ChildNodes[1];

                if (node.LocalName != "materialAsset") throw new XmlException();

                Updater.ValidateTextureAsset(node);

                _TextureDescription.Load(node, "format");
                ColorEncoding = node.ReadAttributeEnum<TextureAssetColorEncoding>("colorEncoding");
                NormalEncoding = node.ReadAttributeEnum<TextureAssetComponentEncoding>("normalEncoding");
                MaterialEncoding = node.ReadAttributeEnum<TextureAssetComponentEncoding>("materialEncoding");
                EmissiveEncoding = node.ReadAttributeEnum<TextureAssetColorEncoding>("emissiveEncoding");
                HasOpacity = node.ReadAttributeBool("hasOpacity");
                HasDisplacement = node.ReadAttributeBool("hasDisplacement");
                HasMetallic = node.ReadAttributeBool("hasMetallic");
                HasRoughness = node.ReadAttributeBool("hasRoughness");
                HasAmbientOcclusion = node.ReadAttributeBool("hasAmbientOcclusion");

                Material.Load(node.GetChildNode("material"));

                ResetColorEncoding();
                ResetNormalEncoding();
                ResetMaterialEncoding();
                ResetEmissiveEncoding();

                _ColorMap.Load();
                _NormalMap.Load();
                _MaterialMap.Load();
                _EmissiveMap.Load();
            }
        }

        protected override void GetDataPaths(List<string> paths)
        {
            Load();

            paths.Add(ContentXmlPath);
            _ColorMap.GetDataPath(paths);
            _NormalMap.GetDataPath(paths);
            _MaterialMap.GetDataPath(paths);
            _EmissiveMap.GetDataPath(paths);
        }
    }
}

