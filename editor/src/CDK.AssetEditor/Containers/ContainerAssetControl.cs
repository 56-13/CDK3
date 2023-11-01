using System;
using System.Numerics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;

using CDK.Drawing;

using CDK.Assets.Support;
using CDK.Assets.Scenes;

using Graphics = CDK.Drawing.Graphics;
using GDIRectangle = System.Drawing.Rectangle;

namespace CDK.Assets.Containers
{
    public partial class ContainerAssetControl : UserControl
    {
        private Asset _Asset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset Asset
        {
            set
            {
                if (_Asset != value)
                {
                    if (_Asset != null)
                    {
                        childComboBox.Items.Clear();

                        foreach (var child in _Asset.Children)
                        {
                            RemoveTile(child);
                        }
                        
                        listView.Items.Clear();

                        _Asset.Children.ListChanged -= Children_ListChanged;
                    }
                    
                    _Asset = value;

                    _switchingTimer.Stop();

                    if (_Asset != null)
                    {
                        ResetAddChildEnabled();

                        for (var i = 0; i < _Asset.Children.Count; i++)
                        {
                            AddListViewItem(i);
                        }

                        if (listView.View == View.Tile)
                        {
                            foreach (var child in _Asset.Children)
                            {
                                RenderTile(child);
                            }
                        }

                        if (listView.Focused) _drawTimer.Start();

                        _Asset.Children.ListChanged += Children_ListChanged;
                    }
                    else
                    {
                        _drawTimer.Stop();
                    }

                    ApplyAuthority();

                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }

        public event EventHandler AssetChanged;

        private Graphics _graphics;
        private int _switchingDistance;
        private Timer _switchingTimer;
        private Timer _drawTimer;
        private bool _labelEditTag;

        private class TileRender
        {
            public Scene Scene;
            public LightSpace LightSpace;
            public Bitmap Capture;
        }
        private Dictionary<Asset, TileRender> _tileRenders;

        private LocaleStringExportForm _localeExportForm;

        private Asset[] SelectedAssets
        {
            get
            {
                var assets = new Asset[listView.SelectedItems.Count];
                for (var i = 0; i < assets.Length; i++) assets[i] = (Asset)listView.SelectedItems[i].Tag;
                return assets;
            }
        }

        private const int TileSize = 92;

        public ContainerAssetControl()
        {
            InitializeComponent();
            
            _tileRenders = new Dictionary<Asset, TileRender>();

            viewComboBox.DataSource = Enum.GetValues(typeof(View));
            viewComboBox.SelectedItem = listView.View;

            //TODO:좀 정리할 필요 있음

            _switchingTimer = new Timer
            {
                Interval = 500
            };
            _switchingTimer.Tick += SwitchingTimer_Tick;

            _drawTimer = new Timer
            {
                Interval = 50        //굳이 빠를 필요가 없음
            };
            _drawTimer.Tick += DrawTimer_Tick;

            localeImportFileDialog.Filter = localeExportFileDialog.Filter = FileFilters.ExcelOrCsv;

            _localeExportForm = new LocaleStringExportForm();

            listView.SmallImageList = AssetControl.Instance?.SmallImageList;
            listView.LargeImageList = AssetControl.Instance?.LargeImageList;

            if (AssetManager.IsCreated)
            {
                AssetManager.Instance.PropertyChanged += AssetManager_PropertyChanged;
                AssetManager.Instance.Purging += AssetManager_Purging;

                var target = new RenderTarget(TileSize, TileSize);
                target.Attach(FramebufferAttachment.ColorAttachment0, new RenderBuffer(TileSize, TileSize, RawFormat.Rgba8), true);
                target.Attach(FramebufferAttachment.DepthStencilAttachment, new RenderBuffer(TileSize, TileSize, RawFormat.Depth24Stencil8), true);

                _graphics = new Graphics(target);
            }

            Disposed += ContainerAssetControl_Disposed;
        }

        private void ContainerAssetControl_Disposed(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                _Asset.Children.ListChanged -= Children_ListChanged;
            }
            _switchingTimer.Dispose();
            _drawTimer.Dispose();

            foreach (var i in _tileRenders)
            {
                i.Value.LightSpace?.Dispose();
                i.Value.Capture.Dispose();
            }
            _tileRenders.Clear();

            if (AssetManager.IsCreated)
            {
                AssetManager.Instance.PropertyChanged -= AssetManager_PropertyChanged;
                AssetManager.Instance.Purging -= AssetManager_Purging;

                _graphics.Dispose();
            }

            _localeExportForm.Dispose();
        }

        private void ApplyAuthority()
        {
            var isDeveloper = AssetManager.Instance.IsDeveloper;

            removeToolStripMenuItem.Enabled =
                cutToolStripMenuItem.Enabled =
                pasteToolStripMenuItem.Enabled =
                rightButton.Enabled =
                rightJumpButton.Enabled =
                leftButton.Enabled =
                leftJumpButton.Enabled = _Asset != null && (_Asset.Type == AssetType.Project || _Asset.IsListed || isDeveloper);

            renameToolStripMenuItem.Enabled = isDeveloper;
        }

        private void AssetManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsDeveloper"))
            {
                ApplyAuthority();
                
                if (_Asset != null) ResetAddChildEnabled();
            }
        }

        private void AssetManager_Purging(object sender, EventArgs e)
        {
            if (_graphics == null) return;

            foreach (var i in _tileRenders)
            {
                i.Value.Capture.Dispose();
            }
            _tileRenders.Clear();
        }

        private void RewindTile(Asset asset)
        {
            if (_graphics != null && _tileRenders.TryGetValue(asset, out var render))
            {
                render.Scene.Rewind();

                RenderTile(render);
            }
        }

        private void UpdateTile(Asset asset, float delta)
        {
            if (_graphics != null && _tileRenders.TryGetValue(asset, out var render))
            {
                render.Scene.Update(TileSize, TileSize, ref render.LightSpace, delta, true);

                RenderTile(render);
            }
        }

        private void RenderTile(TileRender render)
        {
            _graphics.Focus();
            render.Scene.ResetCamera();
            render.Scene.FitCamera();
            render.Scene.Draw(_graphics, render.LightSpace);
            _graphics.Render();

            render.Capture = _graphics.Target.CaptureBitmap(0);
        }

        private bool RenderTile(Asset asset)
        {
            if (_graphics == null) return false;

            if (!_tileRenders.TryGetValue(asset, out var render))
            {
                var scene = asset.NewScene();

                if (scene == null) return false;

                scene.CameraGizmo = false;
                scene.Mode = SceneMode.Preview;

                if (scene.World is Ground ground) ground.GridVisible = false;

                render = new TileRender()
                {
                    Scene = scene
                };

                scene.Update(TileSize, TileSize, ref render.LightSpace, 0, true);

                _tileRenders.Add(asset, render);
            }

            RenderTile(render);

            return true;
        }

        private void RemoveTile(Asset asset)
        {
            if (_tileRenders.TryGetValue(asset, out var render))
            {
                render.Capture.Dispose();

                _tileRenders.Remove(asset);
            }
        }

        private void AddListViewItem(int index)
        {
            var item = new ListViewItem();

            var child = _Asset.Children[index];

            item.Text = child.Title;
            item.ImageKey = child.Type.ToString();
            item.SubItems.Add(child.GetType().Name);
            item.SubItems.Add(listView.View == View.Details ? child.Description : string.Empty);
            item.Tag = child;
            item.ForeColor = child.Linked ? Color.Black : Color.Red;

            listView.Items.Insert(index, item);

            if (listView.View == View.Tile)
            {
                RewindTile(child);
            }
        }

        private void ReorderListView()  //list view item 순서가 변경되지 않는 현상 발생, VS2008
        {
            View view = listView.View;
            switch (view)
            {
                case View.LargeIcon:
                case View.SmallIcon:
                case View.Tile:
                    listView.View = View.List;
                    listView.View = view;
                    break;
            }
            if (listView.SelectedIndices.Count != 0)
            {
                listView.EnsureVisible(listView.SelectedIndices[0]);
            }
        }

        private void ResetAddChildEnabled()
        {
            childComboBox.Items.Clear();
            if (_Asset.IsListed || AssetManager.Instance.IsDeveloper)
            {
                foreach (AssetType type in Enum.GetValues(typeof(AssetType)))
                {
                    if (_Asset.AddChildEnabled(type))
                    {
                        childComboBox.Items.Add(type);
                    }
                }
                if (childComboBox.Items.Count != 0)
                {
                    childComboBox.SelectedIndex = 0;
                }
            }
        }

        private void Children_ListChanged(object sender, ListChangedEventArgs e)
        {
            //================================================================================
            /*
            if (_Asset == null || sender != _Asset.Children)              //TODO:ERROR FIX (TEMPORARY)
            {
                return;             
            }
             */
            //================================================================================

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    AddListViewItem(e.NewIndex);
                    ReorderListView();

                    if (_Asset.Children.Count == 1)
                    {
                        ResetAddChildEnabled();
                    }
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor != null)
                    {
                        switch (e.PropertyDescriptor.Name)
                        {
                            case "Title":
                                listView.Items[e.NewIndex].Text = _Asset.Children[e.NewIndex].Title;
                                break;
                            case "Description":
                                listView.Items[e.NewIndex].SubItems[2].Text = _Asset.Children[e.NewIndex].Description;
                                break;
                        }
                    }
                    else
                    {
                        RemoveTile((Asset)listView.Items[e.NewIndex].Tag);
                        listView.Items.RemoveAt(e.NewIndex);

                        AddListViewItem(e.NewIndex);

                        if (_Asset.Children.Count == 1)
                        {
                            ResetAddChildEnabled();
                        }
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    RemoveTile((Asset)listView.Items[e.NewIndex].Tag);
                    listView.Items.RemoveAt(e.NewIndex);

                    if (_Asset.Children.Count == 0)
                    {
                        ResetAddChildEnabled();
                    }
                    break;
                case ListChangedType.ItemMoved:
                    {
                        var item = listView.Items[e.OldIndex];
                        listView.Items.RemoveAt(e.OldIndex);
                        listView.Items.Insert(e.NewIndex, item);

                        ReorderListView();
                    }
                    break;
                case ListChangedType.Reset:
                    foreach (ListViewItem item in listView.Items)
                    {
                        RemoveTile((Asset)item.Tag);
                    }
                    listView.Items.Clear();
                    for (var i = 0; i < _Asset.Children.Count; i++)
                    {
                        AddListViewItem(i);
                    }
                    ResetAddChildEnabled();
                    break;
            }
        }

        
        private void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (listView.View == View.Tile)
            {
                e.DrawBackground();

                if (e.Item.Selected)
                {
                    e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);
                }
                var asset = (Asset)e.Item.Tag;

                var frame = new GDIRectangle(e.Bounds.Left + 4, e.Bounds.Top + 18, e.Bounds.Width - 8, e.Bounds.Height - 22);

                var image = _tileRenders.TryGetValue(asset, out var render) && render.Capture != null ? render.Capture : AssetControl.Instance.LargeImageList.Images[asset.Type.ToString()];

                e.Graphics.DrawImage(image, frame);

                e.Graphics.SetClip(new GDIRectangle(e.Bounds.Left + 4, e.Bounds.Top + 4, e.Bounds.Width - 8, 12));
                using (Font font = new Font(Font.FontFamily, 9))
                {
                    e.Graphics.DrawString(e.Item.Text, font, Brushes.Black, e.Bounds.Left + 4, e.Bounds.Top + 4);
                }
            }
        }

        private void ViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (viewComboBox.Focused)
            {
                var view = (View)viewComboBox.SelectedItem;
                listView.OwnerDraw = view == View.Tile;
                listView.View = view;

                //TODO:제대로 그려지지 않는 현상 VS2008
                switch (view)
                {
                    case View.Tile:
                        if (_Asset != null)
                        {
                            foreach (var child in _Asset.Children)
                            {
                                RenderTile(child);
                            }
                        }
                        listView.View = View.List;
                        listView.View = view;
                        break;
                    case View.LargeIcon:
                    case View.SmallIcon:
                        listView.View = View.List;
                        listView.View = view;
                        break;
                    case View.Details:
                        foreach (ListViewItem item in listView.Items)
                        {
                            item.SubItems[2].Text = ((Asset)item.Tag).Description;
                        }
                        break;
                }
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = listView.GetItemAt(e.X, e.Y);

            if (item != null)
            {
                var asset = (Asset)item.Tag;

                AssetManager.Instance.Open(asset);
            }
        }

        private void LeftButton_Click(object sender, EventArgs e)
        {
            if (_switchingTimer.Enabled)
            {
                _switchingDistance--;
            }
            else
            {
                _switchingDistance = -1;
                _switchingTimer.Start();
            }
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            if (_switchingTimer.Enabled)
            {
                _switchingDistance++;
            }
            else
            {
                _switchingDistance = 1;
                _switchingTimer.Start();
            }
        }


        private void LeftJumpButton_Click(object sender, EventArgs e)
        {
            if (_switchingTimer.Enabled)
            {
                _switchingDistance -= 10;
            }
            else
            {
                _switchingDistance = -10;
                _switchingTimer.Start();
            }
        }

        private void RightJumpButton_Click(object sender, EventArgs e)
        {
            if (_switchingTimer.Enabled)
            {
                _switchingDistance += 10;
            }
            else
            {
                _switchingDistance = 10;
                _switchingTimer.Start();
            }
        }

        private void SwitchingTimer_Tick(object sender, EventArgs e)
        {
            if (_Asset != null && listView.SelectedIndices.Count > 0)
            {
                var indices = new int[listView.SelectedIndices.Count];
                listView.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);

                if (-_switchingDistance > indices[0])
                {
                    _switchingDistance = -indices[0];
                }
                else if (_switchingDistance >= listView.Items.Count - indices[indices.Length - 1])
                {
                    _switchingDistance = listView.Items.Count - indices[indices.Length - 1] - 1;
                }
                if (_switchingDistance < 0)
                {
                    foreach (var idx in indices)
                    {
                        _Asset.Children.Move(idx, idx + _switchingDistance);
                    }
                }
                else if (_switchingDistance > 0)
                {
                    for (var i = indices.Length - 1; i >= 0; i--)
                    {
                        _Asset.Children.Move(indices[i], indices[i] + _switchingDistance);
                    }
                }
            }
            _switchingTimer.Stop();
        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            if (listView.View == View.Tile && listView.SelectedItems.Count != 0 && _graphics != null)
            {
                var asset = (Asset)listView.SelectedItems[listView.SelectedItems.Count - 1].Tag;

                UpdateTile(asset, _drawTimer.Interval / 1000f);

                listView.Invalidate();
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null && (_Asset.IsListed || AssetManager.Instance.IsDeveloper) && childComboBox.SelectedItem != null)
            {
                var type = (AssetType)childComboBox.SelectedItem;

                if (_Asset.AddChildEnabled(type))
                {
                    _Asset.Children.Add(_Asset.NewChild(type));
                }
            }
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null && (_Asset.IsListed || AssetManager.Instance.IsDeveloper) && childComboBox.SelectedItem != null)
            {
                var type = (AssetType)childComboBox.SelectedItem;

                if (_Asset.AddChildEnabled(type))
                {
                    var filter = AssetControl.GetFilter(type);

                    if (filter != null)
                    {
                        openFileDialog.Filter = filter;

                        if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                        {
                            var filenames = new List<string>();
                            filenames.AddRange(openFileDialog.FileNames);
                            filenames.Sort();

                            foreach (var filename in filenames)
                            {
                                var asset = _Asset.NewChild(type);
                                _Asset.Children.Add(asset);

                                try
                                {
                                    asset.Import(filename);
                                }
                                catch (Exception uex)
                                {
                                    ErrorHandler.Record(uex);

                                    MessageBox.Show(filename + "파일을 로드할 수 없습니다.");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ChildComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (childComboBox.SelectedIndex >= 0)
            {
                var type = (AssetType)childComboBox.SelectedItem;

                importButton.Enabled = AssetControl.GetFilter(type) != null;
            }
        }

        private bool _raiseListViewSelectedIndexChanged = true;

        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_raiseListViewSelectedIndexChanged)
            {
                _raiseListViewSelectedIndexChanged = false;

                var h = Handle;

                BeginInvoke((Action)(() =>
                {
                    if (listView.View == View.Tile && listView.SelectedItems.Count != 0)
                    {
                        var asset = (Asset)listView.SelectedItems[listView.SelectedItems.Count - 1].Tag;

                        RewindTile(asset);

                        listView.Invalidate();
                    }
                    _raiseListViewSelectedIndexChanged = true;
                }));
            }
        }

        private void ListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            var asset = ((Asset)listView.Items[e.Item].Tag);

            if (e.Label != null)
            {
                if (_labelEditTag)
                {
                    asset.Tag = e.Label;
                }
                else
                {
                    asset.Name = e.Label;
                }
            }
            e.CancelEdit = true;

            listView.LabelEdit = false;

            listView.Items[e.Item].Text = asset.Title;

            copyToolStripMenuItem.Enabled = true;

            cutToolStripMenuItem.Enabled =
                pasteToolStripMenuItem.Enabled =
                removeToolStripMenuItem.Enabled = _Asset.Type == AssetType.Project || _Asset.IsListed || AssetManager.Instance.IsDeveloper;
        }

        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 0)
            {
                _labelEditTag = false;
                
                listView.LabelEdit = true;

                listView.SelectedItems[0].Text = ((Asset)listView.SelectedItems[0].Tag).Name;

                listView.SelectedItems[0].BeginEdit();

                copyToolStripMenuItem.Enabled = false;
                cutToolStripMenuItem.Enabled = false;
                pasteToolStripMenuItem.Enabled = false;
                removeToolStripMenuItem.Enabled = false;
            }
        }

        private void TagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 0)
            {
                _labelEditTag = true;

                listView.LabelEdit = true;

                listView.SelectedItems[0].Text = ((Asset)listView.SelectedItems[0].Tag).Tag;

                listView.SelectedItems[0].BeginEdit();

                copyToolStripMenuItem.Enabled = false;
                cutToolStripMenuItem.Enabled = false;
                pasteToolStripMenuItem.Enabled = false;
                removeToolStripMenuItem.Enabled = false;
            }
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Asset != null && listView.SelectedItems.Count != 0 && MessageBox.Show(this, "리소스를 삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var items = new ListViewItem[listView.SelectedItems.Count];
                listView.SelectedItems.CopyTo(items, 0);
                foreach (var item in items)
                {
                    if (((Asset)item.Tag).IsRetained(out var from, out var to))
                    {
                        AssetControl.ShowRetained(this, from, to);

                        return;
                    }
                }
                foreach (var item in items)
                {
                    _Asset.Children.Remove(((Asset)item.Tag));
                }
            }
        }

        private void ClipAssets(bool cut)
        {
            if (_Asset != null && listView.SelectedIndices.Count != 0)
            {
                var indices = new int[listView.SelectedIndices.Count];
                listView.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);

                _Asset.ClipChildren(indices, cut);
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipAssets(false);
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipAssets(true);
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _Asset?.PasteChildren();
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 0 && folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var dirpath = folderBrowserDialog.SelectedPath;

                    foreach (ListViewItem item in listView.SelectedItems)
                    {
                        var asset = (Asset)item.Tag;

                        asset.Export(dirpath);
                    }

                    MessageBox.Show(this, "내보내기가 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "내보내기를 진행할 수 없습니다.");
                }
            }
        }

        private void LocaleExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 0 && _localeExportForm.ShowDialog(this) == DialogResult.OK && localeExportFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var path = localeExportFileDialog.FileName;

                var timestamp = _localeExportForm.Timestamp;
                var keyLocale = _localeExportForm.KeyLocale;
                var targetLocales = _localeExportForm.TargetLocales;

                try
                {
                    var cells = Asset.LocaleExport(SelectedAssets, keyLocale, targetLocales, timestamp);

                    if (path.EndsWith(".xlsx"))
                    {
                        using (var excel = new Excel(path))
                        {
                            using (var form = new ExcelSheetForm())
                            {
                                form.Excel = excel;

                                if (form.ShowDialog(this) == DialogResult.OK)
                                {
                                    for (int r = 0; r < cells.Length; r++)
                                    {
                                        for (int c = 0; c < cells[r].Length; c++)
                                        {
                                            excel.SetCell(r, c, cells[r][c]);
                                        }
                                    }
                                    excel.Save();
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var fs = new FileStream(path, FileMode.Create))
                        {
                            using (var writer = new StreamWriter(fs, Encoding.UTF8))
                            {
                                foreach (var row in cells)
                                {
                                    writer.WriteLine(CSV.Make(row));
                                }
                            }
                        }
                    }

                    MessageBox.Show(this, "추출이 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    File.Delete(path);

                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    File.Delete(path);

                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "추출을 진행할 수 없습니다.");
                }
            }
        }

        private void LocaleImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 0 && localeImportFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var path = localeImportFileDialog.FileName;

                var collisions = new List<LocaleString.ImportCollision>();

                try
                {
                    string[][] cells;

                    if (path.EndsWith(".xlsx"))
                    {
                        using (var excel = new Excel(path))
                        {
                            using (var form = new ExcelSheetForm())
                            {
                                form.Excel = excel;
                                form.ReadOnly = true;

                                if (form.ShowDialog(this) == DialogResult.OK)
                                {
                                    cells = excel.GetCells();
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        var csv = File.ReadAllText(path, FileEncoding.GetFileEncoding(path));

                        cells = CSV.ParseAll(csv);
                    }

                    Asset.LocaleImport(SelectedAssets, cells, collisions);

                    if (collisions.Count != 0)
                    {
                        if (MessageBox.Show(this, "일부 텍스트가 내용이 다릅니다.") == DialogResult.OK)
                        {
                            using (var form = new LocaleStringImportCollisionForm(collisions))
                            {
                                form.ShowDialog(this);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "변환이 완료되었습니다.");
                    }
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "변환을 진행할 수 없습니다.");
                }
            }
        }


        private void BuildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 0 && folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var path = folderBrowserDialog.SelectedPath;

                    Asset.Build(SelectedAssets, path);

                    MessageBox.Show(this, "빌드가 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "빌드를 진행할 수 없습니다.");
                }
            }
        }

        private void UploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 0)
            {
                try
                {
                    Asset.Upload(SelectedAssets);

                    MessageBox.Show(this, "업로드가 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "업로드를 진행할 수 없습니다.");
                }
            }
        }

        private void ListView_Enter(object sender, EventArgs e)
        {
            if (_Asset != null) _drawTimer.Start();
        }

        private void ListView_Leave(object sender, EventArgs e)
        {
            _drawTimer.Stop();
        }
    }
}
