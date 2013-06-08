using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworksLab3Server.Classes
{
    class ResponseBuilder
    {
        // class private variables
        private string msg = String.Empty;
        private string[] msgArray = null;

        /// <summary>
        /// non default constructor, 
        /// only use this one to implement
        /// ResponseBuilder class
        /// </summary>
        /// <param name="msg">
        /// message form the client
        /// </param>
        public ResponseBuilder(string msg)
        {
            msgArray = msg.Split('|');

            if (msgArray.Length < 14)
            {
                //System.Windows.Forms.MessageBox.Show("ERROR: Message sent from client is invalid");
            }
        }

        /// <summary>
        /// Builds response message and prepares byte array
        /// </summary>
        /// <param name="count">
        /// integer value that is a unique value to append to messages
        /// </param>
        /// <returns>
        /// formatted byte array
        /// </returns>
        public byte[] Response(int count, string msTimeStamp, 
            string foreignHostIP, string serverSocketNumber, string serverIPAddress)
        {
            byte[] msgByte = null;

            msgArray[0] = "RSP";
            msgArray[1] = msTimeStamp;
            msgArray[6] = foreignHostIP;
            msgArray[7] = "2605";
            msgArray[8] = serverSocketNumber;
            msgArray[9] = serverIPAddress;
            msgArray[11] = "OWServ " + count.ToString();

            string message = String.Empty;
            //foreach (string s in msgArray)
            //{
            //    if (!s.Equals(""))
            //    {
            //        message += s + "|";
            //    }
            //}

            message = string.Join("|", msgArray);

            msgByte = System.Text.Encoding.ASCII.GetBytes(message);

            //msgByte = SetMsgLength(msgByte);
            //return msgByte;
            return SetMsgLength(msgByte);
        }

        /// <summary>
        /// Builds response message and prepares byte array
        /// </summary>
        /// <param name="count">
        /// integer value that is a unique value to append to messages
        /// </param>
        /// <returns>
        /// formatted byte array
        /// </returns>
        public byte[] ResponseAsync(SocketState sockState, string serverIPAddress)
        {
            byte[] msgByte = null;

            msgArray[0] = "RSP";
            msgArray[1] = sockState.stpWatch.ElapsedMilliseconds.ToString();
            msgArray[6] = sockState.sock.RemoteEndPoint.AddressFamily.ToString();
            msgArray[7] = "2605";
            msgArray[8] = sockState.sock.Handle.ToString();
            msgArray[9] = serverIPAddress;
            msgArray[11] = "OWServ " + sockState.countNumber.ToString();

            //msgArray[0] = "RSP";
            //msgArray[1] = msTimeStamp;
            //msgArray[6] = foreignHostIP;
            //msgArray[7] = "2605";
            //msgArray[8] = serverSocketNumber;
            //msgArray[9] = serverIPAddress;
            //msgArray[11] = "OW " + count.ToString();

            string message = String.Empty;
            //foreach (string s in msgArray)
            //{
            //    if (!s.Equals(""))
            //    {
            //        message += s + "|";
            //    }
            //}

            message = string.Join("|", msgArray);

            msgByte = System.Text.Encoding.ASCII.GetBytes(message);

            //msgByte = SetMsgLength(msgByte);
            //return msgByte;
            return SetMsgLength(msgByte);
        }

        /// <summary>
        /// Gets the length of the message and tack that value
        /// in bits onto the front of the byte array
        /// </summary>
        /// <param name="msgByte">
        /// byte array that will be sent back to calling function
        /// </param>
        /// <returns></returns>
        private byte[] SetMsgLength(byte[] msgByte)
        {
            // get length of message
            short msgLength = (short)msgByte.Length;

            // create a byte array fro the size of message
            byte[] bitSize = BitConverter.GetBytes(msgLength);

            // create a byte array so that size of message and message will fit
            byte[] sendMsg = new byte[msgLength + bitSize.Length];

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bitSize);
            }

            // Copy both arrays into new message byte array
            Array.Copy(bitSize, sendMsg, bitSize.Length);
            Array.Copy(msgByte, 0, sendMsg, bitSize.Length, msgLength);

            // return the message array back to calling function
            return sendMsg;
        }
    }
}
