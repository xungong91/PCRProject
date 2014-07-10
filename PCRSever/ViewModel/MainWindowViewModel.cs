using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PCRSever.Control;
using PCRSever.Interface;
using PCRSever.View;
using System.Windows.Controls;
using System.Windows.Input;



namespace PCRSever.ViewModel
{
    class MainWindowViewModel : ViewModelBase, IPCRSocket
    {
        public RelayCommand StartCommand { get; set; }
        public RelayCommand<KeyEventArgs> TextBoxKeyDown { get; set; }
        public RelayCommand StartControlCommand { get; set; }
        public RelayCommand CameraStartCommand { get; set; }
        private PCRSocket PCRSocket;
        public MainWindowViewModel()
        {
            InputHelper.Singleton().setIPCRSocket(this);
            CreateKey();
            StartCommand = new RelayCommand
            (
            () =>
            {
                SelectIpView sv = new SelectIpView();
                sv.DataContext = new SelectIpViewModel()
                {
                    StringDelegate = new Action<string>
                        ((e) =>
                        {
                            if (PCRSocket==null)
                            {
                                PCRSocket = new PCRSocket(this, Key);
                                PCRSocket.Listen(e);
                            }
                        }
                        ),
                       window=sv
                };
                sv.ShowDialog();
            }
            );
            StartControlCommand = new RelayCommand
            (
            () =>
            {
                SelectIpView sv = new SelectIpView();
                sv.DataContext = new SelectIpViewModel()
                {
                    StringDelegate = new Action<string>
                        ((e) =>
                        {
                            if (PCRSocket == null)
                            {
                                PCRSocket = new PCRSocket(this, Key);
                                PCRSocket.ListenControl(e);
                            }
                        }
                        ),
                    window = sv
                };
                sv.ShowDialog();
            }
            );
            TextBoxKeyDown = new RelayCommand<KeyEventArgs>
            (
            (e)=>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    InputHelper.Singleton().setInputTest(Defines.MainW.tb.Text);
                }
            }
            );
            CameraStartCommand = new RelayCommand
            (
            () =>
            {
                CameraView cv = new CameraView();
                cv.Show();
            }
            );
        }
        private void CreateKey()
        {
            Random rand=new Random();
            Key = rand.Next(100000, 999999);
            //Key = 111111;
        }
        #region 属性
        private int _key;
        /// <summary>
        /// 秘钥
        /// </summary>
        public int Key
        {
            get { return _key; }
            set 
            {
                RaisePropertyChanging("Key");
                _key = value;
                RaisePropertyChanged("Key");
            }
        }
        private string _showMsg;
        /// <summary>
        /// 展示信息
        /// </summary>
        public string ShowMsg
        {
            get { return _showMsg; }
            set
            {
                RaisePropertyChanging("ShowMsg");
                _showMsg = value;
                RaisePropertyChanged("ShowMsg");
            }
        }
        #endregion

        public void SocketMessage(string msg)
        {
            //ShowMsg += msg;
            //ShowMsg += "\n";
            Defines.MainW.Dispatcher.BeginInvoke(new Action(() => 
            { 
                Defines.MainW.tb.Text +=string.Format("[{0}]{1}\n",DateTime.Now.ToShortTimeString(),msg); 
                Defines.MainW.tb.ScrollToEnd();
                Defines.MainW.tb.SelectionStart = Defines.MainW.tb.Text.Length;
            }));
        }
    }
}
