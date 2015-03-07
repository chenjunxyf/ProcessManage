using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.ObjectModel;

using Processes_Manage.CommunicationManage;   //通讯模块

namespace Processes_Manage
{
    /// <summary>
    /// Page5.xaml 的交互逻辑
    /// </summary>
    public partial class OnLineUsersPage : Page
    {
        #region      属性
        //超级用户Socket
        private ClientSocket superClientSocket;
        //接收信息事件
        private Action<string> ReceiveMessageAction;
        //敏感进程
        private ObservableCollection<SensitiveProcess> SensitiveProcesses;
        #endregion //属性

        #region     初始化
        //构造函数
        public OnLineUsersPage()
        {
            InitializeComponent();
        }
        //页面加载
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //超级用户Socket初始化
            superClientSocket = new ClientSocket();
            //敏感进程列表
            SensitiveProcesses = new ObservableCollection<SensitiveProcess>();
            //事件初始化
            ReceiveMessageAction = msg =>
            {
                ReceiveMessageHandle(msg);
            };
        }
        #endregion //初始化

        #region  连接 查看管理
        //连接服务器
        private void connectServer_Button_Click(object sender, RoutedEventArgs e)
        {
            superClientSocket.Access(new Action(() =>
            {
                try
                {
                    //首先发送自己的Super权限
                    superClientSocket.Send("SuperUser");
                    //开始异步接收服务器发送来的消息
                    superClientSocket.Receive(ReceiveMessageAction);
                    //连接按钮改变一下属性
                    connectServer_Button.Dispatcher.Invoke(new Action(() =>
                    {
                        connectServer_Button.IsEnabled = false;
                        connectServer_Button.Content = "连接中...";
                        //读取用户信息按钮获取使用权
                        readUsers_Button.IsEnabled = true;
                    }), null);
                }
                catch (Exception)
                {
                    return;
                }
            }));
        }
        //查看用户
        private void readUsers_Button_Click(object sender, RoutedEventArgs e)
        {
            if (readUsers_Button.Content.ToString() == "查看在线用户")
                readUsers_Button.Content = "刷新";
            //发送SuperUser Command
            superClientSocket.Send("2");
            //发送查看 在线用户消息
            superClientSocket.Send("OnlineUsers");
        }
        #endregion //连接 查看管理

        #region      消息管理
        private void ReceiveMessageHandle(string message)
        {
            string[] megs = message.Split('|');
            switch (megs[0])
            {
                case "OnlineUser":
                    //异步改变界面
                    usersListView.Dispatcher.Invoke(new Action(() =>
                    {
                        ////消息格式：mac+name+是否在线+exceptionNum
                        for (int i = 0; i < usersListView.Items.Count; i++)
                        {
                            UserInfo user = (UserInfo)usersListView.Items[i];
                            if (user.UserMac == megs[1])
                            {
                                user.IsOnline = megs[3];
                                user.ExceptionNum = int.Parse(megs[4]);
                                return;
                            }
                        }
                        usersListView.Items.Add(new UserInfo() { UserMac = megs[1], UserName = megs[2], IsOnline = megs[3], ExceptionNum = int.Parse(megs[4]) });
                    }), null);
                    break;
                case "RefreshOk":
                    MessageBox.Show("更新完毕","更新",MessageBoxButton.OK,MessageBoxImage.Information);
                    break;
                case "UserOffLine":
                    MessageBox.Show("对不起,该用户已经离线，无法对其控制，请刷新！","提醒",MessageBoxButton.OK,MessageBoxImage.Warning);
                    break;
                case "NoSProcess":
                    MessageBox.Show("无敏感进程","提醒",MessageBoxButton.OK,MessageBoxImage.Warning);
                    break;
                case "SProcess":
                    string[] megsTemp = megs[1].Split('-');
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    { SensitiveProcesses.Add(new SensitiveProcess 
                    { ProcessName = megsTemp[0], UsingTime = megsTemp[1], OrderTime=megsTemp[2],State = megsTemp[3] }); }), null);
                    break;
                case "KSuccess":
                    MessageBox.Show("操作成功！请刷新！","成功",MessageBoxButton.OK,MessageBoxImage.Information);
                    break;
            }
        }
        #endregion //消息管理

        #region    管理员操作
        //获取选中项 函数
        private UserInfo GetSelectedItem(RoutedEventArgs e)
        {
            //遍历视觉树 寻找选中项
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is ListViewItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep == null)
                return null;

            return (UserInfo)usersListView.ItemContainerGenerator.ItemFromContainer(dep);
        }
        //查看截图
        private void LookOver_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserInfo userInfo = GetSelectedItem(e);
                string path = "\\\\USER-FDCADK0KEH\\UsersManage\\" + userInfo.UserName + userInfo.UserMac.Replace(':', '-') + "\\";
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                Process.Start("explorer", path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //远程控制操作
        private void UserManage_Button_Click(object sender, RoutedEventArgs e)
        {
            //获取选中的用户
            UserInfo userInfo = GetSelectedItem(e);
            //控制窗口
            ControlUserWindow controlUserWindow = new ControlUserWindow();
            //读取敏感进程 button
            controlUserWindow.ReadSProsseses_Button.Click += delegate
            {
                //清空以前的记录
                SensitiveProcesses.Clear();
                superClientSocket.Send("2");
                superClientSocket.Send("SProcesses|" + userInfo.UserMac);
            };
            //结束选中进程 button
            controlUserWindow.KillProcess_Button.Click += delegate
            {
                if (controlUserWindow.SProcess_ListView.SelectedItems.Count != 0)
                {
                    if (MessageBox.Show("确定关闭选中的进程", "提醒", MessageBoxButton.OKCancel, MessageBoxImage.Warning)
                        == MessageBoxResult.OK)
                    {
                        SensitiveProcess sp=(SensitiveProcess)controlUserWindow.SProcess_ListView.SelectedItem;
                        if (sp.State == "已关闭")
                        {
                            MessageBox.Show("已关闭");
                            return;
                        }
                        superClientSocket.Send("2");
                        superClientSocket.Send("KSProcess|" + userInfo.UserMac + "|" + sp.ProcessName);
                    }
                }
                else
                {
                    MessageBox.Show("无选中项！", "提醒", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };
            //定时关闭
            controlUserWindow.orderTime_Button.Click += delegate
            {
                if (controlUserWindow.SProcess_ListView.SelectedIndex < 0)
                {
                    MessageBox.Show("无选中项！","提醒",MessageBoxButton.OK,MessageBoxImage.Warning);
                    return;
                }
                SensitiveProcess sp=(SensitiveProcess)controlUserWindow.SProcess_ListView.SelectedItem;
                if (TimeSpan.Parse(sp.UsingTime) > TimeSpan.Parse(controlUserWindow.orderTime_ComboBox.SelectionBoxItem.ToString()))
                {
                    MessageBox.Show("此进程已经使用超过设定时间！");
                    return;
                }
                superClientSocket.Send("2");
                superClientSocket.Send("KOSProcess|"+userInfo.UserMac+"|"+sp.ProcessName+"-"+
                    controlUserWindow.orderTime_ComboBox.SelectionBoxItem.ToString());
            };
            controlUserWindow.Closed+=delegate
            {
                SensitiveProcesses.Clear();
            };
            //listview
            controlUserWindow.SensitiveProcesses = SensitiveProcesses;
            controlUserWindow.Title = userInfo.UserName + " " + userInfo.UserMac;
            controlUserWindow.ShowDialog();
        }
        #endregion  //管理员操作
    }
}
