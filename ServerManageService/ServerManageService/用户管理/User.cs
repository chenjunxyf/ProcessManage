using System;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Data.OleDb;

using ServerManageService.UsersManageDataSetTableAdapters;

namespace ServerManageService.UserManage
{
    class User
    {
        #region 公共属性
        //文件存放位置
        private static string _path = System.Configuration.ConfigurationManager.AppSettings["Path"].ToString();
        //超级用户消息处理
        private Action<string> SuperUserAction;
        //普通用户消息处理
        private Action<string> NormalUserAction;

        //数据库 user表  mac<-->name
        private static UsersManageDataSet.UserManageTableDataTable 
            UserManageTable = new UsersManageDataSet.UserManageTableDataTable();
        #endregion //公共属性

        #region 用户信息
        private Socket _clientSocket;                //用户Socket
        private bool _isOnline;                         //标记用户是否在线
        private string _userMac;                       //用户Mac地址 标记特定用户
        private string _userName;                    //用户名
        private int _exceptionNum;                  //用户发生异常数

        private TimeSpan _onlineTime;             //记录用户在线时间

        public Socket ClientSocket
        {
            get { return _clientSocket; }
            set { _clientSocket = value; }
        }
        public string UserMac
        {
            get { return _userMac; }
            set { _userMac = value; }
        }
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        public bool IsOnline
        {
            get { return _isOnline; }
            set { _isOnline = true; }
        }
        public TimeSpan OnlineTime
        {
            get { return _onlineTime; }
            set { _onlineTime = value; }
        }
        public int ExcepitonNum
        {
            get { return _exceptionNum; }
            set { _exceptionNum = value; }
        }
        #endregion //用户信息

        #region         初始化
        //初始化构造函数
        public User(Socket clientSocket,string userMac)
        {
            SetSocket(clientSocket);
            _isOnline = true;
            _userMac = userMac;
            _userName = GetNameByMac(userMac);
            _exceptionNum = 0;
            _onlineTime = new TimeSpan(0,0,0);
        }
        private string GetNameByMac(string mac)
        {
            if (UserManageTable.Rows.Count == 0)
            {
                //适配器
                UserManageTableTableAdapter adapter = new UserManageTableTableAdapter();
                //数据库表 添加内容
                UserManageTable = adapter.GetData();
            }
            var name = (from data in UserManageTable
                       where data.UserMac == mac
                       select data.UserName).FirstOrDefault();
            return name;
        }
        #endregion //初始化

        #region 用户基本操作
        //超级用户特别事件处理
        public void SetSuperAction(Action<string> superUserAciton)
        {
            SuperUserAction = superUserAciton;
        }
        //普通用户消息处理
        public void SetNormalAction(Action<string> normalUserAction)
        {
            NormalUserAction = normalUserAction;
        }
        //Socket更新
        public void SetSocket(Socket clientSocket)
        {
            _clientSocket = clientSocket;
            _isOnline = true;
        }

        //发送消息
        public void Send(string message)
        {
            try
            {
                Byte[] msg = Encoding.UTF8.GetBytes(message);
                _clientSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None,
                    ar => { }, null);
            }
            catch (Exception)
            {
                _isOnline = false;
                _clientSocket.Close();
                return;
            }
        }
        //接收消息
        public void Receive()
        {
            if (_clientSocket.Connected)
            {
                try
                {
                    byte[] buffer = new byte[8];
                    //由于long占8位字节，所以先获取前8位字节数据
                    IAsyncResult iar = _clientSocket.BeginReceive(buffer, 0,
                        buffer.Length, SocketFlags.None, null, null);
                    int len = _clientSocket.EndReceive(iar);
                    int command = BitConverter.ToInt32(buffer, 0);
                    //根据不同的命令参数 进行不同的处理
                    //command一定要大于0:当远程client退出时，就服务来讲，会有很多空字节的发送 此时command=0!!!
                    if (command < 10 && command > 0)
                        ReceiveMessage(command);     //接收消息
                    else if (command > 0)
                        ReceiveFile(command);             //接收文件

                    //警告：这里如果加上Receive()理论上是对的  但对于交互的Windows服务，则会有栈溢出！！！
                    //栈溢出原因：递归调用 a调用b,b又调用a的bug!!!
                    //Receive(); 
                }
                catch (Exception)
                {
                    _isOnline = false;
                    _clientSocket.Close();
                    return;
                }
            }
            else
                _isOnline = false;
        }
        #endregion //用户基本操作

        #region 接收消息处理
        private void ReceiveMessage(int command)
        {
            //异步的接受消息
            try
            {
                byte[] msg = new byte[256];
                _clientSocket.Receive(msg, msg.Length, 0);
                string message = Encoding.UTF8.GetString(msg).Trim(' ', '\0');
                MessageHandle(command, message);
                //再次等待消息
                Receive();
            }
            catch (Exception)
            {
                _isOnline = false;
                _clientSocket.Close();
                return;
            }
        }
        //不同的消息指令不同的处理
        private void MessageHandle(int command,string message)
        {
            //普通用户
            if (command == 1)
            {
                NormalUserAction(message);
            }
            //超级用户
            else
            {
                SuperUserAction(message);
            }
        }
        #endregion //接收消息处理

        #region  接收文件处理
        private void ReceiveFile(long filelen)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                int bytesRead = 0;
                long count = 0;
                byte[] buffer = new byte[8192];

                while (count != filelen)
                {
                    bytesRead = _clientSocket.Receive(buffer, buffer.Length, 0);
                    ms.Write(buffer, 0, bytesRead);
                    count += bytesRead;
                }
                ReceivedBitmap(new Bitmap(ms));
                _exceptionNum++;
                //再次等待消息
                Receive();
            }
            catch (Exception)
            {
                _isOnline = false;
                _clientSocket.Close();
                return;
            }
        }
        private void ReceivedBitmap(Bitmap bitmap)
        {
            Bitmap image = new Bitmap(bitmap, 800, 600);
            //图像名
            string imageName = DateTime.Now.ToString().Replace('/','-').Replace(':','-');
            //图像存储路径
            string path = _path +_userName+_userMac.Replace(':','-');
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            image.Save(path+"\\"+ imageName+ ".jpg", ImageFormat.Jpeg);
        }
        #endregion //接收文件处理
    }
}
