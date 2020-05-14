using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Drawing.Drawing2D;



namespace BaslerViewer
{

    public partial class Form1 : Form
    {
        SerialPort com7;
        String data;
        baslerCamcs camera;
        baslerCamcs camera2;
        Bitmap saveBitmap1;
        Bitmap saveBitmap2;
        int cam1Times = 1;
        int cam2Times = 1;
        int w1,h1,w2,h2;
        
                       
        public Form1()
        {

            InitializeComponent();
            try
            {             
                com7 = new SerialPort("COM7");
                com7.BaudRate = 9600;
                com7.Parity = Parity.None;
                com7.StopBits = StopBits.One;
                com7.Handshake = Handshake.None;
                com7.DataBits = 8;
                //com7.RtsEnable = true;
                //com7.DtrEnable = true;
                com7.Open();
                com7.DiscardInBuffer();               
                camera = new baslerCamcs("22547782");
                camera2 = new baslerCamcs("23061596");
                camera.CameraImageEvent += Camera_CameraImageEvent;
                camera2.CameraImageEvent += Camera_CameraImageEvent2;
                LogOutput(DateTime.Now + ": 扫描枪设备初始化成功,摄像头初始化成功");
                WriteLog.WriteRunLog(DateTime.Now+": 扫描枪设备初始化成功,摄像头初始化成功");
                com7.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);
                
            }
            catch (Exception ex) 
            {            
                WriteLog.WriteErrorLog(DateTime.Now + ": " + ex.Message + "扫描枪设备初始化失败,摄像头初始化失败");                           
            }
        }

        private void Camera_CameraImageEvent(Bitmap bmp)
        {
            pictureBox1.BeginInvoke(new MethodInvoker(delegate
            {
                Bitmap old = pictureBox1.Image as Bitmap;
                pictureBox1.Image = bmp;
                saveBitmap1 = bmp;                 
                if (old != null)
                    old.Dispose();
            }));
        }

        private void Camera_CameraImageEvent2(Bitmap bmp)
        {
            pictureBox2.BeginInvoke(new MethodInvoker(delegate
            {
                Bitmap old = pictureBox2.Image as Bitmap;
                pictureBox2.Image = bmp;
                saveBitmap2 = bmp;               
                if (old != null)
                    old.Dispose();
            }));            
        }

        void Unable() 
        {
            But_Enter.Enabled = false;        
        }

        private void button1_Click(object sender, EventArgs e)
        {                       
            try
            {                            
                if (data != "")
                {
                    // Label_ShowBarcode.Text = data.Trim();
                    //Thread.Sleep(100);
                    saveBitmap1.Save(Application.StartupPath + @"\\" + DateTime.Now.ToString("yy-MM-dd") + @"\\" + data.Trim() + "_" + System.DateTime.Now.ToString("hhmmss") + "_Left.bmp", System.Drawing.Imaging.ImageFormat.Jpeg);
                    saveBitmap2.Save(Application.StartupPath + @"\\" + DateTime.Now.ToString("yy-MM-dd") + @"\\" + data.Trim() + "_" + System.DateTime.Now.ToString("hhmmss") + "_Right.bmp", System.Drawing.Imaging.ImageFormat.Jpeg);
                    WriteLog.WriteRunLog(DateTime.Now + ": 条码：" + data.Trim());
                    LogOutput(DateTime.Now + " : 条码S/N : " + data.Trim());
                }
                else 
                {
                    Label_ShowBarcode.Text = " - - - - - - - - - - - - ";
                    WriteLog.WriteRunLog(DateTime.Now + ": 条码为空值");
                    LogOutput(DateTime.Now + " : 条码S/N值为空 ");            
                }
                com7.DiscardInBuffer();
                data = "";                      
            }
            catch(Exception ex)
            {
                WriteLog.WriteErrorLog(DateTime.Now + ": 扫码失败 " + ex.Message);
                LogOutput(DateTime.Now + ": 未扫码,请检查扫描枪...... ");
            }           
            
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == 0x0100 && keyData == Keys.Space)
            { //0x0100即WM_KEYDOWN常数
                return true;//按空格键不作处理
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            DirInit();
            LogOutput(DateTime.Now + ": 目录文件初始化成功");
            WriteLog.WriteRunLog(DateTime.Now + ": 目录文件初始化成功");
            if (camera.CameraNumber > 0)
            {
                camera.CameraInit();
                camera.KeepShot();
            }
            else 
            {
                LogOutput(DateTime.Now + ": 未连接到相机1");
                WriteLog.WriteErrorLog(DateTime.Now + ": 未连接到相机1");
                Unable();
            }
            if (camera2.CameraNumber > 0)
            {
                camera2.CameraInit();
                camera2.KeepShot();
            }
            else
            {
                LogOutput(DateTime.Now + ": 未连接到相机2");
                WriteLog.WriteErrorLog(DateTime.Now + ": 未连接到相机2");
                Unable();
            }           
        }

        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (com7.IsOpen)
                {
                    data = com7.ReadExisting();
                    this.BeginInvoke(new System.Threading.ThreadStart(delegate()
                    {
                        Label_ShowBarcode.Text = data.Trim();
                    }));
                }
                else
                {
                    TimeSpan waitTime = new TimeSpan(0, 0, 0, 0, 50);
                    Thread.Sleep(waitTime);
                }
            }
            catch (Exception ex)
            {
                LogOutput(DateTime.Now + ": " + ex.Message);
                WriteLog.WriteErrorLog(DateTime.Now + ": "+ex.Message);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            camera.DestroyCamera();
            camera2.DestroyCamera();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            camera.Stop();
            camera.DestroyCamera();
            camera2.Stop();
            camera2.DestroyCamera();
        }

        private void But_Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                
            }
            else
            {
                LogOutput(DateTime.Now + ": 异常点击");
                WriteLog.WriteErrorLog(DateTime.Now + ": 异常点击");
            }
            
        }

        public void DirInit() 
        {
            if (!Directory.Exists(Application.StartupPath + @"\\" + DateTime.Now.ToString("yy-MM-dd")))
            {
                Directory.CreateDirectory(Application.StartupPath + @"\\" + DateTime.Now.ToString("yy-MM-dd"));            
            }
            w1 = pictureBox1.Width;
            h1 = pictureBox1.Height;
            w2 = pictureBox2.Width;
            h2 = pictureBox2.Height;
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
           
        }

        public void LogOutput(string str) 
        {
            richTextBox1.AppendText(str);
            richTextBox1.AppendText("\n");        

        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            richTextBox1.Focus();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cam1Times = Convert.ToInt16(comboBox1.Text);
            switch(cam1Times)
            {
                case 1:
                    pictureBox1.Size = new Size(w1, h1);
                    break;
                case 2:
                    pictureBox1.Size = new Size(2*w1, 2*h1);
                    break;
                case 3:
                    pictureBox1.Size = new Size(3*w1, 3*h1);
                    break;
                case 4:
                    pictureBox1.Size = new Size(4*w1, 4*h1);
                    break;
                case 5:
                    pictureBox1.Size = new Size(5*w1, 5*h1);
                    break;
                case 6:
                    pictureBox1.Size = new Size(6 * w1, 6 * h1);
                    break;
                case 7:
                    pictureBox1.Size = new Size(7 * w1, 7 * h1);
                    break;
                case 8:
                    pictureBox1.Size = new Size(8 * w1, 8 * h1);
                    break;
                case 9:
                    pictureBox1.Size = new Size(9 * w1, 9 * h1);
                    break;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            cam2Times = Convert.ToInt16(comboBox2.Text);
            switch (cam2Times)
            {
                case 1:
                    pictureBox2.Size = new Size(w2, h2);
                    break;
                case 2:
                    pictureBox2.Size = new Size(2 * w2, 2 * h2);
                    break;
                case 3:
                    pictureBox2.Size = new Size(3 * w2, 3 * h2);
                    break;
                case 4:
                    pictureBox2.Size = new Size(4 * w2, 4 * h2);
                    break;
                case 5:
                    pictureBox2.Size = new Size(5 * w2, 5 * h2);
                    break;
                case 6:
                    pictureBox2.Size = new Size(6 * w2, 6 * h2);
                    break;
                case 7:
                    pictureBox2.Size = new Size(7 * w2, 7 * h2);
                    break;
                case 8:
                    pictureBox2.Size = new Size(8 * w2, 8 * h2);
                    break;
                case 9:
                    pictureBox2.Size = new Size(9 * w2, 9 * h2);
                    break;
            }
        }

    }
}
