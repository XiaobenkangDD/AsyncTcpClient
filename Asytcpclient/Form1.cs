using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Asytcpclient
{
    public partial class Form1 : Form
    {
        TcpClient tcp = null;
        NetworkStream workStream = null;

        private enum DataMode { Text, Hex }
        private char[] HexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f' };

        public Form1()
        {
            InitializeComponent();
            Test();
        }


        public void Test()
        {
            List<string> list = new List<string>();
            list.Add("172.20.1.118");
            for(int i=0;i<list.Count;i++)
            {
                string ip = list[i];

            }
        }

        
        private bool CharInArray(char aChar, char[] charArray)
        {
            return (Array.Exists<char>(charArray, delegate (char a) { return a == aChar; }));
        }

        /// <summary>

        /// 十六进制字符串转换字节数组

        /// </summary>

        /// <param name="s"></param>

        /// <returns></returns>

        private byte[] HexStringToByteArray(string s)
        {
            // s = s.Replace(" ", "");

            StringBuilder sb = new StringBuilder(s.Length);
            foreach (char aChar in s)
            {
                if (CharInArray(aChar, HexDigits))
                    sb.Append(aChar);
            }
            s = sb.ToString();
            int bufferlength;
            if ((s.Length % 2) == 1)
                bufferlength = s.Length / 2 + 1;
            else bufferlength = s.Length / 2;
            byte[] buffer = new byte[bufferlength];
            for (int i = 0; i < bufferlength - 1; i++)
                buffer[i] = (byte)Convert.ToByte(s.Substring(2 * i, 2), 16);
            if (bufferlength > 0)
                buffer[bufferlength - 1] = (byte)Convert.ToByte(s.Substring(2 * (bufferlength - 1), (s.Length % 2 == 1 ? 1 : 2)), 16);
            return buffer;
        }

        /// <summary>

        /// 字节数组转换十六进制字符串

        /// </summary>

        /// <param name="data"></param>

        /// <returns></returns>

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }

        /// <summary>

        /// 当前接收模式

        /// </summary>

        private DataMode CurrentReceiveDataMode
        {
            get
            {
                return (chkHexReceive.Checked) ? DataMode.Hex : DataMode.Text;
            }
            set
            {
                chkHexReceive.Checked = (value == DataMode.Text);
            }
        }

        /// <summary>

        /// 当前发送模式

        /// </summary>

        private DataMode CurrentSendDataMode
        {
            get
            {
                return (chkHexSend.Checked) ? DataMode.Hex : DataMode.Text;
            }
            set
            {
                chkHexSend.Checked = (value == DataMode.Text);
            }
        }

        /// <summary>

        /// 发送数据

        /// </summary>

        private void SendData()
        {
            if (workStream != null)
            {
                byte[] data;
                if (CurrentSendDataMode == DataMode.Text)
                {
                    data = Encoding.ASCII.GetBytes(rtfSend.Text);
                }
                else
                {
                    // 转换用户十六进制数据到字节数组

                    data = HexStringToByteArray(rtfSend.Text);
                }
                workStream.Write(data, 0, data.Length);
            }
        }

        delegate void SetTextCallback(string text);
        delegate void SetControl();
        delegate void GetData(byte[] data);
          

        /// <summary>

        /// 异步接收数据  

        /// </summary>

        /// <param name="data"></param>

        private void OnGetData(byte[] data)
        {
            string sdata;
            if (CurrentReceiveDataMode == DataMode.Text)
            {
                sdata = new string(Encoding.UTF8.GetChars(data));
            }
            else
            {
                sdata = ByteArrayToHexString(data);
            }

            rtfReceive.Invoke(new EventHandler(delegate
            {
                rtfReceive.AppendText(sdata);
            }));
        }

        /// <summary>

        /// 异步设置Log

        /// </summary>

        /// <param name="text"></param>

        private void SetText(string text)
        {
            if (rtfLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                rtfLog.AppendText(text + Environment.NewLine);
            }
        }

        public ManualResetEvent connectDone = new ManualResetEvent(false);

        /// <summary>

        /// 异步连接的回调函数

        /// </summary>

        /// <param name="ar"></param>

        private void ConnectCallback(IAsyncResult ar)
        {
            connectDone.Set();
            TcpClient t = (TcpClient)ar.AsyncState;
            try
            {
                if (t.Connected)
                {
                    SetText("连接成功");
                    t.EndConnect(ar);
                    SetText("连接线程完成");
                }
                else
                {
                    SetText("连接失败");
                    t.EndConnect(ar);
                }

            }
            catch (SocketException se)
            {
                SetText("连接发生错误ConnCallBack.......:" + se.Message);
            }
        }

        /// <summary>

        /// 异步连接

        /// </summary>

        private void Connect()
        {
            if ((tcp == null) || (!tcp.Connected))
            {
                try
                {
                    tcp = new TcpClient();
                    tcp.ReceiveTimeout = 10;


                    connectDone.Reset();

                    SetText("Establishing Connection to " + txtIP.Text);

                    tcp.BeginConnect(txtIP.Text, (int)numPort.Value,
                        new AsyncCallback(ConnectCallback), tcp);

                    connectDone.WaitOne();

                    if ((tcp != null) && (tcp.Connected))
                    {
                        workStream = tcp.GetStream();

                        SetText("Connection established");

                        asyncread(tcp);
                    }
                }
                catch (Exception se)
                {
                    rtfLog.AppendText(se.Message + " Conn......." + Environment.NewLine);
                }
            }
        }

        /// <summary>

        /// 断开连接

        /// </summary>

        private void DisConnect()
        {
            if ((tcp != null) && (tcp.Connected))
            {
                workStream.Close();
                tcp.Close();
            }
        }

        /// <summary>

        /// 设置控件状态

        /// </summary>

        private void setBtnStatus()
        {
            if ((btnConnect.InvokeRequired) || (btnSend.InvokeRequired))
            {
                this.Invoke(new SetControl(setBtnStatus));
            }
            else
            {
                int con = ((tcp == null) || (!tcp.Connected)) ? 0 : 1;
                string[] constr = { "连接", "断开" };
                bool[] btnEnabled = { false, true };

                btnConnect.Text = constr[con];
                btnSend.Enabled = btnEnabled[con];
            }
        }

        /// <summary>

        /// 连接按钮

        /// </summary>

        /// <param name="sender"></param>

        /// <param name="e"></param>

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if ((tcp != null) && (tcp.Connected))
            {
                DisConnect();
            }
            else
            {
                Connect();
            }
            setBtnStatus();
        }

        /// <summary>

        /// 发送数据按钮

        /// </summary>

        /// <param name="sender"></param>

        /// <param name="e"></param>

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                SendData();
            }
            catch (Exception se)
            {
                rtfLog.AppendText(se.Message + Environment.NewLine);
            }
        }

        /// <summary>

        /// 异步读TCP数据

        /// </summary>

        /// <param name="sock"></param>

        private void asyncread(TcpClient sock)
        {
            StateObject state = new StateObject();
            state.client = sock;
            NetworkStream stream = sock.GetStream();
       //`     IPEndPoint ip = (IPEndPoint)state.client.Client.RemoteEndPoint;
            if (stream.CanRead)
            {
                try
                {
                    IAsyncResult ar = stream.BeginRead(state.buffer, 0, StateObject.BufferSize,
                            new AsyncCallback(TCPReadCallBack), state);
                }
                catch (Exception e)
                {
                    SetText("Network IO problem " + e.ToString());
                }
            }
        }

        /// <summary>

        /// TCP读数据的回调函数

        /// </summary>

        /// <param name="ar"></param>

        private void TCPReadCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            //主动断开时

            if ((state.client == null) || (!state.client.Connected))
                return;
            int numberOfBytesRead;
            NetworkStream mas = state.client.GetStream();
            string type = null;

            numberOfBytesRead = mas.EndRead(ar);
            state.totalBytesRead += numberOfBytesRead;

            SetText("Bytes read ------ " + numberOfBytesRead.ToString());
            if (numberOfBytesRead > 0)
            {
                byte[] dd = new byte[numberOfBytesRead];
                Array.Copy(state.buffer, 0, dd, 0, numberOfBytesRead);
                OnGetData(dd);
                mas.BeginRead(state.buffer, 0, StateObject.BufferSize,
                        new AsyncCallback(TCPReadCallBack), state);
            }
            else
            {
                //被动断开时 

                mas.Close();
                state.client.Close();
                SetText("Bytes read ------ " + numberOfBytesRead.ToString());
                SetText("不读了");
                mas = null;
                state = null;

                setBtnStatus();
            }
        }

    }

    internal class StateObject
    {
        public TcpClient client = null;
        public int totalBytesRead = 0;
        public const int BufferSize = 1024;
        public string readType = null;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder messageBuffer = new StringBuilder();
    }

    }
