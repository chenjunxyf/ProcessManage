using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

using System.Configuration;
using System.Threading;
using System.Collections.Generic;
using ServerManageService.UserManage;

namespace ServerManageService.CommunicationManage
{
    class ServerSocket
    {
        #region     属性
        private string Ip = "127.0.0.1";                                               //Ip地址
        private int Port = 9050;                                                         //端口
        private List<User> Users;                                                      //用户管理
        private Action<string> SuperUserAction;                              //处理超级用户消息
        private Action<string> NormalUserAction;                            //处理普通用户消息
        #endregion //属性

        //服务器初始化
        public ServerSocket()
        {
            //配置文档读取ip和端口
            Ip = ConfigurationManager.AppSettings["IpAddress"];
            Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            //所有用户
            Users = new List<User>();
            //超级用户 特殊消息处理
            SuperUserAction = msg =>
            {
                SuperUserMessageHandle(msg);
            };
            //普通用户消息处理
            NormalUserAction = msg =>
            {
                NormalUserMessageHandle(msg);
            };
        }

        #region  服务器管理
        //服务器开放端口 让客户连接
        private static ManualResetEvent allDone = new ManualResetEvent(false);              //设置状态
        public void Access()
        {
            Socket serverSoket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverIp = new IPEndPoint(IPAddress.Parse(Ip), Port);
            //绑定特定的IP和端口
            serverSoket.Bind(serverIp);
            serverSoket.Listen(50);
            Thread thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    allDone.Reset();
                    serverSoket.BeginAccept(new AsyncCallback(AcceptCallback), serverSoket);
                    allDone.WaitOne();
                }
            }));
            thread.IsBackground = true;
            thread.Start();
        }
        //客户连接到服务器后的反馈
        private void AcceptCallback(IAsyncResult ar)   //接收到后处理 函数
        {
            allDone.Set();
            Socket server = (Socket)ar.AsyncState;
            Socket client = server.EndAccept(ar);          //连接到的客户Socket
            //连接后接收的第一条消息:客户的mac地址 或 超级用户的"SuperUser"指令
            byte[] msg = new byte[100];
            client.Receive(msg);
            //此消息一定要过滤多余的空字节
            string mac = Encoding.UTF8.GetString(msg).Trim(' ', '\0');
            //根据消息添加新用户
            AddUser(client, mac);
        }
        //服务器退出 释放资源
        public void Close()
        {
            Users.Clear();
        }
        #endregion //服务器管理

        #region  用户管理
        //添加用户
        private void AddUser(Socket client, string mac)
        {
            if (mac == "")
            {
                try
                {
                    client.Close();
                }
                catch
                { }
                return;
            }
            User user = SearchUserByMac(mac);
            //不存在此用户 则新建；存在则更新一下信息
            if (user == null)
            {
                User newUser = new User(client, mac);
                if (mac == "SuperUser")
                    newUser.SetSuperAction(SuperUserAction);
                else
                    newUser.SetNormalAction(NormalUserAction);
                Users.Add(newUser);   //由于异步监听 所以添加成员时 应该在监听前
                newUser.Receive();      //开始监听消息
            }
            else
            {
                user.SetSocket(client);
                user.IsOnline = true;
                user.Receive();             //重新开始监听消息
            }
        }
        //查询用户
        private User SearchUserByMac(string mac)
        {
            int i = 0;
            for (i = 0; i < Users.Count; i++)
                if (Users[i].UserMac == mac)
                {
                    return Users[i];
                }
            return null;
        }
        #endregion //用户管理

        #region     超级用户消息处理
        private void SuperUserMessageHandle(string message)
        {
            string[] megs = message.Split('|');
            switch (megs[0])
            {
                case "OnlineUsers":
                    SendAllUsers();
                    break;
                case "SProcesses":
                    SendSelectedUser(megs[1]);
                    break;
                case "KSProcess":
                    SendKillProcesses(megs[1], megs[2]);
                    break;
                case "KOSProcess":
                    SendOProcesses(megs[1],megs[2]);
                    break;
            }
        }
        //发送 所有在线用户信息
        private void SendAllUsers()
        {
            User superUser = SearchUserByMac("SuperUser");
            if (superUser.IsOnline)
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    if (Users[i].UserMac != "SuperUser")
                    {
                        string userState = "离线";
                        if (Users[i].IsOnline)
                            userState = "在线";
                        //消息格式：mac+name+是否在线+exceptionNum
                        string message = Users[i].UserMac + "|" +
                            Users[i].UserName + "|" + userState + "|" + Users[i].ExcepitonNum.ToString();
                        superUser.Send("OnlineUser|" + message);
                        Thread.Sleep(450);
                    }
                }
                superUser.Send("RefreshOk");
            }
        }
        //发送给指定被监控用户指令
        private void SendSelectedUser(string message)
        {
            User user = SearchUserByMac(message);
            if (user.IsOnline)
                user.Send("AllSProcesses");
            else
            {
                User superUser = SearchUserByMac("SuperUser");
                superUser.Send("UserOffLine");
            }
        }
        //发送结束进程指令
        private void SendKillProcesses(string message1, string message2)
        {
            User user = SearchUserByMac(message1);
            if (user.IsOnline)
                user.Send("KSProcess|" + message2);
            else
            {
                User superUser = SearchUserByMac("SuperUser");
                superUser.Send("UserOffLine");
            }
        }
        //发送指定进程结束时间命令
        private void SendOProcesses(string message1,string message2)
        {
            User user = SearchUserByMac(message1);
            if (user.IsOnline)
                user.Send("KOSProcess|" + message2);
            else
            {
                User superUser = SearchUserByMac("SuperUser");
                superUser.Send("UserOffLine");
            }
        }
        #endregion  //超级用户消息处理

        #region         普通用户消息处理
        private void NormalUserMessageHandle(string message)
        {
            string[] megs = message.Split('|');
            switch (megs[0])
            {
                case "NoSProcess":
                    SendNoSProcess();
                    break;
                case "SProcess":
                    SendSProcess(megs[1]);
                    break;
                case "KSuccess":
                    SendKSuccess();
                    break;
            }
        }
        //发送无
        private void SendNoSProcess()
        {
            User superUser = SearchUserByMac("SuperUser");
            if (superUser.IsOnline)
                superUser.Send("NoSProcess");
        }
        //发送指定用户的敏感进程给superUser
        private void SendSProcess(string message)
        {
            User superUser = SearchUserByMac("SuperUser");
            if (superUser.IsOnline)
                superUser.Send("SProcess|" + message);
        }
        //发送给super 结束进程成功
        private void SendKSuccess()
        {
            User superUser = SearchUserByMac("SuperUser");
            if (superUser.IsOnline)
                superUser.Send("KSuccess");
        }
        #endregion   //普通用户处理
    }
}
