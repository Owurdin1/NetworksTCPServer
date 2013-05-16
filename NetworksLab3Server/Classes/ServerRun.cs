using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
//using System.Net.NetworkInformation;
using System.Net;

namespace NetworksLab3Server.Classes
{
    class ServerRun
    {
        // class global Constant variables
        private const int WIRELESS_NIC_INDEX = 1; //2;
        private const int PORT = 2605;
        private const int BUFFER_SIZE = 256;

        // class private global variables
        private Socket sock = null;
        private int socketNumber;
        //private List<SocketState> sockList = new List<SocketState>();
        private Thread acceptThread;

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
        /// Start running the server. This will create
        /// a TCP listening server and spawn threads off
        /// to respond to incoming messages.
        /// </summary>
        public string Start()
        //public void Start()
        {
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

                // Spawns thread and starts ConnectionHandler function
                sockState.thread = new Thread(ConnectionHandler);
                sockState.thread.IsBackground = true;
                sockState.thread.Start(sockState);
                //lock (sockList) sockList.Add(sockState);
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

            // set up socket
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(localEndPoint);
            sock.Listen((int)SocketOptionName.MaxConnections);

            // test code to be removed
            results = "Have created a socket and it is listening!";
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
            byte[] processedBuffer = new byte[BUFFER_SIZE];

            while (true)
            {
                int bytesRead = sockState.sock.Receive(buffer);

                if (bytesRead > 0)
                {
                    // Process the incoming message and prepares it for response
                    ResponseBuilder rb = new ResponseBuilder(System.Text.Encoding.ASCII.GetString(buffer));
                    processedBuffer = rb.Response((int)sockState.sock.Handle);

                    lock (sockState)
                    {
                        // Send message back to the client
                        sockState.sock.Send(processedBuffer);
                    }
                }
            }
        }
    }
}
