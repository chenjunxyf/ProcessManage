using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Processes_Manage
{
    class ProcessesLogList : ObservableCollection<TopWinProcessInfo>
    {
        private static ProcessesLogList instance = null;
        public static ProcessesLogList GetInstance()
        {
            if (instance==null||instance.Count==0)
                instance = new ProcessesLogList();
            return instance;
        }
        public static void ClearLog()
        {
            instance.Clear();
        }
        private ProcessesLogList()
        {
            try 
            {
                List<TopWinProcessInfo> processes = XMLFileOperations.GetProcesses();
                for (int i = 0; i < processes.Count;i++ )
                    this.Add(processes[i]);
            }
            catch{ }
        }
    }
}
