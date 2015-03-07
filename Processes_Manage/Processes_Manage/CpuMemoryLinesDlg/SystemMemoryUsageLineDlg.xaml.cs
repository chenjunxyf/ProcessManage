using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Input;
using System.Threading;

namespace Processes_Manage
{
    /// <summary>
    /// SystemMemoryUsageLineDlg.xaml 的交互逻辑
    /// </summary>
    public partial class SystemMemoryUsageLineDlg : Window
    {
        private ObservableDataSource<Point> dataSource = new ObservableDataSource<Point>();
        private Thread thread;
        private int i = 0;

        public SystemMemoryUsageLineDlg()
        {
            InitializeComponent();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Escape))
                this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            plotter.AddLineGraph(dataSource, Colors.Green, 2, "Percentage");
            thread = new Thread(new ThreadStart(Refresh));
            thread.IsBackground = true;
            thread.Start();
            plotter.Viewport.FitToView();
        }
        private void Refresh()
        {
            while (true)
            {
                this.Dispatcher.Invoke(new Action(() => { AnimatedPlot(); }), null);
                Thread.Sleep(2000);
            }
        }
        private void AnimatedPlot()
        {
            memoryUsageText.Text = SystemInfoHelper.SysMemoryUsage+"%"; //内存 使用率
            double x = i;
            double y = double.Parse(SystemInfoHelper.SysMemoryUsage);
            Point point = new Point(x, y);
            dataSource.AppendAsync(base.Dispatcher, point);
            i++;
        }
        protected override void OnClosed(EventArgs e)
        {
            thread.Abort();
            base.OnClosed(e);
        }

    }
}
