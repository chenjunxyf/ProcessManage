using System;
using System.ServiceProcess;
using ServerManageService.CommunicationManage;

namespace ServerManageService
{
    public partial class ServerManageService : ServiceBase
    {
        ServerSocket serverSocket = null;
        public ServerManageService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            serverSocket = new ServerSocket();
            serverSocket.Access();
        }

        protected override void OnStop()
        {
            serverSocket.Close();
        }
    }
}
