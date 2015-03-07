using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;

namespace ProcessesManageService.SensitiveProcessesManage
{
    class ExceptionCatch
    {
        //是否有某种异常
        public static bool IsSuspicious(string pName,IntPtr hWnd)
        {
            if (JudgeMemoryOver(pName))
                return true;
            if (JudgeNetBehavior(pName))
                return true;
            if (JudgeFullWindowState(hWnd))
                return true;
            return false;
        }

        #region  内存行为
        private static bool JudgeMemoryOver(string pName)
        {
            Process[] processes = Process.GetProcessesByName(pName);
            float memoryUsed = 0;
            foreach (Process process in processes)
            {
                float memoryUsedTemp = process.PrivateMemorySize64 / 1024 / 1024;
                if (memoryUsed < memoryUsedTemp)
                    memoryUsed = memoryUsedTemp;
            }
            if (memoryUsed > 50)
                return true;
            else
                return false;
        }
        #endregion //内存行为

        #region 网络行为
        public static bool  JudgeNetBehavior(string pName)
        {
            if (IsOnline())
            {
                Process[] processes = Process.GetProcessesByName(pName);
                List<int> ports = PortsInfo.GetCommunicatedPorts();
                foreach (Process process in processes)
                {
                    if (ports.Contains(process.Id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private static bool IsOnline()
        {
            //Ping 百度
            return Ping("202.108.22.5");
        }
        private static bool Ping(string ip)
        {
            int timeout = 100;
            string data = "Test Data!";
            Ping p = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            PingReply reply = p.Send(ip, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }
        #endregion //网络行为

        #region      全屏行为
        //判别窗口状态基本函数
        private static bool JudgeFullWindowState(IntPtr hWnd)
        {
            if (IsWindow(hWnd))
            {
                RECT appBounds;
                Rectangle screenBounds;
                //取得窗口大小  
                GetWindowRect(hWnd, out appBounds);
                //判断是否全屏  
                screenBounds = System.Windows.Forms.Screen.FromHandle(hWnd).Bounds;
                if ((appBounds.Bottom - appBounds.Top) == screenBounds.Height && (appBounds.Right - appBounds.Left) == screenBounds.Width)
                {
                    return true;
                }
            }
            return false;
        }
        //判断窗口有效性
        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr hWnd);     
        //取得窗口大小函数 
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hwnd, out RECT rc);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        #endregion //全屏行为
    }
}
