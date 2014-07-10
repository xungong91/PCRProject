using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRSever.Control
{
    public class SendMsg
    {
        public int length;
        public byte[] data;

        public SendMsg(MsgType _type_)
        {
            data = new byte[3];
            data[0]=Convert.ToByte((int)_type_);
            short len = 0;
            byte[] buffer = new byte[2];
            buffer = BitConverter.GetBytes(len);
            data[1] = buffer[0];
            data[2] = buffer[1];
            length = 3;
        }
    }
}
