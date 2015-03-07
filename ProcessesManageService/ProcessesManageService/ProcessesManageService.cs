using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

using System.Text;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Windows.Forms;

using ProcessesManageService.CommunicationManage;
using ProcessesManageService.SystemInfoManage;
using ProcessesManageService.SensitiveProcessesManage;

namespace ProcessesManageService
{
    public partial class ProcessesManageService : ServiceBase
    {
        #region  服务重要参数
        //客户端主线程管理
        private Thread MainThread;
        //客户端Socket
        private ClientSocket clientSocket = null;
        //接收信息处理
        private Action<string> ReceiveMessageAction;
        //进程管理链表
        private ProcessesManageList processesManageList = new ProcessesManageList();
        //图片文件存储路径
        private static string ImageBufferPath;
        #endregion  //服务重要参数

        #region  服务管理
        public ProcessesManageService()
        {
            InitializeComponent();
        }

        //启动时 初始化参数
        protected override void OnStart(string[] args)
        {
            //客户端Socket初始化
            clientSocket = new ClientSocket();
            //接收到控制消息处理
            ReceiveMessageAction = msg =>
            {
                ReceivedMessageHandle(msg);
            };
            //异常敏感进程出现处理事件
            VerifySensitiveProcess.SensitiveAction = () =>
            {
                SensitiveActionHandle();
            };
            //图片缓存文件夹位置
            ImageBufferPath = ConfigurationManager.AppSettings["ImageBufferPath"];
            //主线程初始化处理
            MainThread = new Thread(new ThreadStart(MainThread_Handle));
            MainThread.IsBackground = true;
            MainThread.Start();
        }

        //退出服务时发生
        protected override void OnStop()
        {
            processesManageList.Clear();
            MainThread.Abort();
        }
        #endregion //服务管理

        #region 主线程管理
        //计时器的事件处理
        private void MainThread_Handle()
        {
            while (true)
            {
                MainBusinessHandle();
                Thread.Sleep(1000);
            }
        }
        //主线程处理的主要事务
        private void MainBusinessHandle()
        {
            //应用程序管理
            processesManageList.BeginManage();
            //Socket管理
            if (clientSocket.communicateSocket.Connected == false)
            {
                //检测是否能够ping通服务器
                if (clientSocket.IAccessful())
                    clientSocket.Access(new Action(() =>
                    {
                        try
                        {
                            //首先发送自己的Mac地址
                            clientSocket.Send(SystemInfoHelper.GetMacAddress());
                            //开始异步接收服务器发送来的消息
                            clientSocket.Receive(ReceiveMessageAction);
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }));
            }
        }
        #endregion //主线程管理

        #region   消息事件管理
        //获取服务器消息 处理函数
        private void ReceivedMessageHandle(string message)
        {
            string[] msg = message.Split('|');
            switch (msg[0])
            {
                case "AllSProcesses":
                    SendAllSProcesses();
                    break;
                case "KSProcess":
                    KillSProcesses(msg[1]);
                    break;
                case "KOSProcess":
                    SetOrderTime(msg[1]);
                    break;
            }
        }
        //发送所有敏感进程
        private void SendAllSProcesses()
        {
            List<string> temp = ProcessesManageList.GetAllSensitiveProcesses();
            if (temp.Count == 0)        //无敏感进程
            {
                clientSocket.Send("1");
                clientSocket.Send("NoSProcess");
            }
            else
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    clientSocket.Send("1");
                    clientSocket.Send("SProcess|" + temp[i]);
                    Thread.Sleep(300);
                }
            }
        }
        //结束所有敏感进程
        private void KillSProcesses(string message)
        {
            Process[] p = Process.GetProcessesByName(message);
            for (int k = 0; k < p.Length; k++)
                p[k].Kill();
            clientSocket.Send("1");
            clientSocket.Send("KSuccess");
        }
        //重设敏感时间
        private void SetOrderTime(string message)
        {
            string[] megs = message.Split('-');
            ProcessesManageList.SetSProcessOrderTime(megs[0],megs[1]);
            clientSocket.Send("1");
            clientSocket.Send("KSuccess");
        }
        //异常敏感事件处理函数
        private void SensitiveActionHandle()
        {
            //抓屏 图片名
            string fileName = DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + ".jpg";
            //保证缓存目录存在
            if (!Directory.Exists(ImageBufferPath))
                Directory.CreateDirectory(ImageBufferPath);
            //图片保存位置
            string filePath = ImageBufferPath + fileName;
            //抓屏 
            CaptureScreen.CaptureScreenNow(filePath);
            //如果连接着服务器 则发送给服务器(一定要异步)
            MethodInvoker methodInvoke = new MethodInvoker(() =>
            {
                if (clientSocket.communicateSocket.Connected)
                {
                    clientSocket.SendFile(filePath);
                }
            });
            methodInvoke.BeginInvoke(null, null);
        }
        #endregion  //消息事件管理
    }
}
