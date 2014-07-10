using PCRSever.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCRSever.Control
{
    public class InputHelper
    {
        PCRSocket mPCRSocket = null;
        public IPCRSocket _mainModel;
        public static InputHelper Singleton()
        {
            if (mInputHelper == null)
            {
                mInputHelper = new InputHelper();
            }
            return mInputHelper;
        }
        private static InputHelper mInputHelper = null;
        private InputHelper()
        {

        }
        public void setIPCRSocket(IPCRSocket mainModel)
        {
            _mainModel = mainModel;
        }
        public void setPCRSocket(PCRSocket socket)
        {
            mPCRSocket = socket;
        }

        public void setInputTest(string text)
        {
            string temp1 = text.Replace("\r", "");
            string temp2 = temp1.Replace("\n", @"\");
            string[] arrayS = temp2.Split('\\');
            string Last = arrayS.Last<string>();
            setSendMsg(Last);
        }
        private void setSendMsg(string msg)
        {
            if (mPCRSocket != null)
            {
                string[] arrayS = msg.Split(' ');
                if (arrayS[0] == "color")
                {
                    if (arrayS.Length >3)
                    {
                        mPCRSocket.sendSetColorMsg(arrayS[1] + " " + arrayS[2] + " " + arrayS[3]);
                        return;
                    }
                    else if (arrayS.Length == 2)
                    {
                        if (arrayS[1] == "red")
                        { mPCRSocket.sendSetColorMsg("255 0 0"); return; }
                        else if (arrayS[1] == "white")
                        { mPCRSocket.sendSetColorMsg("255 255 255"); return; }
                        else if (arrayS[1] == "black")
                        { mPCRSocket.sendSetColorMsg("0 0 0"); return; }
                        else if (arrayS[1] == "yellow")
                        { mPCRSocket.sendSetColorMsg("255 255 0"); return; }
                        else if (arrayS[1] == "blue")
                        { mPCRSocket.sendSetColorMsg("0 0 255"); return; }
                        else if (arrayS[1] == "pink")
                        { mPCRSocket.sendSetColorMsg("255 0 255"); return; }
                    }
                }
                _mainModel.SocketMessage("输入参数有误!");
            }
            else
            {
                _mainModel.SocketMessage("还没有可以控制的客户端");
            }
        }
    }
}
