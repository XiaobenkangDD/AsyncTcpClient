﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace LTKJ.RTU
{
   
    public class AsynClient
    {
        // The port number for the remote device.     
       
        // ManualResetEvent instances signal completion.     
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        // The response from the remote device.     
        private static String response = String.Empty;
        public string ip { get; set; }
        private int port { get; set; }
        public StateObject state { get; set; }

        public  void StartClient()
        {
            // Connect to a remote device.     
            try
            {
               
                IPAddress ipAddress = IPAddress.Parse(this.ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, this.port);
                 
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
              
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
         //       connectDone.WaitOne();
                   
          //      Send(client, "This is a test<EOF>");
           //     sendDone.WaitOne();
                // Receive the response from the remote device.     
            //    Receive(client);
          //      receiveDone.WaitOne();
                // Write the response to the console.            
                // Release the socket.     
           //     client.Shutdown(SocketShutdown.Both);
            //    client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private  void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.     
                Socket client = (Socket)ar.AsyncState;
                // Complete the connection.     
                client.EndConnect(ar);
              //  Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());
                // Signal that the connection has been made.     
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private  void Receive(Socket client)
        {
            try
            {
                // Create the state object.     
                StateObject state = new StateObject();
                state.workSocket = client;
                // Begin receiving the data from the remote device.     
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private  void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket     
                // from the asynchronous state object.     
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                // Read data from the remote device.     
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.     

                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    // Get the rest of the data.     
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.     
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.     
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private  void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.     
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            // Begin sending the data to the remote device.     
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }
        private  void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.     
                Socket client = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.     
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
                // Signal that all bytes have been sent.     
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // State object for receiving data from remote device.     
        public class StateObject
        {
            // Client socket.     
            public Socket workSocket = null;
            // Size of receive buffer.     
            public const int BufferSize = 2048;
            // Receive buffer.     
            public byte[] buffer = new byte[BufferSize];
            // Received data string.     
            public StringBuilder sb = new StringBuilder();
        }

    }
}