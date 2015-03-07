using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Configuration;

namespace Processes_Manage.CommunicationManage
{
    class ClientSocket:SocketFunc
    {
        //服务器端点
        private IPEndPoint _serverIp;
        //构造函数
        public ClientSocket()
        {
            //初始化Socket
            InitializeSocket();
            //配置文档读取ip和端口
            base.Ip = ConfigurationManager.AppSettings["IpAddress"];          
            base.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            //要连接的服务器 IP和端口
            _serverIp = new IPEndPoint(IPAddress.Parse(base.Ip), base.Port);
        }

        //初始化Socket 以便反复利用此变量
        private void InitializeSocket()
        {
            base.communicateSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        //连接服务器操作
        public void Access(Action AccessAction)
        {
            try
            {
                //客户端只用来向指定的服务器发送、接收信息  不需要绑定本机的IP和端口,不需要监听
                /*base.communicateSocket.BeginConnect(_serverIp, ar =>
                {
                    if (communicateSocket.Connected == true)
                    {
                        MessageBox.Show("连接服务器成功");
                        AccessAction();
                    }
                }, null);*/
                base.communicateSocket.Connect(IPAddress.Parse(base.Ip),base.Port);
                if (base.communicateSocket.Connected)
                {
                    MessageBox.Show("连接服务器成功");
                    AccessAction();
                }
            }
            catch(Exception)
            {
                //连接失败
                MessageBox.Show("对不起!连接失败,请重试!");
                //释放资源 重新配置Socket 以便下一次连接
                base.communicateSocket.Close();
                //Socket重置
                InitializeSocket();
                return;
            }
        }
    }
}
