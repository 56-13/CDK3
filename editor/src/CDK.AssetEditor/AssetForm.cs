using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets
{
    public partial class AssetForm : Form
    {
        public AssetForm()
        {
            InitializeComponent();

            if (AssetManager.IsCreated)
            {
                undoToolStripMenuItem.Enabled = AssetManager.Instance.UndoCommandEnabled;
                redoToolStripMenuItem.Enabled = AssetManager.Instance.RedoCommandEnabled;

                AssetManager.Instance.PropertyChanged += AssetForm_PropertyChanged;
            }
            Disposed += AssetForm_Disposed;
        }

        private void AssetForm_Disposed(object sender, EventArgs e)
        {
            if (AssetManager.IsCreated) AssetManager.Instance.PropertyChanged -= AssetForm_PropertyChanged;
        }

        private void AssetForm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "UndoCommandEnabled":
                    undoToolStripMenuItem.Enabled = AssetManager.Instance.UndoCommandEnabled;
                    break;
                case "RedoCommandEnabled":
                    redoToolStripMenuItem.Enabled = AssetManager.Instance.RedoCommandEnabled;
                    break;
            }
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetManager.Instance.UndoCommand();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetManager.Instance.RedoCommand();
        }
    }
}
