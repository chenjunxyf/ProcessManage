using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

using System.Configuration;
using System.Net.NetworkInformation;

namespace ProcessesManageService.CommunicationManage
{
    class ClientSocket:SocketFunc
    {
        //构造函数 初始化Socket
        public ClientSocket()
        {
            //初始化Socket
            InitializeSocket();
            //配置文档读取ip和端口
            base.Ip = ConfigurationManager.AppSettings["IpAddress"];          
            base.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
        }
        //初始化Socket 以便反复利用此变量
        private void InitializeSocket()
        {
            base.communicateSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        //是否能ping通 服务器
        public bool IAccessful()
        {
            int timeout = 100;
            string data = "Test Data!";
            Ping p = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            PingReply reply = p.Send(base.Ip, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }
        //连接服务器操作
        public override void Access(Action AccessAciton)
        {
            try
            {
                //要连接的服务器 IP和端口
                IPEndPoint serverIP = new IPEndPoint(IPAddress.Parse(base.Ip), base.Port);
                //客户端只用来向指定的服务器发送、接收信息  不需要绑定本机的IP和端口,不需要监听
                base.communicateSocket.BeginConnect(serverIP, ar =>
                {
                    if(communicateSocket.Connected==true)
                         AccessAciton();
                }, null);
            }
            catch(Exception)
            {
                //释放资源 重新配置Socket 以便下一次连接
                base.communicateSocket.Close();
                InitializeSocket();
                return;
            }
        }
        //文件发送
        public void SendFile(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            //文件大小
            byte[] len = BitConverter.GetBytes(fi.Length);
            try
            {
                //首先把文件长度发送过去 然后再发送文件流
                base.communicateSocket.BeginSendFile(filename,
                    len,
                    null,
                    TransmitFileOptions.UseDefaultWorkerThread,
                    new AsyncCallback(SendFileCallback),
                    null);
            }
            catch (Exception)
            {
                base.communicateSocket.Close();
                return;
            }
        }
        private void SendFileCallback(IAsyncResult iar)
        {
            try
            {
                base.communicateSocket.EndSendFile(iar);
            }
            catch (Exception)
            {
                base.communicateSocket.Close();
                return;
            }
        }
    }
}
