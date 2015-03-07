using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProcessesManageService.CommunicationManage
{
    public abstract class SocketFunc
    {
        public string Ip = "127.0.0.1";                                               //Ip地址
        public int Port = 9050;                                                         //端口
        public Socket communicateSocket = null;                            //通信套接字
        public abstract void Access(Action AcessAction);                  //连接

        //发送消息的函数
        public void Send(string message)
        {
            Byte[] msg = null;
            //消息格式为数字+真正的消息   数字用于区别发送文件消息
            long result = 0;
            if (long.TryParse(message, out result))
                msg = BitConverter.GetBytes(result);
            else
                msg = Encoding.UTF8.GetBytes(message);

            try
            {
                communicateSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None,
                    ar => { }, null);
            }
            catch (Exception)
            {
                communicateSocket.Close();
                return;
            }
        }

        //接受消息的函数
        public void Receive(Action<string> ReceiveAction)
        {
            //如果消息超过1024个字节, 收到的消息会分为(总字节长度/1024 +1)条显示
            Byte[] msg = new byte[1024];
            //异步的接受消息
            communicateSocket.BeginReceive(msg, 0, msg.Length, SocketFlags.None,
                ar =>
                {
                    //对方断开连接时, 这里抛出Socket Exception
                    try
                    {
                        communicateSocket.EndReceive(ar);
                        ReceiveAction(Encoding.UTF8.GetString(msg).Trim('\0', ' '));
                        Receive(ReceiveAction);
                    }
                    catch (Exception)
                    {
                        communicateSocket.Close();
                        return;
                    }
                }, null);
        }
    }
}
