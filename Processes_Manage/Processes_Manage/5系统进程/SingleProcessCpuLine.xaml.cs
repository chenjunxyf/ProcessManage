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
    /// SingleProcessCpuLine.xaml 的交互逻辑
    /// </summary>
    public partial class SingleProcessCpuLine : Window
    {
        private ObservableDataSource<Point> dataSource = new ObservableDataSource<Point>();
        private DispatcherTimer timer = new DispatcherTimer();
        private PerformanceCounter cpuPerformanceCounter;
        private int i = 0;

        public SingleProcessCpuLine()
        {
            InitializeComponent();        
            plotter.AddLineGraph(dataSource, Colors.Green, 2, "CPU");
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(AnimatedPlot);
            timer.IsEnabled = true;
            cpuPerformanceCounter = new PerformanceCounter("Process", "% Processor Time");
            plotter.Viewport.FitToView();
        }
        private void AnimatedPlot(object sender, EventArgs e)
        {
            Process[] myProcess = Process.GetProcessesByName(processName.Text.ToString());
            if (myProcess.Length < 1)
            {
                timer.Stop();
                this.Close();
            }
            else
            {
                cpuPerformanceCounter.InstanceName = processName.Text.ToString();
                double x = i;
                double y = cpuPerformanceCounter.NextValue();
                if (y > 100)
                    y = 100;
                Point point = new Point(x, y);
                dataSource.AppendAsync(base.Dispatcher, point);

                cpuText.Text = string.Format("{0:0}%",y);
                i++;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            cpuPerformanceCounter.Dispose();
            timer = null;
            base.OnClosed(e);
        }
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Escape))
                this.Close();
        }
    }
}
