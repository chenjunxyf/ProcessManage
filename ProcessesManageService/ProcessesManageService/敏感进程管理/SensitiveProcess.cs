using System;

namespace ProcessesManageService.SensitiveProcessesManage
{
    public class SensitiveProcess
    {
        private string _pName;                  //进程名
        private TimeSpan _pTopTime;       //处于顶层窗口时间
        private TimeSpan _pTotalTime;     //已经运行的总时间
        private TimeSpan _pOrderTime;    //规定使用时间
        private IntPtr _pHandle;                //进程窗口句柄
        private string _state;                     //进程的状态标记：是否敏感？怀疑还是。。

        public SensitiveProcess(string pName, IntPtr pHandle)
        {
            _pName = pName;
            _pTopTime = new TimeSpan(0, 0, 1);
            _pTotalTime = new TimeSpan(0, 0, 0);
            _pOrderTime = new TimeSpan(2,0,0);
            _pHandle = pHandle;
            _state = "waiting";
        }

        public string PName
        {
            get { return _pName; }
            set { _pName = value; }
        }
        public TimeSpan PTopTime
        {
            get { return _pTopTime; }
            set { _pTopTime = value; }
        }
        public TimeSpan PTotalTime
        {
            get { return _pTotalTime; }
            set { _pTotalTime = value; }
        }
        public TimeSpan POrderTime
        {
            get { return _pOrderTime; }
            set { _pOrderTime = value; }
        }
        public IntPtr PHandle
        {
            get { return _pHandle; }
            set { _pHandle = value; }
        }
        public string State
        {
            get { return _state; }
            set { _state = value; }
        }
    }
}
