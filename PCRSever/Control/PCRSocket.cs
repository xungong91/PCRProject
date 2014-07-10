using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCRSever.Interface;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace PCRSever.Control
{
    public class PCRSocket
    {
        static public List<Socket> mClients = new List<Socket>();
        public IPCRSocket _mainModel;
        private Socket m_socket;
        private Socket m_tcpClient;
        private int m_key;
        public MsgType mConnectionMsgType = MsgType.CONNECTION_SUCCESSFUL;
        public PCRSocket() { }
        public PCRSocket(IPCRSocket mainModel, int key)
        {
            InputHelper.Singleton().setPCRSocket(this);
            _mainModel = mainModel;
            m_key = key;
        }
        public void ListenControl(string ip)
        {
            Listen(ip);
            mConnectionMsgType = MsgType.CONNECTION_CONTROL;
        }
        public void Listen(string ip)
        {
            try
            {
                mConnectionMsgType = MsgType.CONNECTION_SUCCESSFUL;
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), Defines.Port);
                Thread thread = new Thread(new ParameterizedThreadStart(UdpSever)) { IsBackground=true};
                thread.Start(ipEndPoint);
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_socket.Bind(ipEndPoint);
                m_socket.Listen(20);
                _mainModel.SocketMessage(string.Format("开始监听IP:{0},端口:{1}", ipEndPoint.Address, ipEndPoint.Port));
                _mainModel.SocketMessage("-------------");
                AsynAccept(m_socket);
            }
            catch (Exception ex)
            {
                _mainModel.SocketMessage(ex.Message);
                _mainModel.SocketMessage("-------------");
            }
        }
        private void CloseClient(Socket tcpClient)
        {
            IPEndPoint clientipe = (IPEndPoint)tcpClient.RemoteEndPoint;
            _mainModel.SocketMessage("-------------");
            _mainModel.SocketMessage(string.Format("断开IP:{0},端口:{1}", clientipe.Address, clientipe.Port));
            _mainModel.SocketMessage("-------------");
            tcpClient.Close();
        }
        private bool EqualKey(Socket tcpClient)
        {
            bool bRet = false;
            byte[] buffer = new byte[1024];
            tcpClient.Receive(buffer);
            try
            {
                int key = BitConverter.ToInt32(buffer, 0);
                if (key == m_key)
                {
                    bRet = true;
                }
            }
            catch { }
            return bRet;
        }
        private void AsynAccept(Socket tcpServer)
        {
            tcpServer.BeginAccept(asyncResult =>
            {
                try
                {
                    Socket tcpClient = tcpServer.EndAccept(asyncResult);
                    mClients.Add(tcpClient);
                    IPEndPoint clientipe = (IPEndPoint)tcpClient.RemoteEndPoint;
                    _mainModel.SocketMessage(string.Format("收到连接请求IP:{0},端口:{1}", clientipe.Address, clientipe.Port));
                    _mainModel.SocketMessage("准备验证秘钥");
                    //继续接受连接请求
                    AsynAccept(tcpServer);

                    int sendLength;
                    byte[] senddata;
                    if (!EqualKey(tcpClient))
                    {
                        _mainModel.SocketMessage("秘钥验证失败，断开该连接");
                        MessageCenter.SendMsg(MsgType.CONNECTION_FAILURE,out senddata, out sendLength);
                        AsynSend(tcpClient, senddata, sendLength);
                        return;
                    }
                    _mainModel.SocketMessage("秘钥验证成功，开始连接");
                    _mainModel.SocketMessage("---------------");
                    MessageCenter.SendMsg(mConnectionMsgType, out senddata, out sendLength, clientipe);
                    m_tcpClient = tcpClient;
                    AsynSend(tcpClient, senddata, sendLength);
                    //接受信息
                    AsynRecive(tcpClient);
                }
                catch (Exception ex)
                {
                    _mainModel.SocketMessage(string.Format("出错，错误原因{0}", ex.Message));
                    _mainModel.SocketMessage("-------------");
                }
            }, null);

        }
        private void AsynRecive(Socket tcpClient)
        {
            byte[] data = new byte[1024];
            tcpClient.BeginReceive(data, 0, data.Length, SocketFlags.None,
                asyncResult =>
                {
                    try
                    {
                        int length = tcpClient.EndReceive(asyncResult);
                        if (length>0)
                        {
                            MessageCenter.ReciveMsg(data, length, _mainModel, (IPEndPoint)tcpClient.RemoteEndPoint);
                        }
                        AsynRecive(tcpClient);
                    }
                    catch
                    {
                        mClients.Remove(tcpClient);
                        _mainModel.SocketMessage("已经断开连接");
                        _mainModel.SocketMessage("-------------");
                    }
                }, null);
        }
        private void AsynSend(Socket tcpClient, byte[] data,int lenght)
        {
            try
            {
                tcpClient.BeginSend(data, 0, lenght, SocketFlags.None, asyncResult =>
                {
                    //完成发送消息
                    int length = tcpClient.EndSend(asyncResult);
                }, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常信息：{0}", ex.Message);
            }
        }
        public void sendSetColorMsg(string color, int index = 0)
        {
            int sendLength;
            byte[] senddata;
            _mainModel.SocketMessage(string.Format("设置颜色 准备设置{0}号客户端:颜色rgba为{1}",index, color));
            MessageCenter.SendMsg(MsgType.CONNECTION_CONTROL_COLOR, out senddata, out sendLength, color);
            AsynSend(mClients[index], senddata, sendLength);
        }
        private void UdpSever(object obi)
        {
            IPEndPoint ip = (IPEndPoint)obi;
            //ip.Port = 65432;
            int recv;
            byte[] data = new byte[1024];
            //得到本机IP，设置TCP端口号        
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //绑定网络地址
            newsock.Bind(ip);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
            //Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            while (true)
            {
                recv = newsock.ReceiveFrom(data, ref Remote);
                if (recv>0)
                {
                    MsgType type = (MsgType)Convert.ToInt32(data[0]);
                    if (type==MsgType.CONNECTION_NAT_UDP_CTS)
                    {
                        int msglength;
                        msglength = (int)(data[1] | data[2] << 8);
                        byte[] msg = new byte[msglength];
                        Array.Copy(data, 3, msg, 0, msglength);
                        _mainModel.SocketMessage(string.Format("收到udp打洞请求，ip和端口号为:{0}", ((IPEndPoint)Remote).ToString()));
                        int sendLength;
                        byte[] senddata;
                        MessageCenter.SendMsg(MsgType.CONNECTION_NAT_UDP_STC, out senddata, out sendLength, (IPEndPoint)Remote);
                        AsynSend(m_tcpClient, senddata, sendLength);
                    }
                    else
                    {
                        MessageCenter.ReciveMsg(data, recv, _mainModel, (IPEndPoint)Remote);
                    }
                }
            }
        }
    }
}
