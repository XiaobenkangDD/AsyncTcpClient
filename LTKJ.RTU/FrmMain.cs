using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using LTKJ.DataBase;
//using System.Threading;
using System.Globalization;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace LTKJ.RTU
{
    public partial class FrmMain : Form
    {
        #region 公共变量

        //获取App.config配置参数
        public string strConnection = System.Configuration.ConfigurationManager.ConnectionStrings["OracleConnection"].ToString();
        //       public int intPort = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["ListenPort"]); //UPD服务默认监听端口

        //设置TCP委托
        public delegate void invokeDelegate();

        //公共变量
        List<string> cmdList = new List<string>();

        //定时抄表
        //public Timer timerCollection = new Timer();
        public System.Windows.Forms.Timer timerControl = new System.Windows.Forms.Timer();//定时获取设备控制信息



        private static Dictionary<string, TcpClient> Dic_tcp = new Dictionary<string, TcpClient>();
        private static object locker = new object();

        Thread thread = null;
        #endregion


        #region 测试
        DataTable dtDevice = null;//所有站信息
        Crc16 crc16 = new Crc16();  //校验位对象
        #endregion
        public  TcpClient client1 = null;


        /// <summary>
        /// 窗体初始化
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();

        }


        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                //   Text += "[Port:" + intPort + "]";
                icnSys.Text = Text;
                //窗体控件事件

                tsbtnClose.Click += TsbtnClose_Click;

                //最小化通知区域和退出
                FormClosing += FrmMain_FormClosing;         //关闭窗体前事件
                SizeChanged += FrmMain_SizeChanged;         //窗体大小改变事件                
                tsbtnExit.Click += TsbtnExit_Click;         //右键菜单退出事件
                icnSys.Click += IcnSys_Click;               //点击图标事件           

        //        TcpClientInit();
                InitCmd();
       //         Connect(ref client1, "172.20.1.122", 502);             
                Test();


            }
            catch (Exception ex)
            {
                ShowSendMessage(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
                Common.WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            }
        }

        public void Test()
        {
            AsynClient client = new AsynClient();
            client.StartClient("127.0.0.1",11000);
        }


    public void TcpClientInit()
        {
            dtDevice = Common.GetTableData("CY4_WELL_INFO");
            for (int i = 0; i < dtDevice.Rows.Count; i++)
            {            
                string ip = dtDevice.Rows[i]["ip"].ToString();
                int port = int.Parse(dtDevice.Rows[i]["port"].ToString());
                TcpClient client = null;
           //     Connect(ref client, ip, port);
                Dic_tcp.Add(ip,client);

            }
        }

        public void InitCmd()
        {
            string full_cmd = "";
            string cmdType = "01";
            string baseCmd = "0000000006";
            full_cmd = cmdType+ baseCmd + "01 03" + DEC_to_HEX("10001") + DEC_to_HEX("18");        
            cmdList.Add(full_cmd);

            cmdType = "02";
            full_cmd = cmdType + baseCmd + "01 03" + DEC_to_HEX("10051") + DEC_to_HEX("18");
            cmdList.Add(full_cmd);

            cmdType = "03";
            full_cmd = cmdType + baseCmd + "01 03" + DEC_to_HEX("10001") + DEC_to_HEX("18");

            cmdList.Add(full_cmd);
        }

        public int StringToDec(string s)
        {
            int n = 0;
            for (int i = 0; i < s.Length; i++)
            {
                n = 10 * n + (s[i] - '0');
            }
            return n;

        }

        #region 窗体事件方法

        /// <summary>
        /// 双击图标事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void IcnSys_Click(object sender, EventArgs e)
        {
            try
            {
                Visible = true;//显示窗体
                ShowInTaskbar = true;//不在任务栏显示
                WindowState = FormWindowState.Normal;//窗体大小
            }
            catch (Exception ex)
            {
                Common.WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            }
        }
        /// <summary>
        /// 右键菜单退出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TsbtnExit_Click(object sender, EventArgs e)
        {
            try
            {
                Close();//关闭窗体
                Dispose();//释放资源
                Application.Exit();//退出程序
            }
            catch (Exception ex)
            {
                Common.WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            }
        }
        /// <summary>
        /// 窗口大小改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FrmMain_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                //最小化时只显示图标
                if (WindowState == FormWindowState.Minimized)
                {
                    ShowInTaskbar = false;//不在任务栏显示
                    icnSys.Visible = true;//在任务栏通知区域显示图标
                }
            }
            catch (Exception ex)
            {
                Common.WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            }
        }
        /// <summary>
        /// 关闭窗体时默认为最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = true;//取消关闭事件
                WindowState = FormWindowState.Minimized;//最小化窗口
                Visible = false;//不显示窗体
                icnSys.Visible = true;//在任务栏通知区域显示图标
            }
            catch (Exception ex)
            {
                Common.WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            }
        }

        /// <summary>
        /// 退出-关闭窗体
        /// 刘金鹏 2015-9-10
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TsbtnClose_Click(object sender, EventArgs e)
        {
            try
            {
                Close();//关闭窗体
                Dispose();//释放资源
                Application.Exit();//退出程序
            }
            catch (Exception ex)
            {
                Common.WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            }
        }

        #endregion


        #region 在消息区域显示数据
        private void ShowReceiveMessage(string message)
        {
            //txtMessage.Text += message + "\r\n";
            //txtMessage.Text = "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss =>") + message + txtMessage.Text;
            //接收的消息写入窗体文本框，多线程委托方式写入
            Invoke(new Action(delegate
            {
                textReceive.Text = "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss =>") + message + textReceive.Text;
            }));
        }

        private void ShowSendMessage(string message)
        {
            //txtMessage.Text += message + "\r\n";
            //txtMessage.Text = "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss =>") + message + txtMessage.Text;
            //接收的消息写入窗体文本框，多线程委托方式写入
            Invoke(new Action(delegate
            {
                textSend.Text = "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss =>") + message + textSend.Text;
            }));
        }
        #endregion


        /// <summary>
        /// 计时器，等待指定时间
        /// </summary>
        /// <param name="milliSecond"></param>
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {
                Application.DoEvents();
            }
        }

        #region  数据处理方法类
        public string DEC_to_HEX(string s)
        {
            //     string value = val.ToString("X");  int类型转为string 16进制类型
            int val = int.Parse(s);
            string value = val.ToString("X");
            string result = "";
            if (value.Length == 1)

            {
                result = "00 0" + value.ToUpper();
            }
            else if (value.Length == 2)
            {
                result = "00 " + value.ToUpper();
            }
            else if (value.Length == 3)
            {
                result = "0" + value;
            }
            else
            {
                result = value;
            }
            return result;
        }

        //增加Crc16校验
        public string Full_Crc(string cmd)
        {
            Crc16 crc = new Crc16();
            string result = "";
            byte[] bytes = Common.HexStringToByteArray(cmd);
            byte[] crc16 = crc.crc16_bytes(bytes, bytes.Count());
            string s = Common.ByteArrayToHexString(crc16, 0, crc16.Count());
            result = cmd + s;
            result = result.ToUpper().Replace(" ", "");
            bytes = Common.HexStringToByteArray(result);
            result = Common.ByteArrayToHexString(bytes, 0, bytes.Count());
            return result;
        }
        #endregion


        #region tcpClient异步

      
            #endregion
        }
}
