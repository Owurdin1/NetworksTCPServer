using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace NetworksLab3Server.Classes
{
    class SocketState
    {
        public Socket sock = null;
        public Thread thread = null;
        public Stopwatch stpWatch;
        public int countNumber = 0;

        public SocketState()
        {
            stpWatch = new Stopwatch();
        }
    }
}
