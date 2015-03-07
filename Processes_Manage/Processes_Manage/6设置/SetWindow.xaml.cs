using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.IO;

namespace Processes_Manage
{
    /// <summary>
    /// SetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetWindow : Window
    {
        public SetWindow()
        {
            InitializeComponent();
        }

        private void ok_Button_Click(object sender, RoutedEventArgs e)
        {
            if (hotKey1_ComboBox.SelectedIndex < 0 || hotKey2_ComboBox.SelectedIndex<0)
            {
                MessageBox.Show("信息选择不完全！","提醒",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }
            HotKey.KeyFlags control=HotKey.KeyFlags.MOD_ALT;
            int index = 0;
            switch (hotKey1_ComboBox.SelectedIndex)
            {
                case 0: control = HotKey.KeyFlags.MOD_ALT; break;
                case 1: control = HotKey.KeyFlags.MOD_CONTROL; index = 1; break;
                case 2: control = HotKey.KeyFlags.MOD_SHIFT; index = 2; break;
                case 3: control = HotKey.KeyFlags.MOD_WIN; index = 3; break;
            }

            //撤销先前的热键
            HotKeyFactory.UnregisterHotKey();
            //注册新的热键
            HotKeyFactory.RegisterHotKey(control,(System.Windows.Forms.Keys)hotKey2_ComboBox.SelectedItem);

            if (HotKeyFactory.hotKey.IRightRegistered)
            {
                MessageBox.Show("热键注册成功！");
                FileStream fs = new FileStream(HotKeyFactory.path, FileMode.Create, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fs);
                streamWriter.Flush();
                streamWriter.WriteLine(index.ToString()+":"+hotKey2_ComboBox.SelectedItem.ToString());
                streamWriter.Flush();
                streamWriter.Close();
            }
            else
                MessageBox.Show("热键注册失败！");
        }
    }
}
