using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Processes_Manage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //属性
        private Page[] pages;
        private int pageIndex = 0;
        private Thread mainThread;
        private bool isWindowClosed = false;   //标识 主窗口是否真的关闭
        #region     Windows管理
        //构造函数
        public MainWindow()
        {
            InitializeComponent();
        }
        //加载时的初始化
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pages = new Page[6];
            pages[0] = new FirstPage();                            //首页
            pages[1] = new SoftWareAppPage();              //应用程序
            pages[2] = new OnLineUsersPage();               //在线用户控制管理
            pages[3] = new SystemInfoPage();                 //系统信息
            pages[4] = new ProcessesViewPage();            //系统进程

            mainThread = new Thread(new ThreadStart(mainThreadStart));
            mainThread.IsBackground = true;
            mainThread.Start();
            FirstPageRadioButton.IsChecked = true;
            HotKeyFactory.ReadKeyHistory();                    //历史热键读取
        }
        //关闭后发生
        protected override void OnClosed(EventArgs e)
        {
            mainThread.Abort();
            SoftWareAppPage.SaveProcessesLog();
            base.OnClosed(e);
        }
        //关闭时发生
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("确定真的关闭吗？", "提醒", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                == MessageBoxResult.Yes)
            {
                isWindowClosed = true;
                this.Close();
            }
            else if (!isWindowClosed)
            {
                e.Cancel = true;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
                {
                    this.Hide();
                    return null;
                }, null);
            }
            base.OnClosing(e);
        }
        #endregion //Windows管理

        #region 系统变化信息更新管理
        private void mainThreadStart()
        {
            while (true)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpDateSystemInfo();
                }), null);
                Thread.Sleep(3000);
            }
        }
        private void UpDateSystemInfo()
        {
            //进程个数
            processesNums_Now.Text = SystemInfoHelper.GetProcessesNum().ToString();
            //cpu占用率
            cpu_Rate.Text = SystemInfoHelper.SysCpuUsage + "%";
            //内存占用率
            memory_Rate.Text = SystemInfoHelper.SysMemoryUsage + "%";
            //Gpu温度
            gpu_temperature.Text = SystemInfoHelper.GetGPUTemperature().ToString() + "°C";
        }
        #endregion //系统变化信息更新管理

        #region 界面特效控制区
        private void zoomOutStoryboardCompleted(object sender, EventArgs args)
        {
            mainFrame.Navigate(pages[pageIndex]);
        }
        private void frameContentRendered(object sender, EventArgs args)
        {
            Storyboard s = (Storyboard)this.Resources["ZoomInStoryboard"];
            s.Begin(this);
        }
        private void zoomInStoryboardCompleted(object sender, EventArgs e)
        {
            scrollViewerBorder.Visibility = Visibility.Visible;
        }

        //页面选择事件
        private void pageSelected(object sender, RoutedEventArgs args)
        {
            Point3DCollection points = new Point3DCollection();

            double ratio = myScrollViewer.ActualWidth / myScrollViewer.ActualHeight;

            points.Add(new Point3D(5, -5 * ratio, 0));
            points.Add(new Point3D(5, 5 * ratio, 0));
            points.Add(new Point3D(-5, 5 * ratio, 0));

            points.Add(new Point3D(-5, 5 * ratio, 0));
            points.Add(new Point3D(-5, -5 * ratio, 0));
            points.Add(new Point3D(5, -5 * ratio, 0));

            points.Add(new Point3D(-5, 5 * ratio, 0));
            points.Add(new Point3D(-5, -5 * ratio, 0));
            points.Add(new Point3D(5, -5 * ratio, 0));

            points.Add(new Point3D(5, -5 * ratio, 0));
            points.Add(new Point3D(5, 5 * ratio, 0));
            points.Add(new Point3D(-5, 5 * ratio, 0));

            myGeometry.Positions = points;
            myViewport3D.Width = 100;
            myViewport3D.Height = 100 * ratio;

            scrollViewerBorder.Visibility = Visibility.Hidden;

            RadioButton radioButton = sender as RadioButton;
            PageSelect(radioButton);
        }
        //根据按钮内容选择相应页面
        private void PageSelect(RadioButton radioButton)
        {
            if (radioButton != null)
            {
                string radioButtonContent = radioButton.Content.ToString();
                switch (radioButtonContent)
                {
                    case "-首 页-":
                        pageIndex = 0;
                        break;
                    case "-应用程序管理-":
                        pageIndex = 1;
                        break;
                    case "-在线用户管理-":
                        pageIndex = 2;
                        break;
                    case "-系统信息查看-":
                        pageIndex = 3;
                        break;
                    case "-系统进程查看-":
                        pageIndex = 4;
                        break;
                }
            }
        }
        #endregion //界面特效控制区

        #region Menu
        //热键设置
        private void set_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetWindow setWindow = new SetWindow();
            setWindow.Owner = this;
            setWindow.ShowDialog();
        }
        //新建任务
        private void newTask_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "rundll32.exe";
            proc.StartInfo.Arguments = "shell32.dll #61";
            proc.Start();
        }
        //退出
        private void quit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        //关于对话框
        [DllImport("shell32.dll", EntryPoint = "ShellAbout")]
        public static extern int ShellAbout(
                                                int hwnd,
                                                string szApp,
                                                string szOtherStuff,
                                                int hIcon
                                            );
        //关于对话框
        private void about_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShellAbout(0, "<Processes_Manage>适应平台信息", "By 陈骏 B09040823@njupt.edu.cn", 0);
        }
        //是否置顶
        private void topmost_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
        }
        #endregion
    }
}
