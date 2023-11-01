using System;
using System.Numerics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Drawing;

using CDK.Assets.Scenes;

using Bitmap = System.Drawing.Bitmap;

namespace CDK.Assets.Terrain
{
    public partial class TerrainSurfaceSelectControl : UserControl
    {
        private TerrainAsset _Asset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TerrainAsset Asset
        {
            set
            {
                if (_Asset != value)
                {
                    _scene = null;
                    _sceneObject = null;

                    if (_Asset != null)
                    {
                        _Asset.Surfaces.ListChanged -= Surfaces_ListChanged;
                    }

                    _Asset = value;

                    if (_Asset != null)
                    {
                        _Asset.Surfaces.ListChanged += Surfaces_ListChanged;
                    }

                    ValidateSurfaces();

                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }

        public event EventHandler AssetChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TerrainSurface[] SelectedSurfaces
        {
            set
            {
                var flag = false;
                if (value != null && value.Length != 0)
                {
                    if (_Asset == null) throw new InvalidOperationException();

                    foreach (var v in value)
                    {
                        if (v.Parent != _Asset) throw new InvalidOperationException();
                    }
                    TerrainSurface[] vs;
                    if (!_Multiselect && value.Length != 1)
                    {
                        vs = new TerrainSurface[1];
                        vs[0] = value[value.Length - 1];
                    }
                    else
                    {
                        vs = value;
                    }
                    if (_selections.Count != vs.Length)
                    {
                        flag = true;
                    }
                    else
                    {
                        for (var i = 0; i < _selections.Count; i++)
                        {
                            if (_selections[i] != vs[i])
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        _selections.Clear();
                        _selections.AddRange(vs);
                    }
                }
                else
                {
                    if (_selections.Count != 0)
                    {
                        _selections.Clear();
                        flag = true;
                    }
                }
                if (flag)
                {
                    ValidateSurfaceSelections();

                    SelectedSurfaceChanged?.Invoke(this, EventArgs.Empty);
                    SelectedSurfacesChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _selections.ToArray();
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TerrainSurface SelectedSurface
        {
            set
            {
                if (value != null && (_Asset == null || value.Parent != _Asset))
                {
                    throw new InvalidOperationException();
                }
                SetSelectedSurface(value);
            }
            get => _selections.Count != 0 ? _selections[_selections.Count - 1] : null;
        }

        private void SetSelectedSurface(TerrainSurface value)
        {
            if (_selections.Count > 1 || SelectedSurface != value)
            {
                _selections.Clear();
                if (value != null)
                {
                    _selections.Add(value);
                }
                ValidateSurfaceSelections();

                SelectedSurfaceChanged?.Invoke(this, EventArgs.Empty);
                SelectedSurfacesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler SelectedSurfaceChanged;
        public event EventHandler SelectedSurfacesChanged;
        public event EventHandler Confirm;

        private bool _Multiselect;

        [DefaultValue(false)]
        public bool Multiselect
        {
            set
            {
                if (_Multiselect != value)
                {
                    _Multiselect = value;
                    if (_selections.Count > 1)
                    {
                        _selections.RemoveRange(0, _selections.Count - 1);
                        SelectedSurfacesChanged?.Invoke(this, EventArgs.Empty);
                    }
                    MultiselectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Multiselect;
        }
        public event EventHandler MultiselectChanged;

        private Scene _scene;
        private SphereObject _sceneObject;
        private Graphics _graphics;
        private LightSpace _lightSpace;
        private List<TerrainSurface> _selections;

        private const int SurfaceSize = 80;

        public TerrainSurfaceSelectControl()
        {
            InitializeComponent();

            _selections = new List<TerrainSurface>();

            if (AssetManager.IsCreated)
            {
                var target = new RenderTarget(SurfaceSize, SurfaceSize);
                target.Attach(FramebufferAttachment.ColorAttachment0, new RenderBuffer(SurfaceSize, SurfaceSize, RawFormat.Rgba8), true);
                target.Attach(FramebufferAttachment.DepthAttachment, new RenderBuffer(SurfaceSize, SurfaceSize, RawFormat.DepthComponent16), true);
                _graphics = new Graphics(target);
            }

            Disposed += TerrainSurfaceSelectControl_Disposed;
        }

        private void TerrainSurfaceSelectControl_Disposed(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                _Asset.Surfaces.ListChanged -= Surfaces_ListChanged;

                while (panel.Controls.Count > 0)
                {
                    var button = (Button)panel.Controls[panel.Controls.Count - 1];
                    var surface = (TerrainSurface)button.Tag;
                    surface.Material.PropertyChanged -= Material_PropertyChanged;
                    button.Dispose();
                }
            }
            _graphics?.Dispose();
            _lightSpace?.Dispose();
        }

        private Bitmap CreateSurfaceImage(TerrainSurface surface)
        {
            if (_graphics == null) return null;

            if (_scene == null)
            {
                _sceneObject = new SphereObject();
                _scene = _Asset.NewDefaultScene(_sceneObject);
            }
            _sceneObject.Material.CopyFrom(surface.Material, true);

            _scene.Update(SurfaceSize, SurfaceSize, ref _lightSpace, 0, true);
            _scene.Draw(_graphics, _lightSpace);

            _graphics.Render();

            var capture = _graphics.Target.CaptureBitmap(0);

            return capture;
        }

        private void ValidateSurfaces()
        {
            panel.SuspendLayout();

            if (_Asset != null)
            {
                for (var i = 0; i < _Asset.Surfaces.Count; i++)
                {
                    var nextSurface = _Asset.Surfaces[i];

                    var isNew = true;

                    if (i < panel.Controls.Count)
                    {
                        var prevSurface = (TerrainSurface)panel.Controls[i].Tag;

                        if (prevSurface != nextSurface)
                        {
                            for (var j = i + 1; j < panel.Controls.Count; j++)
                            {
                                prevSurface = (TerrainSurface)panel.Controls[j].Tag;

                                if (prevSurface == nextSurface)
                                {
                                    panel.Controls.SetChildIndex(panel.Controls[j], i);
                                    isNew = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (isNew)
                    {
                        var button = new Button
                        {
                            Tag = nextSurface,
                            Size = new System.Drawing.Size(SurfaceSize, SurfaceSize),
                            Image = CreateSurfaceImage(nextSurface),
                            BackColor = _selections.Contains(nextSurface) ? System.Drawing.Color.LightBlue : System.Drawing.Color.Black
                        };
                        button.Click += SurfaceButton_Click;
                        button.DoubleClick += SurfaceButton_DoubleClick;
                        nextSurface.Material.PropertyChanged += Material_PropertyChanged;

                        panel.Controls.Add(button);
                        panel.Controls.SetChildIndex(button, i);
                    }
                }
                while (panel.Controls.Count > _Asset.Surfaces.Count)
                {
                    var button = (Button)panel.Controls[panel.Controls.Count - 1];
                    var surface = (TerrainSurface)button.Tag;
                    surface.Material.PropertyChanged -= Material_PropertyChanged;
                    button.Dispose();
                }
                panel.ResumeLayout();
                {
                    var flag = 0;
                    var i = 0;
                    while (i < _selections.Count)
                    {
                        if (!_Asset.Surfaces.Contains(_selections[i]))
                        {
                            _selections.RemoveAt(i);

                            if (flag != 2)
                            {
                                flag = i == _selections.Count - 1 ? 2 : 1;
                            }
                        }
                        else
                        {
                            i++;
                        }
                    }
                    if (flag != 0)
                    {
                        if (flag == 2)
                        {
                            SelectedSurfaceChanged?.Invoke(this, EventArgs.Empty);
                        }
                        SelectedSurfacesChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            else
            {
                while (panel.Controls.Count > 0)
                {
                    var button = (Button)panel.Controls[panel.Controls.Count - 1];
                    var surface = (TerrainSurface)button.Tag;
                    surface.Material.PropertyChanged -= Material_PropertyChanged;
                    button.Dispose();
                }
                panel.ResumeLayout();

                _selections.Clear();

                SelectedSurfaceChanged?.Invoke(this, EventArgs.Empty);
                SelectedSurfacesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ValidateSurfaceSelections()
        {
            foreach (Button button in panel.Controls)
            {
                button.BackColor = _selections.Contains((TerrainSurface)button.Tag) ? System.Drawing.Color.LightBlue : System.Drawing.Color.Black;
            }
        }

        private HashSet<Texturing.Material> _materialChangings = new HashSet<Texturing.Material>();
        private void Material_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_materialChangings.Count == 0)
            {
                AssetManager.Instance.Invoke(() =>
                {
                    foreach (Button button in panel.Controls)
                    {
                        var surface = (TerrainSurface)button.Tag;

                        if (_materialChangings.Contains(surface.Material))
                        {
                            button.Image = CreateSurfaceImage(surface);
                        }
                    }
                    _materialChangings.Clear();
                });
            }
            _materialChangings.Add((Texturing.Material)sender);
        }

        private void Surfaces_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.PropertyDescriptor == null)
            {
                ValidateSurfaces();
            }
        }

        private void SurfaceButton_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var surface = (TerrainSurface)button.Tag;

            if (_Multiselect)
            {
                if (_selections.Contains(surface))
                {
                    _selections.Remove(surface);

                    button.BackColor = System.Drawing.Color.White;
                }
                else
                {
                    _selections.Add(surface);

                    button.BackColor = System.Drawing.Color.Magenta;
                }
                SelectedSurfaceChanged?.Invoke(this, EventArgs.Empty);
                SelectedSurfacesChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                SetSelectedSurface(surface);
            }
        }

        private void SurfaceButton_DoubleClick(object sender, EventArgs e)
        {
            Confirm?.Invoke(this, EventArgs.Empty);
        }
    }
}
