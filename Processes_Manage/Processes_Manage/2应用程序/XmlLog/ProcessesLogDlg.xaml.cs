using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Processes_Manage
{
    /// <summary>
    /// ProcessesLogDlg.xaml 的交互逻辑
    /// </summary>
    public partial class ProcessesLogDlg : Window
    {
        public ProcessesLogDlg()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnClosed(EventArgs e)
        {
            ProcessesLogList.ClearLog();
            base.OnClosed(e);
        }

        //单例对话框
        private static ProcessesLogDlg processesLogDlg = null;
        public static void GetInstance()
        {
            if (processesLogDlg == null || processesLogDlg.IsLoaded == false)
            {
                processesLogDlg = new ProcessesLogDlg();
            }

            if (processesLogDlg.WindowState == WindowState.Minimized)
            {
                processesLogDlg.WindowState = WindowState.Normal;
            }
            processesLogDlg.Focus();
            processesLogDlg.dataGridControl.ItemsSource = ProcessesLogList.GetInstance();
            processesLogDlg.Show();
        }
    }
}
