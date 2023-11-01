using System;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace CDK.SignalViewer
{
    public partial class SignalViewer : Form
    {
        private SignalLoader _loader;
        private BindingList<Signal> _signals;
        private AddForm _addForm;
        private string _Path;
        private string Path
        {
            set
            {
                _Path = value;
                Text = _Path != null ? $"{Title} {_Path}" : Title;
            }
            get => _Path;
        }

        private const string ConfigPath = "config.xml";

        private const string Title = "SignalViewer 1.0.0";

        public SignalViewer()
        {
            InitializeComponent();

            Text = Title;

            _loader = new SignalLoader(ConfigPath);
            _signals = new BindingList<Signal>();
            _signals.ListChanged += Signals_ListChanged;

            listBox.DataSource = _signals;
            listBox.DisplayMember = "Title";

            _addForm = new AddForm();

            Disposed += SignalViewer_Disposed;
        }

        private void SignalViewer_Disposed(object sender, EventArgs e)
        {
            _addForm.Dispose();
        }

        private void Signals_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.NewIndex == listBox.SelectedIndex && e.PropertyDescriptor?.Name == "Text")
            {
                UpdateText();
            }
        }

        private void UpdateText()
        {
            textBox.Clear();

            var item = (Signal)listBox.SelectedItem;

            if (item != null)
            {
                foreach (var str in item.Text.Split('\n'))
                {
                    if (str.StartsWith("@"))
                    {
                        textBox.SelectionColor = SystemColors.Highlight;
                        textBox.AppendText(str.Substring(1));
                    }
                    else
                    {
                        textBox.SelectionColor = SystemColors.WindowText;
                        textBox.AppendText(str);
                    }
                }
            }
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateText();
        }

        private void Add(string text, bool parse)
        {
            var signals = _loader.Load(text, parse);
            foreach (var signal in signals)
            {
                var flag = true;
                foreach (var other in _signals)
                {
                    if (other.Merge(signal))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag) _signals.Add(signal);
            }
        }

        private void Import(string[] files)
        {
            try
            {
                foreach (var file in files)
                {
                    var text = File.ReadAllText(file);
                    var parse = System.IO.Path.GetExtension(file) != ".mlog";
                    Add(text, parse);
                }
            }
            catch
            {
                MessageBox.Show(this, "파일을 불러올 수 없습니다.");
            }
        }

        private void Save(bool changePath)
        {
            if (_signals.Count == 0) return;

            if (changePath || Path == null)
            {
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK) Path = saveFileDialog.FileName;
                else return;
            }

            var strbuf = new StringBuilder();
            foreach (var signal in _signals)
            {
                strbuf.Append(signal.ToString());
                strbuf.Append('\n');
            }
            try
            {
                File.WriteAllText(Path, strbuf.ToString());
            }
            catch
            {
                MessageBox.Show(this, "파일을 저장할 수 없습니다.");
            }
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_addForm.ShowDialog(this) == DialogResult.OK)
            {
                Add(_addForm.Text, true);

                _addForm.Text = null;
            }
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                Import(openFileDialog.FileNames);
            }
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = listBox.SelectedIndex;

            if (index >= 0) _signals.RemoveAt(index);
        }

        private void UpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = listBox.SelectedIndex;
            if (index > 0)
            {
                var item = _signals[index];
                _signals.RemoveAt(index);
                _signals.Insert(index - 1, item);
                listBox.SelectedIndex = index - 1;
            }
        }

        private void DownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = listBox.SelectedIndex;
            if (index < _signals.Count - 1)
            {
                var item = _signals[index];
                _signals.RemoveAt(index);
                _signals.Insert(index + 1, item);
                listBox.SelectedIndex = index + 1;
            }
        }

        private void SortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_signals.Count > 1)
            {
                _signals.RaiseListChangedEvents = false;

                bool flag;
                do
                {
                    flag = false;
                    for (var i = 0; i < _signals.Count - 1; i++)
                    {
                        var a = _signals[i];
                        var b = _signals[i + 1];
                        if (a.Occurs < b.Occurs)
                        {
                            _signals.RemoveAt(i);
                            _signals.Insert(i + 1, a);
                            flag = true;
                        }
                    }
                } while (flag);

                _signals.RaiseListChangedEvents = true;
                _signals.ResetBindings();
            }
        }

        private void ListBox_DragOver(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            e.Effect = files != null ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void ListBox_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null) Import(files);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                _signals.Clear();

                try
                {
                    var path = openFileDialog.FileName;
                    var text = File.ReadAllText(path);

                    Add(text, false);

                    Path = path;
                }
                catch
                {
                    Path = null;

                    MessageBox.Show(this, "파일을 불러올 수 없습니다.");
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(false);
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(true);
        }
    }
}
