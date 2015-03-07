using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;

namespace Processes_Manage
{
    class XMLFileOperations
    {
        //保存的xml文件名
        public static string XmlFilename = Application.Current.Properties["SavedDetailsFileName"].ToString();
        //保存路径
        public static string FullXmlPath = Application.Current.Properties["FullXmlPath"].ToString();

        //创建初始文件
        private static void CreateInitialFile()
        {
            XElement processesXmlDocument = new XElement("SoftWareInformation");
            processesXmlDocument.Save(FullXmlPath);
        }

        //追加XML到文件中，Load方法从文件加载
        public static void AppendToFile(ObservableCollection<TopWinProcessInfo> processes)
        {
            if (!File.Exists(FullXmlPath))
                CreateInitialFile();
            XElement processesXmlDocument = XElement.Load(FullXmlPath);
            //调用XElement对象追加进程信息
            for (int i = 0; i < processes.Count;i++ )
                processesXmlDocument.Add(new XElement("SoftWare",
                        new XElement("processName", processes[i].ProcessName),
                        new XElement("mainTitle", processes[i].MainTitle),
                        new XElement("startDate", processes[i].StartDate),
                        new XElement("startTime", processes[i].StartTime),
                        new XElement("usingTime", processes[i].UsingTime.ToString())
                        )
            );
            //Save方法将XML序列化为指定位置的文件
            processesXmlDocument.Save(FullXmlPath);
        }

        //使用LINQ to XML语法查询XML文件中进程信息
        public static List<TopWinProcessInfo> GetProcesses()
        { 
            var xmlProcessResults =
                from process in StreamElements(FullXmlPath, "SoftWare")
                select new TopWinProcessInfo
                {//将查询结果投影为一个Friend对象
                   ProcessName=process.Element("processName").SafeValue(),
                   MainTitle=process.Element("mainTitle").SafeValue(),
                   UsingTime=TimeSpan.Parse(process.Element("usingTime").SafeValue()),
                   StartDate = process.Element("startDate").SafeValue(),
                   StartTime=process.Element("startTime").SafeValue()
                };
            //返回所查询的列表
            return xmlProcessResults.ToList();
        }

        //读取xml内容
        public static IEnumerable<XElement> StreamElements(string uri, string name)
        {//返回指定文件中的XML元素的集合
            using (XmlReader reader = XmlReader.Create(uri))
            {//使用指定路径加载XML文件
                reader.MoveToContent();//跳至内容节点
                while (reader.Read())//读取内容节点
                {//如果节点类型为元素，且元素名称为指定的参数名称
                    if ((reader.NodeType == XmlNodeType.Element) &&
                      (reader.Name == name))
                    {//使用ReadFrom方法创建XElement，表示XLINQ节点
                        XElement element = (XElement)XElement.ReadFrom(reader);
                        yield return element;//向迭代器返回元素
                    }
                }
                reader.Close();
            }
        }

    }
}
