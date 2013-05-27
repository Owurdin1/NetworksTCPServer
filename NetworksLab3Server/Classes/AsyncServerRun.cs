using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace NetworksLab3Server.Classes
{
    class AsyncServerRun
    {
        // class global Constant variables
        private const int WIRELESS_NIC_INDEX = 3; //1; //2;
        private const int PORT = 2605;
        private const int BUFFER_SIZE = 256;
        private const int MAX_MSG_SIZE = 256;
        private const int LENGTH_BITS = 2;

        // class private global variables
        private Socket sock = null;
        //private int socketNumber;
        private int responseNumber = 0;
        //private Thread acceptThread;
        private string localServerIP = string.Empty;
        private System.Windows.Forms.RichTextBox formTextBox = null;
        //private bool keepReading = true;
        private List<SocketState> SocketsList = new List<SocketState>();

        
        /// <summary>
        /// Non-Default constructor. 
        /// </summary>
        /// <param name="textBox">
        /// Takes a RichTextBox to allow for printing
        /// to the form during real-time
        /// </param>
        public AsyncServerRun(System.Windows.Forms.RichTextBox textBox)
        {
            formTextBox = textBox;
        }

        /// <summary>
        /// Set up listening for the Async call back
        /// </summary>
        public void Start()
        {
            SetSock();

            for (int i = 0; i < 10; i++)
            {
                sock.BeginAccept(new AsyncCallback(AcceptCallBack), sock);
            }
        }

        /// <summary>
        /// Sets up the socket to listen on
        /// </summary>
        private void SetSock()
        {
            // Get local machine information
            IPHostEntry localIP = Dns.GetHostEntry(Dns.GetHostName());
            IPEndPoint localEndPoint = 
                new IPEndPoint(localIP.AddressList[WIRELESS_NIC_INDEX], PORT);
            localServerIP = localEndPoint.ToString().Split(':')[0];

            // set up socket
            sock = new Socket(AddressFamily.InterNetwork, 
                SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(localEndPoint);
            sock.Listen((int)SocketOptionName.MaxConnections);
        }

        /// <summary>
        /// Accepts the call back for asynch function 
        /// </summary>
        /// <param name="result"></param>
        private void AcceptCallBack(IAsyncResult result)
        {
            SocketState sockState = new SocketState();

            try
            {
                Socket s = (Socket)result.AsyncState;
                sockState.sock = s.EndAccept(result);
                sockState.buffer = new byte[BUFFER_SIZE];
                lock (SocketsList)
                {
                    SocketsList.Add(sockState);
                }
                
                sockState.sock.BeginReceive(sockState.buffer, 0, 
                    sockState.buffer.Length, SocketFlags.None, 
                    new AsyncCallback(ReceiveCallBack), sockState);

                sock.BeginAccept(new AsyncCallback(AcceptCallBack), 
                    result.AsyncState);

            }
            catch (Exception e)
            {
                //KillConnection(conn);
                formTextBox.Text = "Killed a socket in call back " + e.Message;
            }
        }

        /// <summary>
        /// The ReceiveCallBack function
        /// decides which piece the sockState needs to run.
        /// Either it needs to get message size
        /// or it needs to get the message
        /// </summary>
        /// <param name="result">
        /// IAsyncResult object cast as a SocketState.
        /// </param>
        private void ReceiveCallBack(IAsyncResult result)
        {
            SocketState sockState = (SocketState)result.AsyncState;
            //==================================================================================

            //int bytesRead = sockState.sock.EndReceive(result);
            //byte[] byteSize = new byte[LENGTH_BITS];

            //// Get the size values out of current message
            //Array.Copy(sockState.buffer, sockState.offset, byteSize, 0, LENGTH_BITS);

            //// Reverse the bits if they aren't in proper order for proc
            //if (BitConverter.IsLittleEndian)
            //{
            //    Array.Reverse(byteSize);
            //}

            //// Set the size variable
            //sockState.size = BitConverter.ToInt16(byteSize, 0);

            //// Set the offset to begin read after size bytes
            //sockState.offset = LENGTH_BITS;

            //// send back to call back to receive the message
            //sockState.sock.BeginReceive(sockState.buffer, sockState.offset, sockState.size,
            //    SocketFlags.None, new AsyncCallback(ReceiveCallBack), result.AsyncState);

            //==================================================================================
            //try
            //{
            //    switch (sockState.size)
            //    {
            //        case 0:
            //            sockState.sock.BeginReceive(sockState.buffer, sockState.offset,
            //                LENGTH_BITS, SocketFlags.None, new AsyncCallback(ReadSize), result.AsyncState);
            //            break;

            //        default:
            //            sockState.sock.BeginReceive(sockState.buffer, sockState.offset,
            //                sockState.size, SocketFlags.None, new AsyncCallback(ReadMessage), result.AsyncState);
            //            break;
            //    }
            //}
            //catch (Exception e)
            //{
            //    //KillConnection(sockState);
            //    formTextBox.Text = "ReceiveCallBack crash! " + e.Message;
            //}
        }

        /// <summary>
        /// Converts the first 2 bits read into
        /// short and sets the message size to be
        /// pulled off of the socket. Otherwise
        /// the full size bits haven't been read so it will
        /// send back and read until full size has been
        /// read off of the socket. (Shouldn't have to 
        /// handle that with the type of read that is being done,
        /// but easier than tracking down error).
        /// </summary>
        /// <param name="result">
        /// IAsyncResult cast as a SocketState
        /// </param>
        private void ReadSize(IAsyncResult result)
        {
            //SocketState sockState = (SocketState)result;
            SocketState sockState = (SocketState)result.AsyncState;

            int bytesRead = sockState.sock.EndReceive(result);

            //switch (bytesRead)
            //{
            //    case 0:
            //    {
            //        // full 2 bits length hasn't been received yet
            //        sockState.sock.BeginReceive(sockState.buffer, sockState.offset,
            //            sockState.size, SocketFlags.None, new AsyncCallback(ReceiveCallBack), result.AsyncState);
            //        break;
            //    }
            //    case 1:
            //    {
            //        // 1 bit has been read off the buffer, need to get the other one
            //        sockState.offset++;
            //        sockState.sock.BeginReceive(sockState.buffer, sockState.offset,
            //            sockState.size, SocketFlags.None, new AsyncCallback(ReceiveCallBack), result.AsyncState);
            //        break;
            //    }
            //    case 2:
            //    {
                    // Caclulate the size of the message coming in
                    // Varaibles for size calculation
                    byte[] byteSize = new byte[LENGTH_BITS];

                    // Get the size values out of current message
                    Array.Copy(sockState.buffer, sockState.offset, byteSize, 0, LENGTH_BITS);

                    // Reverse the bits if they aren't in proper order for proc
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(byteSize);
                    }

                    // Set the size variable
                    sockState.size = BitConverter.ToInt16(byteSize, 0);

                    // Set the offset to begin read after size bytes
                    sockState.offset = LENGTH_BITS;

                    // send back to call back to receive the message
                    sockState.sock.BeginReceive(sockState.buffer, sockState.offset, sockState.size,
                        SocketFlags.None, new AsyncCallback(ReceiveCallBack), result.AsyncState);
                //    break;
                //}
            //}
        }

        /// <summary>
        /// Reads the message from the socket.
        /// If the full message hasn't been receieved it
        /// will send back to the ReceiveCallBack function
        /// and wait until the whole message has been yanked out of the
        /// socket. If whole message hasn't been recieved the SocketState
        /// offset value will be updated to the latest position in the
        /// sockState object.
        /// </summary>
        /// <param name="result"></param>
        private void ReadMessage(IAsyncResult result)
        {
            //SocketState sockState = (SocketState)result;
            SocketState sockState = (SocketState)result.AsyncState;

            int bytesRead = sockState.sock.EndReceive(result);

            if (bytesRead == sockState.size)
            {
                responseNumber++;
                sockState.countNumber = responseNumber;
                SendReply(sockState);
            }
            else
            {
                // whole message not received yet
                sockState.sock.BeginReceive(sockState.buffer, sockState.offset,
                    sockState.size, SocketFlags.None, new AsyncCallback(ReceiveCallBack), result.AsyncState);
            }
        }

        /// <summary>
        /// build resposne and send it back call the send 
        /// call back function to complete the send.
        /// </summary>
        /// <param name="sockState"></param>
        private void SendReply(SocketState sockState)
        {
            // Must build response then send the message through to the client.
            byte[] choppedBuffer = new byte[sockState.size];
            //byte[] processedBuffer;
            Array.Copy(sockState.buffer, sockState.offset, choppedBuffer, 0, sockState.size);

            ResponseBuilder rb = new ResponseBuilder(Encoding.ASCII.GetString(choppedBuffer));
            sockState.processedBuffer = rb.ResponseAsync(sockState, localServerIP);

            sockState.processedSize = sockState.processedBuffer.Length;

            sockState.sock.BeginSend(sockState.processedBuffer, 0, sockState.processedSize, 
                SocketFlags.None, new AsyncCallback(SendCallBack), sockState);
        }

        /// <summary>
        /// Callback function to send messages
        /// </summary>
        /// <param name="result">
        /// SocketState object
        /// </param>
        private void SendCallBack(IAsyncResult result)
        {
            //SocketState sockState = (SocketState)result;
            SocketState sockState = (SocketState)result.AsyncState;
        }

        /// <summary>
        /// Clears the connection to the current SocketState
        /// object from the SocketsList list
        /// </summary>
        /// <param name="sockState">
        /// SocketState object to remove from the Sockets list
        /// </param>
        private void KillConnection(SocketState sockState)
        {
            sockState.sock.Close();
            lock (SocketsList)
            {
                SocketsList.Remove(sockState);
            }
        }
    }
}
