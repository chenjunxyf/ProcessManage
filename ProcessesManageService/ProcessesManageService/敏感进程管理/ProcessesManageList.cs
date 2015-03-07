using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace ProcessesManageService.SensitiveProcessesManage
{
    class ProcessesManageList
    {
        #region         基本属性
        //管理应用进程链表
        private static List<SensitiveProcess> _sensitiveProcesses;
        //基本时间间隔
        private static TimeSpan _timeInterval = new TimeSpan(0, 0, 1);
        //顶层时间 阈值 30秒钟
        private static TimeSpan _topInterval = new TimeSpan(0, 0, 30);
        //一般进程总时间检测间隔 10分钟（600秒）
        private static double _totalInterval = 600;
        //安全进程总时间检测间隔 30分钟 （1800秒）
        private static double _totalIntervalSafe = 1800;
        #endregion   //基本属性

        //构造函数
        public ProcessesManageList()
        {
            _sensitiveProcesses = new List<SensitiveProcess>();
        }
        //处理接口
        public void BeginManage()
        {
            TopWindowHandle();
            SensitiveProcessesHandle();
        }

        #region            顶层窗口进程捕获处理
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();  //获取前端窗口句柄
        private void TopWindowHandle()
        {
            IntPtr hWnd = GetForegroundWindow();
            Process topProcess = Process.GetProcesses().Where(process => process.MainWindowHandle == hWnd
                && process.MainWindowTitle != "" && process.ProcessName != "explorer").FirstOrDefault();
            if (topProcess != null)
            {
                SensitiveProcess sensitiveProcess = this.Exist(topProcess.ProcessName);
                if (sensitiveProcess == null)
                    this.Add(topProcess.ProcessName, hWnd);
                else
                {
                    sensitiveProcess.PTopTime += _timeInterval;
                    sensitiveProcess.PHandle = hWnd;
                }
            }
        }
        #endregion      //顶层窗口进程捕获处理

        #region          敏感进程判别间隔处理
        private void SensitiveProcessesHandle()
        {
            for (int i = 0; i < _sensitiveProcesses.Count; i++)
            {
                if (IsRunning(_sensitiveProcesses[i].PName))
                {
                    //总时间增一
                    _sensitiveProcesses[i].PTotalTime += _timeInterval;

                    //顶层时间超过1分钟的 可能会进行的首次处理
                    if (_sensitiveProcesses[i].PTopTime > _topInterval &&
                        VerifySensitiveProcess.IsExist(_sensitiveProcesses[i].PName) == false)
                    {
                        ExceptionThenMonitor(_sensitiveProcesses[i].PName, _sensitiveProcesses[i].PHandle);
                    }

                    string pState = VerifySensitiveProcess.GetProcessState(_sensitiveProcesses[i].PName);
                    //如果已判别为正在检测中 则跳过
                    if(pState == "Monitoring")
                        continue;
                    //如果状态为敏感进程 则判别是否使用超时
                    if (pState == "Sensitive")
                    {
                        if (_sensitiveProcesses[i].POrderTime <= _sensitiveProcesses[i].PTotalTime)
                        {
                            Process[] processes = Process.GetProcessesByName(_sensitiveProcesses[i].PName);
                            foreach (Process process in processes)
                                process.Kill();
                        }
                        continue;
                    }
                    //如果已经判别为安全进程 则检测间隔为30分钟
                    if (pState == "Safe")
                    {
                        if (_sensitiveProcesses[i].PTopTime.TotalSeconds % _totalIntervalSafe == 0)
                            ExceptionThenMonitor(_sensitiveProcesses[i].PName, _sensitiveProcesses[i].PHandle);
                        continue;
                    }
                    //怀疑进程或一般进程 按时间间隔10分钟捕获一次异常
                    if (_sensitiveProcesses[i].PTotalTime.TotalSeconds % _totalInterval == 0)
                    {
                        ExceptionThenMonitor(_sensitiveProcesses[i].PName, _sensitiveProcesses[i].PHandle);
                    }
                }
            }
        }
        //判别异常 进行检测
        private void ExceptionThenMonitor(string pName, IntPtr hWnd)
        {
            if (ExceptionCatch.IsSuspicious(pName, hWnd))
            {
                new VerifySensitiveProcess(pName);
            }
        }
        #endregion    //敏感进程判别间隔处理

        #region 辅助函数
        //判别进程是否任然在运行
        private static bool IsRunning(string pName)
        {
            return Process.GetProcessesByName(pName).Count() > 0 ? true : false;
        }
        //添加新程序
        private void Add(string pName, IntPtr pHandle)
        {
            _sensitiveProcesses.Add(new SensitiveProcess(pName, pHandle));
        }
        //判断是否已经存在该项
        private SensitiveProcess Exist(string pName)
        {
            for (int i = 0; i < _sensitiveProcesses.Count; i++)
            {
                if (_sensitiveProcesses[i].PName == pName)
                    return _sensitiveProcesses[i];
            }
            return null;
        }
        //获取所有敏感进程
        public static List<string> GetAllSensitiveProcesses()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < _sensitiveProcesses.Count; i++)
            {
                string state = VerifySensitiveProcess.GetProcessState(_sensitiveProcesses[i].PName);
                if (state == "Sensitive")
                {
                    string pState = "已关闭";
                    if (IsRunning(_sensitiveProcesses[i].PName))
                        pState = "运行中";
                    temp.Add(_sensitiveProcesses[i].PName + "-" + _sensitiveProcesses[i].PTotalTime.ToString()+"-"
                        +_sensitiveProcesses[i].POrderTime.ToString() + "-" + pState);
                }
            }
            return temp;
        }
        //获取指定的敏感进程
        public static void SetSProcessOrderTime(string name,string time)
        {
            for (int i = 0; i < _sensitiveProcesses.Count; i++)
            {
                if (_sensitiveProcesses[i].PName == name)
                {
                    _sensitiveProcesses[i].POrderTime = TimeSpan.Parse(time);
                    break;
                }
            }
        }
        //清除管理链表
        public void Clear()
        {
            _sensitiveProcesses.Clear();
            _sensitiveProcesses = null;
        }
        #endregion //辅助函数
    }
}
