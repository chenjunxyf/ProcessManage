using System;
using System.Windows;
using System.Collections.ObjectModel;


namespace Processes_Manage
{
    /// <summary>
    /// ControlUserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ControlUserWindow : Window
    {
        #region    交互参数
        private ObservableCollection<SensitiveProcess> _sensitiveProcesses;
        public ObservableCollection<SensitiveProcess> SensitiveProcesses
        {
            get { return _sensitiveProcesses; }
            set { _sensitiveProcesses = value; }
        }
        #endregion //交互参数

        public ControlUserWindow()
        {
            InitializeComponent();
        }
        //窗口加载
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SProcess_ListView.ItemsSource = _sensitiveProcesses;
        }
    }
}
