using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using PCRSever.API;

namespace PCRSever.ViewModel
{
    public class CameraViewModel : ViewModelBase
    {
        private ImageSource _imagesource;

        public ImageSource Imagesource
        {
            get { return _imagesource; }
            set
            {
                RaisePropertyChanging("Imagesource");
                _imagesource = value;
                RaisePropertyChanged("Imagesource");
            }
        }
        public Action<MemoryStream> GetImageDelegate{set;get;}
        private DispatcherTimer timer;
        private Capture cam;
        private bool IsOpenCamera;
        public CameraViewModel()
        {
            Defines.MainW.Dispatcher.BeginInvoke(new Action(() =>
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(30);
            }));
            IsOpenCamera = false;
        }
        public void StartCamera()
        {
            if (!IsOpenCamera)
            {
                IsOpenCamera = true;
                cam = new Capture(0, 10, 320, 240);
                cam.Start();
                //timer.Tick += StartPlayVideo;  //你的事件
                //timer.Start();
            }
        }
        public void Close()
        {
            if (IsOpenCamera)
            {
                IsOpenCamera = false;
                NotSendImage();
                cam.Dispose();
            }
        }
        public void SendImage()
        {
            if (IsOpenCamera)
            {
                Defines.MainW.Dispatcher.BeginInvoke(new Action(() =>
                {
                    timer.Tick -= StartSendVideo;
                    timer.Tick += StartSendVideo;
                    timer.Start();
                }));
            }
        }
        public void NotSendImage()
        {
            Defines.MainW.Dispatcher.BeginInvoke(new Action(() =>
            {
                timer.Stop();
            }));
        }
        private void StartSendVideo(object sender, EventArgs e)
        {
            //timer.Stop();
            MemoryStream ms = new MemoryStream();
            Bitmap bitmap = null;
            IntPtr ip = IntPtr.Zero;
            ip = cam.GetBitMap();
            bitmap = new Bitmap(cam.Width, cam.Height, cam.Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, ip);
            //旋转
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
             //不安全的刷新方式
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch
            { return; }
            //释放指针ip
            Marshal.FreeCoTaskMem(ip);
            //回收垃圾
            GC.Collect();
            if (GetImageDelegate!=null)
            {
                GetImageDelegate(ms);
            }
        }
        private void StartPlayVideo(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            Bitmap bitmap = null;
            IntPtr ip = IntPtr.Zero;
            ip = cam.GetBitMap();
            bitmap = new Bitmap(cam.Width, cam.Height, cam.Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, ip);
            //旋转
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            //不安全的刷新方式
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                Imagesource = bitmapImage;
            }
            catch (Exception ex)
            {
                global::System.Windows.MessageBox.Show(ex.ToString());
                throw;
            }
            //释放指针ip
            Marshal.FreeCoTaskMem(ip);
            //回收垃圾
            GC.Collect();
        }
    }
}
