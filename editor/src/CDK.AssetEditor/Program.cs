using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CDK.Assets
{
    static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        /// <summary>
        /// �ش� ���� ���α׷��� �� �������Դϴ�.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if DEBUG
            AllocConsole();
#endif
            AssetManager.CreateShared();
            AssetControl.CreateShared();

            Application.Run(new AssetEditor());

            AssetControl.DisposeShared();
            AssetManager.DisposeShared();
        }
    }
}
