using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using System.Threading;

namespace PCRSever.Control
{
    public class KeyboardHelper
    {
        //static object lockobj = new object();
        //static object lockupobj = new object();
        private List<KeyModel> key_list;
        private KeyboardHelper()
        {
            key_list = new List<KeyModel>();
        }
        private static KeyboardHelper _KeyboardHelper;
        public static KeyboardHelper Singleton()
        {
            if (_KeyboardHelper==null)
            {
                _KeyboardHelper = new KeyboardHelper();
            }
            return _KeyboardHelper;
        }

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        //public void KeyDown(byte key)
        //{
        //    keybd_event(key, 0, 0, 0);
        //    KeyModel keymodel=new KeyModel(){data=key};
        //    keymodel.timer = new Timer(KeyUp, keymodel, 100, 5000);
        //}
        //public void KeyUp(object key)
        //{
        //    if (key is KeyModel)
        //    {
        //        KeyModel keymodel = (KeyModel)key;

        //        if (keymodel.timer != null)
        //        {
        //            keymodel.timer.Dispose();
        //        }
        //        keybd_event(keymodel.data, 0, 0x2, 0);
        //    }
        //    else
        //        keybd_event((byte)key, 0, 0x2, 0);
        //}
        public void EasyDown(byte key)
        {
            keybd_event(key, 0, 0, 0);
        }
        public void EasyUp(byte key)
        {
            keybd_event((byte)key, 0, 0x2, 0);
        }
        public void KeyDown(byte key)
        {
            var v1 = from d in key_list
                     where d.data == key
                     select d;
            if (v1.Count() > 0)
            {
                KeyModel list = v1.First();
                if (list.timer != null)
                {
                    list.timer.Dispose();
                    list.timer = new Timer(TimerCallback, key, 500, 25);
                }
            }
            else
            {
                KeyModel model = new KeyModel() { data = key, timer = new Timer(TimerCallback, key, 500, 25) };
                key_list.Add(model);
            }
            keybd_event(key, 0, 0, 0);
        }
        public void KeyUp(byte key)
        {
            keybd_event((byte)key, 0, 0x2, 0);
            for (int i = 0; i < key_list.Count; i++)
            {
                if (key_list[i].timer != null)
                {
                    key_list[i].timer.Dispose();
                }
            }
        }
        public void TimerCallback(object state)
        {
            keybd_event((byte)state, 0, 0, 0);
        }
    }
    public class KeyModel
    {
        public byte data { get; set; }
        public Timer timer { get; set; }
    }
}
