using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

using System.IO;
namespace ProcessesManageService.SensitiveProcessesManage
{
    class VerifySensitiveProcess : IDisposable
    {
        //键值:进程名和其对应的状态
        public static Dictionary<string, string> SensitiveProcessesDictionary = new Dictionary<string, string>();
        //进程状态检测次数
        private static int MonitoredNum = 30;
        //敏感异常状况处理
        public static Action SensitiveAction;

        #region      进程识别记录属性
        private string _monitoredName = "";                                 //被检测的进程名
        private int _actualMonitoredNum = 0;                               //实际进程状态检测次数

        private bool _isOnline = false;                                           //标志其是否有网络行为
        private PerformanceCounter _cpuPerformanceCounter;    //cpu性能计数器
        private List<float> _cpuUsage = new List<float>();           //cpu值记录
        private List<float> _memoryUsed = new List<float>();      //内存占用记录
        Thread _monitorThread;                                                   //检测线程
        #endregion  //进程识别记录属性

        //检测进程初始化构造函数
        public VerifySensitiveProcess(string monitoredName)
        {
            //资源字典中加入新的项 如果先前没有该项
            if (!IsExist(monitoredName))
                SensitiveProcessesDictionary.Add(monitoredName, "Monitoring");
            else
                //已经存在该项 则设置状态为检测中
                SetProcessState("Monitoring");

            _monitoredName = monitoredName;
            _cpuPerformanceCounter = new PerformanceCounter("Process", "% Processor Time", _monitoredName);
            _cpuPerformanceCounter.NextValue();

            _monitorThread = new Thread(new ThreadStart(MonitorThreading));
            _monitorThread.IsBackground = true;
            _monitorThread.Start();
        }

        #region      字典处理
        //判别字典是否存在某一项
        public static bool IsExist(string pName)
        {
            return SensitiveProcessesDictionary.Keys.Contains(pName);
        }
        //获取某一进程的状态
        public static string GetProcessState(string pName)
        {
            if (IsExist(pName))
                return SensitiveProcessesDictionary[pName];
            else
                return "";
        }
        //进程状态赋值
        private void SetProcessState(string state)
        {
            SensitiveProcessesDictionary[_monitoredName] = state;
        }
        #endregion  //字典处理

        #region    检测线程管理
        private void MonitorThreading()
        {
            while (_actualMonitoredNum < MonitoredNum)
            {
                GetProcessInfo();
                _actualMonitoredNum++;
                //收集到数据后 进行处理并释放资源
                if (_actualMonitoredNum == MonitoredNum)
                {
                    DealWithDataCollected();
                    Dispose();
                }
                Thread.Sleep(1000);
            }
        }
        //获取进程行为数据
        private void GetProcessInfo()
        {
            Process[] processes = Process.GetProcessesByName(_monitoredName);
            //同名进程数
            int processNum = processes.Count();
            //此进程在判别时关闭了  则释放资源，判别无效
            if (processNum == 0) //无此进程 值为0
            {
                SetProcessState("");
                Dispose();
                return;
            }
            else
            {
                //判别是否有网络行为
                if (_isOnline == false)
                {
                    if (ExceptionCatch.JudgeNetBehavior(_monitoredName))
                        _isOnline = true;
                }
                //cpu值记录一次
                AddCpuValue();
                //内存值记录
                AddMemoryValue(processes);
            }
        }
        #endregion //检测线程管理

        #region       cpu memory值记录
        //cpu值记录
        private void AddCpuValue()
        {
            //_cpu值记录
            _cpuUsage.Add(float.Parse(string.Format("{0:0}", _cpuPerformanceCounter.NextValue())));
        }
        //内存值记录
        private void AddMemoryValue(Process[] processes)
        {
            float memoryUsed = 0;
            foreach (Process process in processes)
            {
                float memoryUsedTemp = process.PrivateMemorySize64 / 1024 / 1024;
                if (memoryUsed < memoryUsedTemp)
                    memoryUsed = memoryUsedTemp;
            }
            _memoryUsed.Add(memoryUsed);
        }
        #endregion //cpu memory值记录

        #region         信息处理结果管理
        private void DealWithDataCollected()
        {
            //检测期间进程cpu占用值为0的次数
            int cpuZeroNum = _cpuUsage.Where(p => p == 0).Count();
            //cpu均值
            float cpuAverageUsed = 0;
            if (cpuZeroNum != MonitoredNum)
            {
                cpuAverageUsed = _cpuUsage.Average();
            }
            //内存使用均值
            float memoryAverageUsed = _memoryUsed.Average();

            //此区间 基本上是敏感的进程 如若不在阈值内 则标注怀疑 待下一次重检
            if (cpuZeroNum <= 3)
            {
                if (cpuAverageUsed > 10 || memoryAverageUsed > 60||_isOnline==true)
                    SetProcessState("Sensitive");
                else
                    SetProcessState("Suspicious");
            }
            //此区间 多出现IM 浏览器 音乐类程序
            else if (cpuZeroNum > 3 && cpuZeroNum <= 15)
            {
                if ((memoryAverageUsed > 60 && memoryAverageUsed < 100) || _isOnline == true)
                    SetProcessState("Sensitive");
                else if (cpuAverageUsed > 10 && memoryAverageUsed > 100)
                    SetProcessState("Sensitive");
                else
                    SetProcessState("Safe");
            }
            //多出现IM 正常程序
            else
            {
                if (memoryAverageUsed > 50 && _isOnline == true)
                    SetProcessState("Sensitive");
                else if (memoryAverageUsed > 50 || _isOnline == true)
                    SetProcessState("Suspicious");
                else
                    SetProcessState("Safe");
            }

            if (GetProcessState(_monitoredName) == "Sensitive")
            {
                SensitiveAction();
            }

            StreamWriter sw = new StreamWriter("C:\\data\\csss.txt", true);
            sw.WriteLine(_monitoredName + "  " + _isOnline.ToString() + "  " + cpuZeroNum.ToString() + "  " + cpuAverageUsed.ToString() + " " +
                memoryAverageUsed.ToString() + " " + SensitiveProcessesDictionary[_monitoredName]);
            sw.Close();
        }
        #endregion   //函数处理结果管理

        #region           资源管理
        private bool disposed = false;
        //实现Dispose接口 保证资源释放一次
        public void Dispose()
        {
            if (!this.disposed)
            {
                ClearAllResource();
                disposed = true;
            }
            GC.SuppressFinalize(this);
        }
        //释放资源
        private void ClearAllResource()
        {
            try
            {
                _monitorThread.Abort();        //监控线程退出
            }
            catch { }
            finally
            {
                _monitorThread = null;
            }
            _monitoredName = null;
            //cpu性能计数器资源释放
            _cpuPerformanceCounter.Dispose();
            _cpuUsage.Clear();                 //cpu值记录 释放
            _cpuUsage = null;
            _memoryUsed.Clear();            //内存值记录 释放
            _memoryUsed = null;
        }
        #endregion     //资源管理
    }
}
