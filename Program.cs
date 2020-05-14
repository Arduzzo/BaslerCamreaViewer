using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaslerViewer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool ss;
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out ss);
            if (ss)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else 
            {
                MessageBox.Show(null, "名为BaslerViewer的应用程序已经在运行，请不要同时运行多个本程序。\n\n这个程序即将退出。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //   提示信息，可以删除。   
                Application.Exit();//退出程序      
            }
        }
    }
}
