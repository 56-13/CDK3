using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Scenes
{
    public partial class SceneComponentSelectForm : Form
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Scene Scene
        {
            set => treeView.Scene = value;
            get => treeView.Scene;
        }
        public event EventHandler SceneChanged;

        public SceneComponentType[] Types
        {
            set => treeView.Types = value;
            get => treeView.Types;
        }
        public event EventHandler TypesChanged;

        [DefaultValue(false)]
        public bool Multiselect
        {
            set => treeView.Multiselect = value;
            get => treeView.Multiselect;
        }
        public event EventHandler MultiselectChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SceneComponent SelectedComponent
        {
            set => treeView.SelectedComponent = value;
            get => treeView.SelectedComponent;
        }
        public event EventHandler SelectedComponentChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SceneComponent[] SelectedComponents
        {
            set => treeView.SelectedComponents = value;
            get => treeView.SelectedComponents;
        }
        public event EventHandler SelectedComponentsChanged;

        public SceneComponentSelectForm()
        {
            InitializeComponent();

            treeView.SceneChanged += TreeView_SceneChanged;
            treeView.TypesChanged += TreeView_TypesChanged;
            treeView.MultiselectChanged += TreeView_MultiselectChanged;
            treeView.SelectedComponentChanged += TreeView_SelectedComponentChanged;
            treeView.SelectedComponentsChanged += TreeView_SelectedComponentsChanged;
        }

        private void TreeView_SceneChanged(object sender, EventArgs e)
        {
            SceneChanged?.Invoke(this, e);
        }

        private void TreeView_TypesChanged(object sender, EventArgs e)
        {
            TypesChanged?.Invoke(this, e);
        }

        private void TreeView_MultiselectChanged(object sender, EventArgs e)
        {
            MultiselectChanged?.Invoke(this, e);
        }

        private void TreeView_SelectedComponentChanged(object sender, EventArgs e)
        {
            SelectedComponentChanged?.Invoke(this, e);
        }

        private void TreeView_SelectedComponentsChanged(object sender, EventArgs e)
        {
            SelectedComponentsChanged?.Invoke(this, e);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }
    }
}
