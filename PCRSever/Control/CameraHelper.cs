using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using PCRSever.ViewModel;

namespace PCRSever.Control
{
    public class CameraHelper
    {
        private IPEndPoint ipPoint;

        private UdpClient udpclient;
        private CameraHelper()
        {
            _CameraViewModel = new CameraViewModel();
            udpclient = new UdpClient();
        }
        private static CameraHelper _CameraHelper;
        private CameraViewModel _CameraViewModel;
        public static CameraHelper Singleton()
        {
            if (_CameraHelper==null)
            {
                _CameraHelper = new CameraHelper();
            }
            return _CameraHelper;
        }
        public void StartCamera()
        {
            _CameraViewModel.StartCamera();
        }
        public void CloseCamera()
        {
            _CameraViewModel.Close();
        }
        public void SendImage(IPEndPoint ip)
        {
            ipPoint = ip;
            _CameraViewModel.GetImageDelegate = SendImageDelegate;
            _CameraViewModel.SendImage();
        }
        private void SendImageDelegate(MemoryStream mss)
        {
           try
           {
                MemoryStream ms = new MemoryStream(mss.ToArray());
                int lenght = (int)ms.Length;
                byte[] data = new byte[10240];
                ms.Read(data, 0, lenght);
                udpclient.BeginSend(data, lenght, ipPoint, e =>
                    {
                        int length = udpclient.EndSend(e);
                        int a = 121;
                    }, null );
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常信息：{0}", ex.Message);
            }
        }
        public void NotSendImage()
        {
            _CameraViewModel.NotSendImage();
        }
    }
}
