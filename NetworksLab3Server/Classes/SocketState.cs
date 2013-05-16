using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace NetworksLab3Server.Classes
{
    class SocketState
    {
        public Socket sock = null;
        public Thread thread = null;
    }
}
