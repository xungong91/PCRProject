using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Net;

namespace PCRSever.ViewModel
{
    public class SelectIpViewModel : ViewModelBase
    {
        public RelayCommand<MouseButtonEventArgs> StartCommand { get; set; }
        public ObservableCollection<string> listboxlist { get; set; }
        public Action<string> StringDelegate;
        private System.Windows.Window _window;
        public System.Windows.Window window
        {
            get { return _window; }
            set
            {
                _window = value;
                IPAddress[] localAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList;  //获取本地ip
                for (int i = 0; i < localAddress.Length; i++)
                {
                    IPAddress Address = localAddress[i];
                    if (!Address.IsIPv6LinkLocal)
                    {
                        listboxlist.Add(Address.ToString());
                    }
                }
            }
        }
        public SelectIpViewModel()
        {
            listboxlist = new ObservableCollection<string>();
            StartCommand = new RelayCommand<MouseButtonEventArgs>
            (
            (e) =>
            {
                ListBox listbox = (ListBox)e.Source;
                if (listbox.SelectedItems.Count>0)
                {
                    string select = listbox.SelectedItems[0].ToString();
                    if (StringDelegate != null)
                    {
                        StringDelegate(select);
                        window.Close();
                    }
                }
            }
            );
        }
       
    }
}
