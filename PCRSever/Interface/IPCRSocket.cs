using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRSever.Interface
{
    public interface IPCRSocket
    {
        void SocketMessage(string msg);
    }
}
