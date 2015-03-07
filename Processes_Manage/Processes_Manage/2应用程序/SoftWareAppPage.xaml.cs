using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;

namespace Processes_Manage
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class SoftWareAppPage : Page
    {
        private static ObservableCollection<TopWinProcessInfo> SoftWareList
                          = new ObservableCollection<TopWinProcessInfo>();                          //应用程序日志链表

        public SoftWareAppPage()
        {
            InitializeComponent();
            StartToRecordLog();
        }

        #region           信息获取
        //计时器初始化
        private void StartToRecordLog()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            getTopSoftWare();
        }
        //获取顶层窗口 函数处理
        [DllImport("user32")]
        private static extern IntPtr GetForegroundWindow();
        void getTopSoftWare()
        {
            IntPtr ptrWnd = GetForegroundWindow();
            Process[] processes = Process.GetProcesses();
            //使用linq查询
            var myProcess = (from p in processes
                             where p.MainWindowHandle == ptrWnd && p.MainWindowTitle != ""
                             select p).FirstOrDefault();
            if (myProcess != null && myProcess.ProcessName != "Processes_Manage")
            {
                int length = SoftWareList.Count;
                //如果进程名和窗口名与上一个相同
                if (length != 0 && ((TopWinProcessInfo)SoftWareList[length - 1]).ProcessName == myProcess.ProcessName.ToString() + ".exe")
                {
                    ((TopWinProcessInfo)SoftWareList[length - 1]).UsingTime += new TimeSpan(0, 0, 1);
                }
                else
                {
                    //添加新的程序使用信息
                    SoftWareList.Add(new TopWinProcessInfo(myProcess.ProcessName + ".exe",
                        myProcess.MainWindowTitle.ToString(), myProcess.StartTime.ToString()));
                }
            }
            appListView.ItemsSource = SoftWareList;
        }
        #endregion    //信息获取

        //Log信息保存
        public static void SaveProcessesLog()
        {
            XMLFileOperations.AppendToFile(SoftWareList);
            SoftWareList.Clear();
        }

        //清空
        private void clearLog_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定清空并保存吗？", "确定", MessageBoxButton.OKCancel, MessageBoxImage.Warning)
                == MessageBoxResult.OK)
            {
                SaveProcessesLog();
            }
        }

        //查看历史
        private void checkHistory_Button_Click(object sender, RoutedEventArgs e)
        {
            ProcessesLogDlg.GetInstance();
        }
    }
}
