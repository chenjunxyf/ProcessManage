using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace ProcessesManageService.SensitiveProcessesManage
{
    class PortsInfo
    {
        //获取通信的端口
        public static List<int> GetCommunicatedPorts()
        {
            string cmdtext = "netstat -an -o";
            Process commandProcess = CommandBase(cmdtext);
            StreamReader reader = commandProcess.StandardOutput;  //截取输出流
            //前8行为无用信息
            for (int i = 0; i < 8; i++)
                reader.ReadLine();
            List<int> portsInfoList=new List<int>();   //保存通信端口号
            string temp = reader.ReadLine();             
            while (!reader.EndOfStream)                    //只要有信息就处理
            {
                if (temp.Contains("ESTABLISHED"))       //"ESTABLISHED"标志 此端口的通信行为
                {
                    string[] portInfo = temp.Split(' ');      //字符串分割
                    int port = int.Parse(portInfo[portInfo.Length-1].ToString());      //最后的为端口号
                    if (!portsInfoList.Contains(port))
                        portsInfoList.Add(port);
                }
                temp = reader.ReadLine();
            }
            reader.Close();                       //输出流的关闭
            return portsInfoList;              //返回通信的端口号
        }
        //命令处理基本格式
        private static Process CommandBase(string cmdtext)
        {
            Process MyProcess = new Process();
            //设定程序名 
            MyProcess.StartInfo.FileName = "cmd.exe";
            //关闭Shell的使用 
            MyProcess.StartInfo.UseShellExecute = false;
            //重定向标准输入 
            MyProcess.StartInfo.RedirectStandardInput = true;
            //重定向标准输出 
            MyProcess.StartInfo.RedirectStandardOutput = true;
            //重定向错误输出 
            MyProcess.StartInfo.RedirectStandardError = true;
            //设置不显示窗口 
            MyProcess.StartInfo.CreateNoWindow = true;
            //执行VER命令 
            MyProcess.Start();
            MyProcess.StandardInput.WriteLine(cmdtext);
            //退出
            MyProcess.StandardInput.WriteLine("exit");
            return MyProcess;
        }
    }
}
