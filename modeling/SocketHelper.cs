using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace wfapp1
{
    class SocketHelper
    {
       private Socket client = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public SocketHelper(string ip,int port)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
        }

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="ip">python ip 地址</param>
        /// <param name="port">python 端口号</param>
        /// <param name="msg">消息</param>
        /// <param name="data">数据</param>
        /// <returns>返回值</returns>
        public string sendProcess(string ip,int port,string msg, string data)
        {
            //发送计算数据
            EndPoint point = new IPEndPoint(IPAddress.Parse(ip), port);
            client.SendTo(Encoding.UTF8.GetBytes(msg+":"+data), point);

            //得到计算结果
            EndPoint rpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = new byte[1024];
            int length = client.ReceiveFrom(buffer, ref rpoint);
            string str = System.Text.Encoding.Default.GetString(buffer);

            //结果
            return str;
        }

        /// <summary>
        /// 发送退出
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void sendQuit(string ip, int port)
        {
            sendProcess(ip, port,"quit","");
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void close()
        {
            client.Close();
        }


    }
}
