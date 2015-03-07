using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.IO;
using System.Configuration;
using System.Windows.Navigation;
using System.Threading;

namespace Processes_Manage
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
        }
        private string xmlFilename = "SoftWareInformation.xml";
        private Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            bool isFirstAppInstance;
            mutex = new Mutex(true, "Processes_Manage", out isFirstAppInstance); //单实例启动
            if (!isFirstAppInstance)
            {
                MessageBox.Show("系统中已经运行了程序的其它实例！", "注意");
                Environment.Exit(0);
            }

            InitializeDataGrid();

            SplashScreen ss = new SplashScreen("Images/start.png");
            ss.Show(true);

            base.OnStartup(e);
        }
        //软件使用日志 参数初始化
        private void InitializeDataGrid()
        {
            //注册序列号
            Xceed.Wpf.DataGrid.Licenser.LicenseKey = "DGP30-E852N-G9C6E-DW5A";
            Application.Current.Properties["SavedDetailsFileName"] = xmlFilename;    //保存文件名
            Application.Current.Properties["SaveFolder"] =                                          //将要保存到的文件夹
                Environment.CurrentDirectory;
            Application.Current.Properties["FullXmlPath"] =                                        //完整的XML路径
                Path.Combine(Environment.CurrentDirectory, xmlFilename);
        }
    }
}
