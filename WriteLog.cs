using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BaslerViewer
{
    class WriteLog
    {

        public static void WriteErrorLog(string log)
        {
            string path = Directory.GetCurrentDirectory() + "\\"+DateTime.Now.ToString("yy-MM-dd") + "\\Data\\ErrorLog.txt";
            if (!Directory.Exists(Directory.GetCurrentDirectory() +"\\"+ DateTime.Now.ToString("yy-MM-dd") + "\\Data"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() +"\\"+ DateTime.Now.ToString("yy-MM-dd") + "\\Data");
            }
            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine(log);
            sw.Flush();
            sw.Close();
        }

        public static void WriteRunLog(string log)
        {
            string path = Directory.GetCurrentDirectory() + "\\"+DateTime.Now.ToString("yy-MM-dd") + "\\Data\\RunLog.txt";
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\"+DateTime.Now.ToString("yy-MM-dd") + "\\Data"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\"+DateTime.Now.ToString("yy-MM-dd") + "\\Data");
            }
            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine(log);
            sw.Flush();
            sw.Close();
        }

    }
}
