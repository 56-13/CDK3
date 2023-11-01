using System;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Assets.Scenes;

namespace CDK.Assets.Spawn
{
    public partial class SpawnAssetControl : UserControl
    {
        private SpawnAsset _Asset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpawnAsset Asset
        {
            set
            {
                if (_Asset != value)
                {
                    if (_Asset != null)
                    {
                        collisionSourceTextBox.DataBindings.Clear();
                        colliderUpDown.DataBindings.Clear();
                        colliderReferenceControl.DataBindings.Clear();
                        locationTypeComboBox.DataBindings.Clear();

                        collisionTargetsDataGridView.DataSource = null;
                        collisionTilesDataGridView.DataSource = null;

                        viewTreeView.DataBindings.Clear();
                        viewScreenControl.Scene = null;

                        attributeControl.Attribute = null;

                        _Asset.PropertyChanged -= Asset_PropertyChanged;
                    }

                    _Asset = value;

                    if (_Asset != null)
                    {
                        _originCollisionSource = _Asset.CollisionSource;

                        collisionSourceTextBox.DataBindings.Add("Text", _Asset, "CollisionSource", false, DataSourceUpdateMode.OnPropertyChanged);
                        colliderUpDown.DataBindings.Add("Value", _Asset, "Collider", false, DataSourceUpdateMode.OnPropertyChanged);
                        colliderReferenceControl.DataBindings.Add("SelectedElement", _Asset, "ColliderReference", true, DataSourceUpdateMode.OnPropertyChanged);
                        locationTypeComboBox.DataBindings.Add("SelectedItem", _Asset, "LocationType", false, DataSourceUpdateMode.OnPropertyChanged);

                        collisionTargetsDataGridView.DataSource = _Asset.CollisionTargets;
                        collisionTilesDataGridView.DataSource = _Asset.CollisionTiles;

                        //viewTreeView.DataBindings.Add("RootAsset", _Asset, "Project", true, DataSourceUpdateMode.Never);
                        viewTreeView.DataBindings.Add("SelectedAsset", _Asset, "View", true, DataSourceUpdateMode.OnPropertyChanged);

                        var scene = _Asset.View?.NewScene();
                        if (scene != null) scene.Mode = SceneMode.Preview;
                        viewScreenControl.Scene = scene;

                        attributeControl.Attribute = _Asset.Attribute;

                        _Asset.PropertyChanged += Asset_PropertyChanged;
                    }

                    subControl.Asset = _Asset;

                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }

        public event EventHandler AssetChanged;

        private string _originCollisionSource;
        private string _originCollisionTargetSource;
        private string _originCollisionTileValue;

        public SpawnAssetControl()
        {
            InitializeComponent();

            locationTypeComboBox.DataSource = Enum.GetValues(typeof(SpawnLocationType));

            collisionTargetsDataGridView.AutoGenerateColumns = false;
            collisionTilesDataGridView.AutoGenerateColumns = false;

            Disposed += SpawnAssetControl_Disposed;
        }

        private void SpawnAssetControl_Disposed(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                _Asset.PropertyChanged -= Asset_PropertyChanged;
            }
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "View")
            {
                var scene = _Asset.View?.NewScene();
                if (scene != null) scene.Mode = SceneMode.Preview;
                viewScreenControl.Scene = scene;
            }
        }

        private void CollisionSourceTextBox_Validated(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                if (collisionSourceTextBox.Modified && !string.IsNullOrEmpty(_originCollisionSource))
                {
                    var msg = "충돌 이름이 변경되었습니다. 다른 애셋 중 이전과 이름이 같은 충돌 이름을 새 이름으로 변경하시겠습니까?";

                    if (MessageBox.Show(this, msg, "전체 적용", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        _Asset.ReplaceCollisionSource(_originCollisionSource, collisionSourceTextBox.Text);
                    }
                }
                _originCollisionSource = collisionSourceTextBox.Text;
            }
            collisionSourceTextBox.Modified = false;
        }

        private void CollisionTargetsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (_Asset != null && e.ColumnIndex == 0)
            {
                var row = collisionTargetsDataGridView.Rows[e.RowIndex];

                if (!row.IsNewRow)
                {
                    _originCollisionTargetSource = (string)row.Cells[e.ColumnIndex].Value;
                }
            }
        }

        private void CollisionTargetsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_Asset != null && e.ColumnIndex == 0)
            {
                var row = collisionTargetsDataGridView.Rows[e.RowIndex];

                if (!row.IsNewRow && !string.IsNullOrEmpty(_originCollisionTargetSource)) 
                {
                    var newCollisionTargetSource = (string)row.Cells[e.ColumnIndex].Value;

                    if (_originCollisionTargetSource != newCollisionTargetSource)
                    {
                        var msg = "충돌 이름이 변경되었습니다. 다른 애셋 중 이전과 이름이 같은 충돌 이름을 새 이름으로 변경하시겠습니까?";

                        if (MessageBox.Show(this, msg, "전체 적용", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            _Asset.ReplaceCollisionSource(_originCollisionTargetSource, newCollisionTargetSource);
                        }

                        _originCollisionTargetSource = newCollisionTargetSource;
                    }
                }
            }        
        }

        private void CollisionTilesDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (_Asset != null)
            {
                var row = collisionTilesDataGridView.Rows[e.RowIndex];

                if (!row.IsNewRow)
                {
                    _originCollisionTileValue = (string)row.Cells[e.ColumnIndex].Value;
                }
            }
        }

        private void CollisionTilesDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_Asset != null)
            {
                var row = collisionTilesDataGridView.Rows[e.RowIndex];

                if (!row.IsNewRow && !string.IsNullOrEmpty(_originCollisionTileValue))
                {
                    var newCollisionTileValue = (string)row.Cells[e.ColumnIndex].Value;

                    if (_originCollisionTileValue != newCollisionTileValue)
                    {
                        var msg = "충돌 이름이 변경되었습니다. 다른 애셋 중 이전과 이름이 같은 충돌 이름을 새 이름으로 변경하시겠습니까?";

                        if (MessageBox.Show(this, msg, "전체 적용", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            //if (e.ColumnIndex == 0) _Asset.ReplaceCollisionTileName(_originCollisionTileValue, newCollisionTileValue);
                            //else _Asset.ReplaceCollisionTileElement(_originCollisionTileValue, newCollisionTileValue);
                        }

                        _originCollisionTileValue = newCollisionTileValue;
                    }
                }
            }
        }
    }
}
