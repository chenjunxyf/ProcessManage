using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Threading;

namespace Processes_Manage
{
    /// <summary>
    /// Page3.xaml 的交互逻辑
    /// </summary>
    public partial class FirstPage : Page
    {
        public FirstPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Thread firstPageThread = new Thread(new ThreadStart(FirstPageThreadStart));
            firstPageThread.IsBackground = true;
            firstPageThread.Start();
        }
        //首页信息 3秒变化一次
        private void FirstPageThreadStart()
        {
            while (true)
            {
                //UpDateSystemInfo();
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //系统时间跟新
                    systemTimeNow_TextBlock.Text = "当前系统时间：" + DateTime.Now.ToString();
                    int tickCount = Environment.TickCount;
                    systemUsingTime_TextBlock.Text = "电脑已经开机：" + Convert.ToString(tickCount / 3600000) + "时:"
                                         + Convert.ToString((tickCount % 3600000) / 60000) + "分:"
                                               + Convert.ToString((tickCount % 60000) / 1000) + "秒";
                    //网络状况
                    SystemInfoHelper.UpdateNetInfo();
                    netWorkState_TextBlock.Text = SystemInfoHelper.IsOnline == true ? "连接网络" : "未连接网络";
                    IpAddress_TextBlock.Text = SystemInfoHelper.IPAddress;
                    IpSubnet_TextBlock.Text = SystemInfoHelper.IPSubnet;
                    //cpu
                    SystemInfoHelper.UpdateCpuUsage();
                    cpuRate_TextBlock.Text = "系统cpu使用率" + SystemInfoHelper.SysCpuUsage + "%";
                    cpu_ProgressBar.Value = double.Parse(SystemInfoHelper.SysCpuUsage);
                    //内存
                    SystemInfoHelper.UpdateMemoryUsage();
                    memoryRate_TextBlock.Text = "系统内存使用率" + SystemInfoHelper.SysMemoryUsage + "%";
                    memory_ProgressBar.Value = double.Parse(SystemInfoHelper.SysMemoryUsage);
                }), null);
                Thread.Sleep(3000);
            }
        }
        //系统cpu 内存过大时变色
        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((sender as ProgressBar).Value > 50)
                (sender as ProgressBar).Foreground = Brushes.Red;
            else
                (sender as ProgressBar).Foreground = Brushes.Green;
        }

        //cpu变化曲线
        SystemCpuUsageLineDlg SCULD = null;
        private void cpuUsageLine_Button_Click(object sender, RoutedEventArgs e)
        {
            if (SCULD == null || SCULD.IsLoaded == false)
                SCULD = new SystemCpuUsageLineDlg();
            SCULD.Focus();
            SCULD.WindowState = WindowState.Maximized;
            SCULD.Show();
        }
        //内存变化曲线
        SystemMemoryUsageLineDlg SMULD;
        private void memoryUsageLine_Button_Click(object sender, RoutedEventArgs e)
        {
            if (SMULD == null || SMULD.IsLoaded == false)
                SMULD = new SystemMemoryUsageLineDlg();
            SMULD.Focus();
            SMULD.WindowState = WindowState.Maximized;
            SMULD.Show();
        }

    }
}
