using System;

namespace Processes_Manage
{
    public class SensitiveProcess
    {
        private string _processName;  //敏感进程名
        private string _usingTime;       //敏感进程使用时间
        private string _orderTime;       //规定使用时间
        private string _state;               //进程状态

        public string ProcessName
        {
            get { return _processName; }
            set { _processName = value;}
        }
        public string UsingTime
        {
            get { return _usingTime; }
            set { _usingTime = value;}
        }
        public string OrderTime
        {
            get { return _orderTime; }
            set { _orderTime = value; }
        }
        public string State
        {
            get { return _state; }
            set { _state = value; }
        }
    }
}
