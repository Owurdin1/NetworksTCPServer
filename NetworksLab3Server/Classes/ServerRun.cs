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
        private const int MAX_MSG_SIZE = 256;
        private const int LENGTH_BITS = 2;
        private const int MAX_MESSAGES = 100;

        // class private global variables
        private Socket sock = null;
        private int socketNumber;
        private Thread acceptThread;
        private string localServerIP = string.Empty;
        private Object receiveLock = new Object();
        private Object sendLock = new Object();
        private Object messageLock = new Object();
        private System.Windows.Forms.RichTextBox formTextBox = null;
        private bool keepReading = true;

        #region ClassProperties
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
        public bool KeepReading
        {
            get { return keepReading; }
            set { keepReading = value; }
        }
        #endregion

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
            keepReading = true;
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
            Socket socket = null;

            do
            {
                // Accepts a connection on socket
                try
                {
                    socket = sock.Accept();
                }
                catch (Exception)
                {

                }

                SocketState sockState = new SocketState();
                sockState.sock = socket;
                sockState.stpWatch.Start();

                // Spawns thread and starts ConnectionHandler function
                sockState.thread = new Thread(delegate()
                    {
                        ConnectionHandler(sockState);
                    });
                sockState.thread.IsBackground = true;
                sockState.thread.Start();
                //sockState.thread.Start(sockState);
            }
            while (socket.Connected);
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
            keepReading = false;

            string results = String.Empty;

            Thread.Sleep(2000);
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
        public void ConnectionHandler(SocketState sockState)
        {
            // Incoming buffer byte array
            byte[] buffer = new byte[BUFFER_SIZE];

            // Message length byte array, stripped out of buffer
            byte[] byteSize = new byte[LENGTH_BITS];

            // bytes received last read
            int bytesRead = 0;

            int messageCount = 0;

            while (messageCount < MAX_MESSAGES)
            {
                // Current message receive
                byte[] messageBuffer = null;
                int offSet = 0;
                int size = 0;

                lock (receiveLock)
                {
                    bytesRead = sockState.sock.Receive(buffer, offSet, LENGTH_BITS, SocketFlags.None);
                }

                // Get the size values out of current message
                Array.Copy(buffer, offSet, byteSize, 0, LENGTH_BITS);

                // Reverse the bits if they aren't in proper order for proc
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(byteSize);
                }

                // Set the size variable
                size = BitConverter.ToInt16(byteSize, 0);

                // Set offSet variable
                offSet += LENGTH_BITS;

                lock (messageLock)
                {
                    // Read next message out of buffer
                    bytesRead = sockState.sock.Receive(buffer, offSet, size, SocketFlags.None);
                }

                // Set messageBuffer to new byte[] with size index
                messageBuffer = new byte[size];

                // Copy message to messageBuffer
                Array.Copy(buffer, offSet, messageBuffer, 0, size);

                // Send message off, exit do while
                Thread senderThread = new Thread(delegate()
                    {
                        SendFunction(messageBuffer, sockState);
                    });
                senderThread.Start();

                // Increment the message count
                messageCount++;

                # region NonMessageReceive
                //lock (receiveLock)
                //{
                    //// get whole message from client before moving on
                    //do
                    //{
                    //    bytesRead = sockState.sock.Receive(buffer);

                    //    // if haven't received msg length, skip
                    //    if (bytesRead != 0)
                    //    {
                    //        Array.Copy(buffer, byteSize, 2);
                            
                    //        // Swap to proper endian for machine
                    //            // Don't foget to optimize this back to try to save time!
                    //        if (BitConverter.IsLittleEndian)
                    //        {
                    //            Array.Reverse(byteSize);
                    //        }
                    //        short convertedValue = BitConverter.ToInt16(byteSize, 0);
                    //        //IPAddress.NetworkToHostOrder(convertedValue);

                    //        // Set msgSize to actual message size received so far
                    //        msgSize = BitConverter.ToInt16(byteSize, 0);
                    //    }
                    //}
                    //while (msgSize < buffer.Length);

                    //// Truncate message so not to send too much data back.
                    //choppedBuffer = new byte[bytesRead];
                    //Array.Copy(buffer, choppedBuffer, bytesRead);
                    
                    //// Reset counter values for next thread
                    //bytesRead = 0;
                    //msgSize = 0;
                //}

                //// spawn a new sender thread to handle sending messages
                //Thread senderThread = new Thread(delegate()
                //{
                //    SendFunction(choppedBuffer, sockState);
                //});
                //senderThread.Start();
                #endregion

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
            byte[] processedBuffer = new byte[MAX_MSG_SIZE];

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
