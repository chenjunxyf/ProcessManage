using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Net;

namespace Processes_Manage
{
    class SystemInfo
    {
        static ManagementObjectSearcher Processor = new ManagementObjectSearcher("select * from Win32_Processor");
        static ManagementObjectSearcher Os = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
        static ManagementObjectSearcher VideoController = new ManagementObjectSearcher("select * from Win32_VideoController");
        static ManagementObjectSearcher CompSys = new ManagementObjectSearcher("select * from Win32_ComputerSystem");

        public static string 物理内存()
        {
            return SystemInfoHelper.SysPhycicalMemory;
        }
        public static string 处理器()
        {
            return (string)GetValue(Processor, "Name");
        }
        public static string 处理器架构()
        {
            if (IntPtr.Size == 8)
                return "64位";
            else if (IntPtr.Size == 4)
                return "32位";
            else
                return "未知";
        }
        public static string Windows名称()
        {
            return (string)GetValue(Os, "Caption");
        }
        public static string Windows版本()
        {
            return Environment.OSVersion.Version.ToString();
        }
        public static string ServicePack()
        {
            if (!String.IsNullOrEmpty(Environment.OSVersion.ServicePack))
                return Environment.OSVersion.ServicePack;
            return "无";
        }
        public static string 系统目录()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.System);
        }
        public static string 显卡名称()
        {
            return (string)GetValue(VideoController, "Caption");
        }
        public static string 用户名()
        {
            return Environment.UserName;
        }
        public static string 计算机名称()
        {
            return Environment.UserDomainName;
        }
        public static string 工作组()
        {
            return (string)GetValue(CompSys, "Workgroup");
        }
        static object GetValue(ManagementObjectSearcher searcher, string propName)
        {
            foreach (ManagementObject mobj in searcher.Get())
                return mobj[propName];
            throw new NotSupportedException();
        }
    }
}
