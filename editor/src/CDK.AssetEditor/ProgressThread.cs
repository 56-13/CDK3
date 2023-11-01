using System;
using System.Threading;
using System.Windows.Forms;

namespace CDK.Assets
{
    public class ProgressThread
    {
        private ProgressForm form;
        private int maximum;

        public ProgressThread(int maximum)
        {
            this.maximum = maximum;
        }

        public void Start()
        {
            Thread thread = new Thread(StartImpl);
            thread.SetApartmentState(ApartmentState.STA);
            lock (this)
            {
                thread.Start();

                Monitor.Wait(this);
            }
        }

        private void StartImpl()
        {
            form = new ProgressForm();
            form.Load += Form_Load;
            form.Maximum = maximum;
            Application.Run(form);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            lock (this)
            {
                Monitor.Pulse(this);
            }
        }

        public void Progress(int progress, string message)
        {
            form.Invoke((Action<int, string>)(ProgressImpl), progress, message);
        }

        private void ProgressImpl(int progress, string message)
        {
            form.Progress += progress;
            form.Message = message;
        }

        public void End()
        {
            form.Invoke((Action)(form.Dispose));
        }
    }
}
