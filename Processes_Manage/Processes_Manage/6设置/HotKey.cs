using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Collections;
using System.Windows.Controls;
using System.IO;

namespace Processes_Manage
{
    /// <summary>
    /// 直接构造类实例即可注册
    /// 自动完成注销
    /// 注意注册时会抛出异常
    /// </summary>
    public class HotKey
    //注册系统热键类
    //热键会随着程序结束自动解除,不会写入注册表
    {
        #region Member

        public int KeyId;        //热键编号
        static int num = 0;    //保证热键注册的唯一性
        IntPtr Handle;           //窗体句柄
        Window window;      //热键所在窗体
        uint Controlkey;        //热键控制键
        uint Key;                   //热键主键

        public bool IRightRegistered = true;  //热键注册成功标志

        public delegate void OnHotkeyEventHandeler();     //热键事件委托
        public event OnHotkeyEventHandeler OnHotKey = null;   //热键事件    

        static Hashtable KeyPair = new Hashtable();         //热键哈希表

        private const int WM_HOTKEY = 0x0312;       // 热键消息编号

        public enum KeyFlags    //控制键编码
        {
            MOD_ALT = 0x1,
            MOD_CONTROL = 0x2,
            MOD_SHIFT = 0x4,
            MOD_WIN = 0x8
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="win">注册窗体</param>
        /// <param name="control">控制键</param>
        /// <param name="key">主键</param>
        public HotKey()
        {
            KeyId = -1;  //还未注册
        }
        public HotKey(Window win, HotKey.KeyFlags control, System.Windows.Forms.Keys key)
        //构造函数,注册热键
        {
            Handle = new WindowInteropHelper(win).Handle;
            window = win;
            Controlkey = (uint)control;
            Key = (uint)key;
            KeyId = num++;

            if (HotKey.KeyPair.ContainsKey(KeyId))
            {
                IRightRegistered = false;
                MessageBox.Show("热键已经被注册!");
                return;
            }

            //注册热键
            if (false == HotKey.RegisterHotKey(Handle, KeyId, Controlkey, Key))
            {
                IRightRegistered = false;
                MessageBox.Show("热键注册失败!");
                return;
            }

            if (HotKey.KeyPair.Count == 0)
            {
                //消息挂钩只能连接一次!!
                if (false == InstallHotKeyHook(this))                       //创建
                {
                    IRightRegistered = false;
                    MessageBox.Show("消息挂钩连接失败!");
                    return;
                }
            }

            //添加这个热键索引
            HotKey.KeyPair.Add(KeyId, this);
        }

        #region core

        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint controlKey, uint virtualKey);

        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        static private bool InstallHotKeyHook(HotKey hk)
        //安装热键处理挂钩
        {
            if (hk.window == null || hk.Handle == IntPtr.Zero)
                return false;

            //获得消息源
            System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(hk.Handle);
            if (source == null) return false;

            //挂接事件
            source.AddHook(HotKey.HotKeyHook);
            return true;
        }

        static private IntPtr HotKeyHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //热键处理过程
        {
            if (msg == WM_HOTKEY)
            {
                HotKey hk = (HotKey)HotKey.KeyPair[(int)wParam];
                if (hk.OnHotKey != null) hk.OnHotKey();
            }
            return IntPtr.Zero;
        }

        public void IUnregisterHotKey()
        {
            HotKey.UnregisterHotKey(Handle, KeyId);
        }

        ~HotKey()
        //析构函数,解除热键
        {
            HotKey.UnregisterHotKey(Handle, KeyId);
        }

        #endregion
    }

    public class HotKeyFactory
    {
        public static HotKey hotKey = new HotKey();
        public static string path = Environment.CurrentDirectory + "\\HotKey.txt";
        //读取以前的注册记录
        public static void ReadKeyHistory()
        {
            if (File.Exists(path))
            {
                StreamReader streamReader = new StreamReader(path);
                string line = streamReader.ReadLine();
                if (line != null)
                {
                    string[] keyInfo = line.Split(':');
                    HotKey.KeyFlags control = HotKey.KeyFlags.MOD_ALT;
                    switch (int.Parse(keyInfo[0]))
                    {
                        case 0: control = HotKey.KeyFlags.MOD_ALT; break;
                        case 1: control = HotKey.KeyFlags.MOD_CONTROL; break;
                        case 2: control = HotKey.KeyFlags.MOD_SHIFT; break;
                        case 3: control = HotKey.KeyFlags.MOD_WIN; break;
                    }
                    RegisterHotKey(control, (System.Windows.Forms.Keys)((int)keyInfo[1].ToCharArray()[0]));
                }
                else
                    RegisterHotKey(HotKey.KeyFlags.MOD_ALT, System.Windows.Forms.Keys.A);
                streamReader.Close();
            }
            else  //注册默认热键 Alt+A
            {
                RegisterHotKey(HotKey.KeyFlags.MOD_ALT, System.Windows.Forms.Keys.A);
            }
        }
        //热键撤销
        public static void UnregisterHotKey()
        {
            hotKey.IUnregisterHotKey();
        }
        //热键注册
        public static void RegisterHotKey(HotKey.KeyFlags control, System.Windows.Forms.Keys key)
        {
            hotKey = new HotKey(Application.Current.MainWindow, control, key);
            hotKey.OnHotKey += new HotKey.OnHotkeyEventHandeler(hotKey_OnHotKey);
        }
        //热键响应事件
        private static void hotKey_OnHotKey()
        {
            if (Application.Current.MainWindow.Visibility == Visibility.Hidden)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                Application.Current.MainWindow.Show();
            }
        }
    }
}
