using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;

namespace Processes_Manage
{
    //顶层窗口 进程信息类
    class TopWinProcessInfo:INotifyPropertyChanged
    {
        private string _processName;   //进程名
        private string _mainTitle;          //进程窗口名 
        private string _startTime;          //启动时间
        private string _startDate;          //启动日期

        //private string _usingTime;         //使用时间
        private TimeSpan _usingTime;    

        public TopWinProcessInfo()
        { }

        public string ProcessName
        {
            get{return _processName;}
            set{_processName=value; }
        }
        public string MainTitle
        {
            get { return _mainTitle; }
            set { _mainTitle = value;}
        }
        public string StartTime
        {
            get { return _startTime; }
            set { _startTime = value;}
        }
        public string StartDate
        {
            get { return _startTime.Split(' ')[0]; }
            set { _startDate = value; }
        }
        public TimeSpan UsingTime
        {
            get { return _usingTime; }
            set { _usingTime = value; OnPropertyChanged("UsingTime"); }
        }
        public TopWinProcessInfo(string processName,string mainTitle,string startTime)
        {
            _processName = processName;
            _mainTitle = mainTitle;
            _startTime = startTime;
            _usingTime = TimeSpan.FromSeconds(1);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string Info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(Info));
            }
        }
    }
}
