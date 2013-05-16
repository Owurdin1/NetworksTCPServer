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
        private const int WIRELESS_NIC_INDEX = 2;
        private const int PORT = 2605;
        //private const 192.168.1.35;

        // class private global variables
        private Socket sock = null;
        private int socketNumber;

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

        private string SetSock()
        {
            // Test string for passing values
            string results = String.Empty;

            //NetworkInterface[] nicArray = NetworkInterface.GetAllNetworkInterfaces();
            //NetworkInterface nic = nicArray[WIRELESS_NIC_INDEX];
            //UnicastIPAddressInformationCollection nicAddrCollection = nic.GetIPProperties().UnicastAddresses;
            //UnicastIPAddressInformation nicAddr = nicAddrCollection[WIRELESS_NIC_INDEX];
            //results = nicAddr.Address.ToString();

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

        public string CloseServer()
        {
            string results = String.Empty;

            sock.Close(socketNumber);

            results = "Socket number: " + socketNumber.ToString() + " has been closed!";
            return results;
        }
    }
}
