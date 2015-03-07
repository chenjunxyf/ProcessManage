using System;
using System.Management;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net;

namespace Processes_Manage
{
    class SystemInfoHelper
    {
        #region API判断是否联网
        public static bool IsOnline { get; set; }           //是否联网
        public static string IPAddress { get; set; }       //Ip地址
        public static string IPSubnet { get; set; }        //子网掩码
        //更新网络信息
        public static void UpdateNetInfo()
        {
            GetIPInfo();
        }
        //判别是否连接某个网络
        [DllImport("wininet")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        private static bool JudgeOnline()
        {
            int i = 0;
            if (InternetGetConnectedState(out i, 0))
                return true;
            else
                return false;
        }
        //获取Ip相关信息
        private static void GetIPInfo()
        {
            IsOnline = JudgeOnline();
            if (IsOnline)
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection nics = mc.GetInstances();
                foreach (ManagementObject nic in nics)
                {
                    if (Convert.ToBoolean(nic["ipEnabled"]) == true)
                    {
                        IPAddress = "本机内网IP：" + (nic["IPAddress"] as string[])[0];
                        IPSubnet = "子网掩码：" + (nic["IPSubnet"] as string[])[0];
                        break;
                    }
                }
            }
            else
            {
                IPAddress = "本机内网IP：127.0.0.1";
                IPSubnet = "子网掩码：暂无";
            }
        }
        #endregion

        #region cpu使用率
        public static string SysCpuUsage { get; set; }
        private static PerformanceCounter cpuPerformance=new PerformanceCounter("Processor", "% Processor Time", "_Total");
        public static void UpdateCpuUsage()
        {
            SysCpuUsage = string.Format("{0:0}", cpuPerformance.NextValue());
        }
        #endregion 

        #region 内存使用率
        //内存管理
        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref MEMORY_INFO meminfo);
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_INFO
        {
            public uint dwLength;
            public uint dwMemoryLoad;  //内存使用率
            public uint dwTotalPhys;
            public uint dwAvailPhys;
            public uint dwTotalPageFile;
            public uint dwAvailPageFile;
            public uint dwTotalVirtual;
            public uint dwAvailVirtual;
        }
        public static string SysMemoryUsage { get; set; }       //内存使用率
        public static string SysPhycicalMemory { get; set; }    //物理内存
        public static void UpdateMemoryUsage()
        {
            MEMORY_INFO memoryInfo = new MEMORY_INFO();
            GlobalMemoryStatus(ref memoryInfo);
            SysMemoryUsage = memoryInfo.dwMemoryLoad.ToString();
            SysPhycicalMemory = (memoryInfo.dwTotalVirtual / 1024 / 1024).ToString() + "MB";
        }
        #endregion 

        #region       Gpu 温度
        //显卡温度管理
        [DllImport("gputemp.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int GetGPUTemperature(int idxGpu);
        public static int GetGPUTemperature()
        {
            return GetGPUTemperature(0);
        }
        #endregion //Gpu 温度

        #region      进程个数
        public static int GetProcessesNum()
        {
            return Process.GetProcesses().Length;
        }
        #endregion //进程个数
    }
}
