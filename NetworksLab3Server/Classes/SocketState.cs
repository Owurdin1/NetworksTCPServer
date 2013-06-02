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
        // Public values to store for use in threads
        public Socket sock = null;
        public Thread thread = null;
        public Stopwatch stpWatch;
        public int countNumber = 0;
        public byte[] buffer;
        //public byte[] processedBuffer;
        public int offset = 0;
        public int size = 0;
        public int processedSize = 0;
        
        public SocketState()
        {
            stpWatch = new Stopwatch();
        }
    }
}
