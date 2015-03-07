using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Input;

namespace Processes_Manage
{
    /// <summary>
    /// MemoryLine.xaml 的交互逻辑
    /// </summary>
    public partial class SingleProcessMemoryLine : Window
    {
        private ObservableDataSource<Point> dataSource = new ObservableDataSource<Point>();
        private DispatcherTimer timer = new DispatcherTimer();
        int i = 0;
        public SingleProcessMemoryLine()
        {
            InitializeComponent();
        }
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            plotter.AddLineGraph(dataSource, Colors.Green, 2, "Memory");
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(AnimatedPlot);
            timer.IsEnabled = true;
            plotter.Viewport.FitToView();
        }
        private void AnimatedPlot(object sender, EventArgs e)
        {
            Process[] myProcess = Process.GetProcessesByName(processName.Text.ToString());
            if (myProcess.Length<1)
            {
                timer.IsEnabled = false;
                timer.Stop();
                this.Close();
            }
            else
            {
                double x = i;
                double y = myProcess[0].PrivateMemorySize64 / 1024;

                Point point = new Point(x, y);
                dataSource.AppendAsync(base.Dispatcher, point);

                memoryText.Text = string.Format("{0} kb", y);
                i++;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            timer = null;
            base.OnClosed(e);
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Escape))
                this.Close();
        }
            
    }
}
