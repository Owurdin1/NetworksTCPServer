using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace NetworksLab3Server.Classes
{
    class ServerRun
    {
        // class global Constant variables
        private const int WIRELESS_NIC_INDEX = 4; //3; //1; //2;
        private const int PORT = 2605;
        private const int BUFFER_SIZE = 256;
        private const int LENGTH_BITS = 2;

        // class private global variables
        private Socket sock = null;
        private int socketNumber;
        private Thread acceptThread;
        private string localServerIP = string.Empty;
        private Object receiveLock = new Object();
        private Object sendLock = new Object();
        private System.Windows.Forms.RichTextBox formTextBox = null;

        // class properties
        public Socket Sock
        {
            get { return sock; }
            set { sock = value; }
        }
        public int SocketNumber
        {
            get { return socketNumber; }
            set { socketNumber = value; }
        }

        /// <summary>
        /// Non-Default constructor. 
        /// </summary>
        /// <param name="textBox">
        /// Takes a RichTextBox to allow for printing
        /// to the form during real-time
        /// </param>
        public ServerRun(System.Windows.Forms.RichTextBox textBox)
        {
            formTextBox = textBox;
        }
        
        /// <summary>
        /// Start running the server. This will create
        /// a TCP listening server and spawn threads off
        /// to respond to incoming messages.
        /// </summary>
        public string Start()
        //public void Start()
        {
            // start socket and begin listening
            string results = SetSock();
            acceptThread = new Thread(AcceptConnections);
            acceptThread.IsBackground = true;
            acceptThread.Start();

            #region getIPandPrint
            //foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()) 
            //{ 
            //    //Console.WriteLine(nic.Name); 
            //    results += nic.Name;
                
            //    foreach (UnicastIPAddressInformation addrInfo in nic.GetIPProperties().UnicastAddresses)
            //        { 
            //            //Console.WriteLine("\t" + addrInfo.Address); 
            //            results += "\t" + addrInfo.Address;
            //        } 
            //}
            #endregion

            // return value for testing purposes
            return results;
        }

        /// <summary>
        /// Accepts incoming connections and sets
        /// the socket to listen. Spawns a new thread and
        /// sends that to the connection handler function.
        /// </summary>
        private void AcceptConnections()
        {
            while (true)
            {
                // Accepts a connection on socket
                Socket socket = sock.Accept();
                SocketState sockState = new SocketState();
                sockState.sock = socket;
                sockState.stpWatch.Start();

                // Spawns thread and starts ConnectionHandler function
                sockState.thread = new Thread(ConnectionHandler);
                sockState.thread.IsBackground = true;
                sockState.thread.Start(sockState);
            }
        }

        /// <summary>
        /// Sets up the communication socket to begin
        /// running the server.
        /// </summary>
        /// <returns>
        /// returns a string value confirming
        /// that a socket is set up and listening.
        /// This can be changed and removed later.
        /// </returns>
        private string SetSock()
        {
            // Test string for passing values
            string results = String.Empty;

            #region ipAlternate
            //NetworkInterface[] nicArray = NetworkInterface.GetAllNetworkInterfaces();
            //NetworkInterface nic = nicArray[WIRELESS_NIC_INDEX];
            //UnicastIPAddressInformationCollection nicAddrCollection = nic.GetIPProperties().UnicastAddresses;
            //UnicastIPAddressInformation nicAddr = nicAddrCollection[WIRELESS_NIC_INDEX];
            //results = nicAddr.Address.ToString();
#endregion

            // Get local machine information
            IPHostEntry localIP = Dns.GetHostEntry(Dns.GetHostName());
            IPEndPoint localEndPoint = new IPEndPoint(localIP.AddressList[WIRELESS_NIC_INDEX], PORT);
            localServerIP = localEndPoint.ToString().Split(':')[0];

            // set up socket
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(localEndPoint);
            sock.Listen((int)SocketOptionName.MaxConnections);

            // test code to be removed
            results = "Have created a socket and it is listening! serverIP = " + localServerIP;
            socketNumber = (int)sock.Handle;

            return results;
        }

        /// <summary>
        /// Uses this to shut down the server
        /// </summary>
        /// <returns>
        /// Returns a string value to confirm shutdown has happened.
        /// </returns>
        public string CloseServer()
        {
            string results = String.Empty;

            sock.Close(socketNumber);

            results = "Socket number: " + socketNumber.ToString() + " has been closed!";
            return results;
        }

        /// <summary>
        /// Handles the connection input for recieving data
        /// from the client.
        /// </summary>
        /// <param name="value">
        /// Takes a SocketState object that has a socket and thread 
        /// assigned to it.
        /// </param>
        public void ConnectionHandler(object value)
        {
            SocketState sockState = (SocketState)value;
            byte[] buffer = new byte[BUFFER_SIZE];
            byte[] byteSize = new byte[LENGTH_BITS];
            byte[] choppedBuffer = null;
            int bytesRead = 0;
            short msgSize = 0;

            while (true)
            {
                lock (receiveLock)
                {
                    // get whole message from client before moving on
                    do
                    {
                        bytesRead = sockState.sock.Receive(buffer);

                        // if haven't received msg length, skip
                        if (bytesRead > 2)
                        {
                            Array.Copy(buffer, byteSize, 2);
                            
                            // Swap to proper endian for machine
                                // Don't foget to optimize this back to try to save time!
                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(byteSize);
                            }
                            short convertedValue = BitConverter.ToInt16(byteSize, 0);
                            //IPAddress.NetworkToHostOrder(convertedValue);

                            // Set msgSize to actual message size received so far
                            msgSize = BitConverter.ToInt16(byteSize, 0);
                        }
                    }
                    while (msgSize < bytesRead);

                    // Truncate message so not to send too much data back.
                    choppedBuffer = new byte[bytesRead];
                    Array.Copy(buffer, choppedBuffer, bytesRead);
                    
                    // Reset counter values for next thread
                    bytesRead = 0;
                    msgSize = 0;
                }

                // spawn a new sender thread to handle sending messages
                Thread senderThread = new Thread(delegate()
                {
                    SendFunction(choppedBuffer, sockState);
                });
                senderThread.Start();

            }
        }

        /// <summary>
        /// processes the received message and carries out logic
        /// to resend the response back to the client
        /// </summary>
        /// <param name="choppedBuffer">
        /// The received buffer chopped down to
        /// the size of actual bytes received instead of full
        /// 256 size array
        /// </param>
        /// <param name="sockState">
        /// SocketState class object containing the
        /// values that are passed around for the message
        /// </param>
        private void SendFunction(byte[] choppedBuffer, SocketState sockState)
        {
            byte[] processedBuffer = new byte[BUFFER_SIZE];

            // Process the incoming message and prepares it for response
            sockState.countNumber++;
            ResponseBuilder rb = new ResponseBuilder(System.Text.Encoding.ASCII.GetString(choppedBuffer));
            processedBuffer = rb.Response(sockState.countNumber,
                sockState.stpWatch.ElapsedMilliseconds.ToString(),
                sockState.sock.RemoteEndPoint.AddressFamily.ToString(),
                sockState.sock.Handle.ToString(),
                localServerIP);

            lock (sendLock)
            {
                // Send message back to the client
                sockState.sock.Send(processedBuffer);
            }
        }
    }
}
