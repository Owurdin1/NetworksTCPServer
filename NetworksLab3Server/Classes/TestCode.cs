using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;


namespace NetworksLab3Server.Classes
{
    class ThreadedServer 
    { 
        private Socket _serverSocket; 
        private int _port; 
        private Thread _acceptThread; 
        private List<ConnectionInfo> _connections = new List<ConnectionInfo>(); 

        public ThreadedServer(int port) 
        { 
            _port = port; 
        } 
        
        private class ConnectionInfo 
        { 
            public Socket Socket; 
            public Thread Thread; 
        } 
        

        public void Start() 
        { 
            SetupServerSocket(); 
            _acceptThread = new Thread(AcceptConnections); 
            _acceptThread.IsBackground = true; 
            _acceptThread.Start(); 
        } 
        
        private void SetupServerSocket() 
        { 
            // Resolving local machine information 
            IPHostEntry localMachineInfo = Dns.GetHostEntry(Dns.GetHostName()); 
            IPEndPoint myEndpoint = new IPEndPoint( localMachineInfo.AddressList[0], _port); 
            
            // Create the socket, bind it, and start listening 
            _serverSocket = new Socket(myEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp); 
            _serverSocket.Bind(myEndpoint); 
            _serverSocket.Listen((int)SocketOptionName.MaxConnections); 
        } 
        
        private void AcceptConnections() 
        { 
            while (true) 
            { 
                // Accept a connection 
                Socket socket = _serverSocket.Accept(); 
                ConnectionInfo connection = new ConnectionInfo(); 
                connection.Socket = socket; 
                
                // Create the thread for the receives. 
                connection.Thread = new Thread(ProcessConnection); 
                connection.Thread.IsBackground = true; 
                connection.Thread.Start(connection); 
                
                // Store the socket 
                lock (_connections) _connections.Add(connection); 
            } 
        } 
        
        private void ProcessConnection(object state) 
        { 
            ConnectionInfo connection = (ConnectionInfo)state; 
            byte[] buffer = new byte[255]; 
            try 
            { 
                while (true) 
                { 
                    int bytesRead = connection.Socket.Receive(buffer); 
                    if (bytesRead > 0) 
                    { 
                        lock (_connections) 
                        { 
                            // this is just the echo server code....
                            foreach (ConnectionInfo conn in _connections) 
                            { 
                                if (conn != connection) 
                                {
                                    conn.Socket.Send( buffer, bytesRead, SocketFlags.None); 
                                } 
                            } 
                        } 
                    } 
                    else if (bytesRead == 0) return; 
                } 
            } 
            catch (SocketException exc) 
            { 
                Console.WriteLine("Socket exception: " + exc.SocketErrorCode);
            } 
            catch (Exception exc) 
            { 
                Console.WriteLine("Exception: " + exc); 
            } 
            finally 
            { 
                connection.Socket.Close(); 
                lock (_connections) _connections.Remove(connection); 
            } 
        } 
    }
}
