using System;
using System.ComponentModel.Design.Serialization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Distributed_Echo.PDU;

namespace Distributed_Echo.Threads
{
    public class KnotThread
    {
        public int _port;
        public String address;
        private Boolean hasReceived = false;
        public Knot.Knot[] neighbours;

        public KnotThread(int port, string address, Knot.Knot[] _neighbours)
        {
            _port = port;
            this.address = address;
            neighbours = _neighbours;

        }

        private void OnUdpData(IAsyncResult result)
        {
            UdpClient socket = result.AsyncState as UdpClient;
            IPEndPoint source = new IPEndPoint(0, 0);
            var message = socket.EndReceive(result, ref source);
            Console.WriteLine($"KNOT: Got {Encoding.UTF8.GetString(message)} bytes from  {source}");
            socket.BeginReceive(OnUdpData, socket);
            hasReceived = true;
        }

        public void ThreadProc()
        {
            var run = true;

            try
            {
                UdpClient socket = new UdpClient(_port);
                IPEndPoint target = new IPEndPoint(IPAddress.Parse(address), 55555);

                byte[] message;
                while (true)
                {
                    socket.BeginReceive(OnUdpData, socket);
                    if (hasReceived)
                    {
                        message = Encoding.ASCII.GetBytes("OK");
                        socket.Send(message, message.Length, target);

                        foreach (var neigh in neighbours)
                        {
                            if (neigh is not null)
                            {
                                IPEndPoint currTarget = new IPEndPoint(IPAddress.Parse(address), neigh._port);
                                message = Encoding.ASCII.GetBytes("START");
                                socket.Send(message, message.Length, currTarget);
                            }
                        }

                        hasReceived = false;
                    }
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine($"Failed by Socket: {se}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed due to an Exception: {e}");
            }
        }
    }
}