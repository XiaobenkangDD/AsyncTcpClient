using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

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
        public System.Windows.Forms.Timer timerCollect = new System.Windows.Forms.Timer();//定时获取设备控制信息



        private static Dictionary<string, Socket> Dic_tcp = new Dictionary<string, Socket>();
        private static object locker = new object();

        Thread thread = null;
        DataTable dtDevice = null;//所有站信息
        #endregion

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

                InitCmd();
                TcpClientInit();

                //timerCollect.Interval = 10*1000;
                //timerCollect.Tick += Collect; //定时获取设备控制信息
                //timerCollect.Start();

                thread = new Thread(new ThreadStart(Collect));
                thread.Start();

                //     Test();


                //         Control.CheckForIllegalCrossThreadCalls = false;


            }
            catch (Exception ex)
            {
                ShowSendMessage(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
                Common.WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            }
        }

        public void Collect()
        {
            while (true)
            {
                try
                {
                    for (int i = 0; i < cmdList.Count; i++)
                    {
                        string cmd = cmdList[i];
                        sendCmd(cmd);
                        Delay(3000);
                    }

                    Delay(5*60*1000);
                    Invoke(new Action(delegate
                    {
                        textReceive.Text = "";
                        textSend.Text = "";
                    }));

                }
                catch (Exception ex)
                {
                    ShowSendMessage(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
                    Common.WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
                }
            }

        }

        public void Test()
        {
            string result = "05 00 00 00 00 04 01 02 01 09";
            TempData tempData = new TempData();
            result = result.Replace(" ", "");
            if (result.Length < 20) return;
            string type = result.Substring(0, 2);
            string lengthStr = result.Substring(16, 2);
            int length = Convert.ToInt32(lengthStr, 16) * 2;
            string data = result.Substring(18, length);
            if (type == "05")
            {

                tempData.cmdType = "5";
                string s = Convert.ToString(Convert.ToInt32(data), 2).PadLeft(8, '0');
                tempData.D1 = int.Parse(s.Substring(7, 1));
                tempData.D2 = int.Parse(s.Substring(6, 1));

            }
        }


        public void TcpClientInit()
        {
            try
            {
                Invoke(new Action(delegate
                {
                    textReceive.Text = "";
                    textSend.Text = "";
                }));

                dtDevice = Common.GetTableData("UNIT", "and ip is not null", "", "name", "asc");
                for (int i = 0; i < dtDevice.Rows.Count; i++)
                {
                    Socket client = null;
                    string ip = dtDevice.Rows[i]["ip"].ToString();
                    int port = int.Parse(dtDevice.Rows[i]["port"].ToString());
                    AsyncConnect(client, ip, port);

                }
            }
            catch (Exception ex)
            {
                ShowSendMessage(MethodBase.GetCurrentMethod().Name + ex.Message);
            }
        }

        public void InitCmd()
        {
            string full_cmd = "";
            string baseCmd = "0000000006";

            string cmdType = "01";
            full_cmd = cmdType + baseCmd + "01 03" + DEC_to_HEX("10001") + DEC_to_HEX("18");
            full_cmd = full_cmd.Replace(" ", "");
            cmdList.Add(full_cmd);

            cmdType = "02";
            full_cmd = cmdType + baseCmd + "01 03" + DEC_to_HEX("10051") + DEC_to_HEX("18");
            full_cmd = full_cmd.Replace(" ", "");
            cmdList.Add(full_cmd);

            cmdType = "03";
            full_cmd = cmdType + baseCmd + "01 03" + DEC_to_HEX("10001") + DEC_to_HEX("18");
            full_cmd = full_cmd.Replace(" ", "");
            cmdList.Add(full_cmd);

            cmdType = "04";
            full_cmd = cmdType + baseCmd + "01 02" + DEC_to_HEX("10001") + DEC_to_HEX("2");
            full_cmd = full_cmd.Replace(" ", "");
            cmdList.Add(full_cmd);

            cmdType = "05";
            full_cmd = cmdType + baseCmd + "01 02" + DEC_to_HEX("1") + DEC_to_HEX("1");
            full_cmd = full_cmd.Replace(" ", "");
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
                //           DisConnected();
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
                //         DisConnected();
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





        #region 在消息区域显示数据 使用委托，防止界面卡
        private delegate void ShowReceiveMessageDelegate(string message);
        private void ShowReceiveMessage(string message)
        {
            if (this.InvokeRequired)
            {
                ShowReceiveMessageDelegate showReceiveMessageDelegate = ShowReceiveMessage;
                this.Invoke(showReceiveMessageDelegate, new object[] { message });
            }
            else
            {
                textReceive.Text = "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss =>") + message + textReceive.Text;
            }
        }

        private delegate void ShowSendMessageDelegate(string message);
        private void ShowSendMessage(string message)
        {
            if (this.InvokeRequired)
            {
                ShowSendMessageDelegate showSendMessageDelegate = ShowSendMessage;
                this.Invoke(showSendMessageDelegate, new object[] { message });
            }
            else
            {
                textSend.Text = "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss =>") + message + textSend.Text;
            }
            Delay(10);
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

        /// <summary>
        /// 连接到服务器
        /// </summary>
        public void AsyncConnect(Socket client, string ip, int port)
        {
            bool isConnected = true;
            try
            {
                //端口及IP
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ip), port);
                //创建套接字
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //开始连接到服务器
                client.BeginConnect(ipe, asyncResult =>
                {
                    try
                    {
                        client.EndConnect(asyncResult);
                        //向服务器发送消息
                        //     AsyncSend(client, "010000000006010327110012");
                        //接受消息
                        AsyncReceive(client);
                    }
                    catch (Exception ex)
                    {
                        ShowSendMessage("EndConnect连接异常：" + ex.Message);
                        isConnected = false;
                    }


                }, null);
                if (isConnected)
                {
                    lock (locker)
                    {
                        Dic_tcp.Add(ip, client);
                    }

                }
            }
            catch (Exception ex)
            {
                ShowSendMessage("连接异常：" + ex.Message);
            }

        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="message"></param>
        public void AsyncSend(Socket socket, string message)
        {
            if (socket == null || message == string.Empty) return;
            //编码
            ShowSendMessage(message);
            //    byte[] data = Encoding.UTF8.GetBytes(message);
            byte[] data = Common.HexStringToByteArray(message);
            try
            {

                socket.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
                {
                    //完成发送消息
                    int length = socket.EndSend(asyncResult);
                }, null);
            }
            catch (Exception ex)
            {
                ShowSendMessage("发送数据异常 " + ex.Message);
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="socket"></param>
        public void AsyncReceive(Socket socket)
        {
            byte[] data = new byte[1024];
            int length = 0;
            string result = "";

            try
            {
                IPEndPoint ip = (IPEndPoint)socket.RemoteEndPoint;
                string AdrFamily = ip.AddressFamily.ToString();//地址类型
                string IpStr = ip.Address.ToString();//IP地址点分表达方式
                string IpPort = ip.Port.ToString();//IP地址端口      

                //开始接收数据
                socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
                asyncResult =>
                {
                    try
                    {
                        length = socket.EndReceive(asyncResult);
                    }
                    catch (Exception)
                    {
                        AsyncReceive(socket);
                    }
                    // result = Encoding.UTF8.GetString(data);
                    result = Common.ByteArrayToHexString(data, 0, length);
                    ShowReceiveMessage(result);
                    InsertData(IpStr, IpPort, result);
                    AsyncReceive(socket);
                }, null);
            }
            catch (Exception ex)
            {
                ShowSendMessage("接收数据异常 " + ex.Message);
            }
        }
        #endregion

        public void sendCmd(string cmd)
        {
            string IpStr = "";
            string IpPort = "";
            try
            {

                lock (locker)
                {
                    if (Dic_tcp.Count > 0)
                    {
                        List<string> list = new List<string>(Dic_tcp.Keys);
                        for (int i = 0; i < Dic_tcp.Count; i++)
                        {

                            try
                            {
                                Socket client = Dic_tcp[list[i]];
                                IPEndPoint ip = (IPEndPoint)client.RemoteEndPoint;
                                string AdrFamily = ip.AddressFamily.ToString();//地址类型
                                IpStr = ip.Address.ToString();//IP地址点分表达方式
                                IpPort = ip.Port.ToString();//IP地址端口      

                                if (client == null)
                                {
                                    Dic_tcp.Remove(IpStr);
                                }
                                else if (!client.Connected)
                                {
                                    Dic_tcp.Remove(IpStr);
                                    AsyncConnect(client, IpStr, int.Parse(IpPort));
                                }
                                else
                                {
                                    AsyncSend(client, cmd);
                                }

                            }
                            catch (Exception ex)
                            {
                                ShowSendMessage(IpStr + ":" + IpPort + "发送指令出错： " + ex.Message);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowSendMessage(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
                Common.WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string cmd = textCmd.Text;
            sendCmd(cmd);
        }

        public void DisConnected()
        {
            List<string> list = new List<string>(Dic_tcp.Keys);
            for (int i = 0; i < Dic_tcp.Count; i++)
            {
                if (Dic_tcp[list[i]] != null)
                {
                    Dic_tcp[list[i]].Shutdown(SocketShutdown.Both);
                    Dic_tcp[list[i]].Close();
                }

            }
        }

        /// <summary>  
        /// 字符串ABCD转为浮点数 
        /// </summary>  
        /// <param name="s" />字符串
        /// <returns>返回浮点数</returns> 
        private double StringToFloat(string s)
        {
            s = s.Replace(" ", "");
            Byte[] bytes = Common.HexStringToByteArray(s);
            bytes = bytes.Reverse().ToArray();  //反序
            float fNum = BitConverter.ToSingle(bytes, 0);
            return Math.Round(fNum, 2);
        }

        public void InsertData(string IpStr, string IpPort, string result)
        {
            try
            {
                TempData tempData = new TempData();
                result = result.Replace(" ", "");
                if (result.Length < 20) return;
                string type = result.Substring(0, 2);
                string lengthStr = result.Substring(16, 2);
                int length = Convert.ToInt32(lengthStr, 16) * 2;
                string data = result.Substring(18, length);
                if (type == "01")
                {
                    tempData.DeviceIp = IpStr;
                    tempData.DevicePort = IpPort;
                    tempData.cmdType = "1";
                    tempData.D1 = StringToFloat(data.Substring(0, 8));
                    tempData.D2 = StringToFloat(data.Substring(8, 8));
                    tempData.D3 = StringToFloat(data.Substring(16, 8));
                    tempData.D4 = StringToFloat(data.Substring(24, 8));
                    tempData.D5 = StringToFloat(data.Substring(32, 8));
                    tempData.D6 = StringToFloat(data.Substring(40, 8));
                    tempData.D7 = StringToFloat(data.Substring(48, 8));
                    tempData.D8 = StringToFloat(data.Substring(56, 8));
                    tempData.D9 = StringToFloat(data.Substring(64, 8));
                }
                if (type == "02")
                {
                    tempData.DeviceIp = IpStr;
                    tempData.DevicePort = IpPort;
                    tempData.cmdType = "2";
                    tempData.D1 = StringToFloat(data.Substring(0, 8));
                    tempData.D2 = StringToFloat(data.Substring(8, 8));
                    tempData.D3 = StringToFloat(data.Substring(16, 8));
                    tempData.D4 = StringToFloat(data.Substring(24, 8));
                    tempData.D5 = StringToFloat(data.Substring(32, 8));
                    tempData.D6 = StringToFloat(data.Substring(40, 8));
                    tempData.D7 = StringToFloat(data.Substring(48, 8));
                    tempData.D8 = StringToFloat(data.Substring(56, 8));
                    tempData.D9 = StringToFloat(data.Substring(64, 8));
                }
                if (type == "03")
                {
                    tempData.DeviceIp = IpStr;
                    tempData.DevicePort = IpPort;
                    tempData.cmdType = "3";
                    tempData.D1 = StringToFloat(data.Substring(0, 8));
                    tempData.D2 = StringToFloat(data.Substring(8, 8));
                    tempData.D3 = StringToFloat(data.Substring(16, 8));
                    tempData.D4 = StringToFloat(data.Substring(24, 8));
                    tempData.D5 = StringToFloat(data.Substring(32, 8));
                    tempData.D6 = StringToFloat(data.Substring(40, 8));
                    tempData.D7 = StringToFloat(data.Substring(48, 8));
                    tempData.D8 = StringToFloat(data.Substring(56, 8));
                    tempData.D9 = StringToFloat(data.Substring(64, 8));
                }
                if (type == "04")
                {
                    tempData.DeviceIp = IpStr;
                    tempData.DevicePort = IpPort;
                    tempData.cmdType = "4";
                    string s = Convert.ToString(Convert.ToInt32(data), 2).PadLeft(8, '0');
                    tempData.D1 = int.Parse(s.Substring(7, 1));
                    tempData.D2 = int.Parse(s.Substring(6, 1));
                }
                if (type == "05")
                {
                    tempData.DeviceIp = IpStr;
                    tempData.DevicePort = IpPort;
                    tempData.cmdType = "5";
                    string s = Convert.ToString(Convert.ToInt32(data), 2).PadLeft(8, '0');
                    tempData.D1 = int.Parse(s.Substring(7, 1));
                }

                AddTempData(tempData);
            }
            catch (Exception ex)
            {
                ShowSendMessage(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
                Common.WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            }
        }

        private void AddTempData(TempData mod)
        {
            string sqlText = "";
            try
            {

                #region 仪表参数信息
                //ModelDevice mod = new ModelDevice();  //仪表参数信息
                mod.DeviceId = mod.DeviceId == null ? "" : mod.DeviceId;                //设备编号ID
                mod.DeviceName = mod.DeviceName == null ? "" : mod.DeviceName;          //设备名称
                mod.DeviceIp = mod.DeviceIp == null ? "" : mod.DeviceIp;
                mod.DevicePort = mod.DevicePort == null ? "" : mod.DevicePort;
                mod.cmdType = mod.cmdType == null ? "" : mod.cmdType;
                mod.D1 = mod.D1 == null ? 0 : mod.D1;
                mod.D2 = mod.D2 == null ? 0 : mod.D2;
                mod.D3 = mod.D3 == null ? 0 : mod.D3;
                mod.D4 = mod.D4 == null ? 0 : mod.D4;
                mod.D5 = mod.D5 == null ? 0 : mod.D5;
                mod.D6 = mod.D6 == null ? 0 : mod.D6;
                mod.D7 = mod.D7 == null ? 0 : mod.D7;
                mod.D8 = mod.D8 == null ? 0 : mod.D8;
                mod.D9 = mod.D9 == null ? 0 : mod.D9;

                #endregion

                //保存功图数据 WELL_GT_DATA
                //       string  maxId2 = Common.GetSeqMaxId("SEQ_WELL_GT_DATA");//获取序列值为ID
                sqlText = string.Format("insert into Data_temp (WELL_ID,WELL_NAME,CMD_TYPE,IP,PORT,D1,D2,D3,D4,D5,D6,D7,D8,D9,STIME) "
                                           + " values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}',TO_DATE('{14}','yyyy-mm-dd hh24:mi:ss'))",
                                               mod.DeviceId, mod.DeviceName, mod.cmdType, mod.DeviceIp, mod.DevicePort, mod.D1, mod.D2, mod.D3, mod.D4, mod.D5, mod.D6, mod.D7, mod.D8, mod.D9, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                int intResult = Common.ExecuteNonQuery(sqlText);
                if (intResult >= 1)
                {
                    ShowSendMessage("插入数据成功");
                }
            }
            catch (Exception ex)
            {
                Common.WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message + "\r\n" + sqlText);//将出错信息写入错误日志
                ShowSendMessage("插入数据失败 " + ex.Message + "\r\n" + sqlText);
            }
        }
    }
}
