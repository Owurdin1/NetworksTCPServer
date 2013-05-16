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
                System.Windows.Forms.MessageBox.Show("ERROR: Message sent from client is invalid");
            }
        }

        public byte[] Response(int count)
        {
            byte[] msgByte = null;

            msgArray[0] = "RSP";
            msgArray[1] = "msTimeStamp"; // TODO Generate timestamp for server
            msgArray[5] = "foreignHostIP"; // TODO Get ForeignHostIP From socket
            msgArray[7] = "serverSocketNumber"; // TODO Get socket number from server
            msgArray[8] = "serverIPAddress"; // TODO Get server IP Address
            msgArray[10] = "WurdingerO" + count.ToString(); // TODO get a count number for number of messages

            string message = String.Empty;
            foreach (string s in msgArray)
            {
                if (!s.Equals(""))
                {
                    message += s + "|";
                }
            }

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
