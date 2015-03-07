using System;
using System.ComponentModel;

namespace Processes_Manage
{
    //公有类 可当参数进行窗口之间传递
    public class UserInfo : INotifyPropertyChanged
    {
        private string _isOnline;          //是否在线
        private string _userName;       //用户名
        private string _userMac;         //用户mac
        private int _exceptionNum;    //用户异常次数  

        public string IsOnline
        {
            get { return _isOnline; }
            set { _isOnline = value; OnPropertyChanged("IsOnline"); }
        }
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        public string UserMac
        {
            get { return _userMac; }
            set { _userMac = value; }
        }
        public int ExceptionNum
        {
            get { return _exceptionNum; }
            set { _exceptionNum = value; OnPropertyChanged("ExceptionNum"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            var temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
