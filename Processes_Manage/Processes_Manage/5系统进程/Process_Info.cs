using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;

namespace Processes_Manage
{
    public class Process_Info:INotifyPropertyChanged                               //主类
    {
        #region Member_Manage
        private string _name;                           //进程名
        private int _id;                                     //进程id
        private long _privateMemorySize64;    //进程内存
        private int _basePriority;                      //进程优先级
        private string _state;                            //进程状态
        private TimeSpan _sensitiveDuration;

        public Process_Info()
        {
        }

        public Process_Info(string name, int id,long privateMemorySize64,int basePriority,string state)
        {
            _name = name;
            _id = id;
            _privateMemorySize64 = privateMemorySize64;
            _basePriority = basePriority;
            _state = state;
        }
      
        public string Name
        {
            get { return _name+".exe"; }
            set 
            {
                if (value != _name)
                {
                    _name = value; OnPropertyChanged("Name");
                }
            }
        }
        public int ID
        {
            get { return _id; }
            set 
            {
                if (_id != value)
                {
                    _id = value; OnPropertyChanged("ID");
                }
            }
        }
        public long PrivateMemorySize64
        {
            get { return _privateMemorySize64/1024; }
            set 
            {
                if (value!=_privateMemorySize64)
                {
                    _privateMemorySize64 = value; OnPropertyChanged("PrivateMemorySize64");
                }
            }
        }
        public int BasePriority
        {
            get { return _basePriority; }
            set 
            {
                if (value != _basePriority)
                {
                    _basePriority = value; OnPropertyChanged("BasePriority");
                }
            }
        }
        public string State
        {
            get { return _state; }
            set 
            {
                if (value != _state)
                {
                    _state = value; OnPropertyChanged("State");
                }
            }
        }
        public TimeSpan SensitiveDuration
        {
            get { return _sensitiveDuration; }
            set
            {
                if (value != _sensitiveDuration)
                {
                    _sensitiveDuration = value; OnPropertyChanged("SensitiveDuration");
                }
            }
        }
        #endregion Member_Mange

        #region Event_Manage
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string Info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(Info));
            }
        }
        #endregion Event_Manage
    }
}
