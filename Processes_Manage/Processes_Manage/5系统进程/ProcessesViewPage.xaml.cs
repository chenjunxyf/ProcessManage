using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Processes_Manage
{
    /// <summary>
    /// Page4.xaml 的交互逻辑
    /// </summary>
    public partial class ProcessesViewPage : Page
    {
        public ProcessesViewPage()
        {
            InitializeComponent();
            DispatcherTimer ProcessesViewPageTimer = new DispatcherTimer(DispatcherPriority.Background);
            ProcessesViewPageTimer.Tick+=new EventHandler(ProcessesViewPageTimer_Tick);
            ProcessesViewPageTimer.Interval = TimeSpan.FromSeconds(3);
            ProcessesViewPageTimer.Start();
        }
        private void ProcessesViewPageTimer_Tick(object sender, EventArgs e)
        {
            getCurrentProcesses();
        }

        private ObservableCollection<Process_Info> ProcessesInfo = new ObservableCollection<Process_Info>();  //当前进程集合
        private List<int> processesInfoList = new List<int>();                                                                                 //存储运行中的进程ID
        private List<int> newList = new List<int>();                                                                                                 //存储进程ID辅助链表
        //进程信息实时获取
        void getCurrentProcesses()
        {
            Process[] myProcesses = Process.GetProcesses();
            newList.Clear();
            foreach (Process myProcess in myProcesses)
            {
                if (!processesInfoList.Contains(myProcess.Id) && myProcess.Id != 0)                    //去除id=0的特殊进程
                {
                    processesInfoList.Add(myProcess.Id);
                    string status = myProcess.Responding == true ? "正-在-运-行-中……" : "无-响-应......";
                    ProcessesInfo.Add(new Process_Info(myProcess.ProcessName.ToString(),
                    myProcess.Id, myProcess.PrivateMemorySize64, myProcess.BasePriority, status));
                }
                else
                {
                    Process_Info pTemp = (from p in ProcessesInfo
                                         where p.ID == myProcess.Id
                                         select p).FirstOrDefault();
                    if (pTemp != null)
                    {
                        pTemp.PrivateMemorySize64 = myProcess.PrivateMemorySize64;
                        pTemp.BasePriority = myProcess.BasePriority;
                    }
                }
                newList.Add(myProcess.Id);
            }
            List<Process_Info> removedList = new List<Process_Info>();             //记入关闭的进程
            foreach (Process_Info process in ProcessesInfo)
            {
                if (!newList.Contains(process.ID))
                {
                    processesInfoList.Remove(process.ID);
                    removedList.Add(process);
                }
            }
            foreach (Process_Info process in removedList)
            {
                ProcessesInfo.Remove(process);                                                        //删除所有被关闭的进程
            }
            allProcessesListView.ItemsSource = ProcessesInfo;                                //进程列表
        }
        //点击列排序
        GridViewColumn _lastHeaderClicked = null;
        private void allProcessesListView_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader)
            {
                //获取点击的头列
                GridViewColumn clickedColumn = (e.OriginalSource as GridViewColumnHeader).Column;
                if (clickedColumn != null && clickedColumn.Header.ToString() != "选择")
                {
                    //获取绑定属性
                    string bindingProperty;
                    bindingProperty = (clickedColumn.DisplayMemberBinding as Binding).Path.Path;
                    SortDescriptionCollection sdc = allProcessesListView.Items.SortDescriptions;
                    ListSortDirection sortDirection = ListSortDirection.Ascending;
                    if (sdc.Count > 0)
                    {
                        SortDescription sd = sdc[0];
                        sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                        sdc.Clear();
                    }
                    if (sortDirection == ListSortDirection.Ascending)
                    {
                        clickedColumn.HeaderTemplate = Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else if (sortDirection == ListSortDirection.Descending)
                    {
                        clickedColumn.HeaderTemplate = Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }
                    if (_lastHeaderClicked != null && _lastHeaderClicked != clickedColumn)
                    {
                        _lastHeaderClicked.HeaderTemplate = null;
                    }
                    sdc.Add(new SortDescription(bindingProperty, sortDirection));
                    _lastHeaderClicked = clickedColumn;
                }
            }
        }

        //打开进程位置
        private void OpenFilePath_Click(object sender, RoutedEventArgs e)
        {
            if (allProcessesListView.SelectedItems.Count != 0)
            {
                try
                {
                    Process process = Process.GetProcessById(((Process_Info)allProcessesListView.SelectedItem).ID);
                    string path = process.MainModule.FileName;
                    Process.Start("explorer", "/select," + path);
                }
                catch
                {
                    MessageBox.Show("打开文件出错");
                }
            }
        }
        //内存曲线
        private void MemoryLine_Click(object sender, RoutedEventArgs e)
        {
            if (allProcessesListView.SelectedIndex >= 0)
            {
                string name = ((Process_Info)allProcessesListView.SelectedItem).Name;
                int id = ((Process_Info)allProcessesListView.SelectedItem).ID;
                SingleProcessMemoryLine memoryLine = new SingleProcessMemoryLine();
                memoryLine.processName.Text = name.Split('.')[0];
                memoryLine.processId.Text = id.ToString();
                memoryLine.ShowDialog();
            }
            else
                MessageBox.Show("无选中项！", "提醒", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        //进程cpu曲线
        private void singleCpuRate_Click(object sender, RoutedEventArgs e)
        {
            if (allProcessesListView.SelectedIndex >= 0)
            {
                string name = ((Process_Info)allProcessesListView.SelectedItem).Name;
                int id = ((Process_Info)allProcessesListView.SelectedItem).ID;
                SingleProcessCpuLine singleProcessCpuLine = new SingleProcessCpuLine();
                singleProcessCpuLine.processName.Text = name.Split('.')[0];
                singleProcessCpuLine.processId.Text = id.ToString();
                singleProcessCpuLine.ShowDialog();
            }
            else
                MessageBox.Show("无选中项！","提醒",MessageBoxButton.OK,MessageBoxImage.Warning);
        }
        //结束进程
        private void KillProcess_Click(object sender, RoutedEventArgs e)
        {
            if (allProcessesListView.SelectedItems.Count != 0)
            {
                if (MessageBox.Show("确定关闭选中进程?", "提醒", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Process process = Process.GetProcessById(((Process_Info)allProcessesListView.SelectedItem).ID);
                        process.Kill();
                    }
                    catch
                    {
                        MessageBox.Show("关闭出错");
                    }
                }
            }
            else
                MessageBox.Show("没有选中项！","提醒",MessageBoxButton.YesNo,MessageBoxImage.Warning);
        }
    }
}
