using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Distributed_Echo.Threads
{
    public class LoggerThread
    {
        public int _port;
        public String address;
        public int targetPort;

        public LoggerThread(int port, string address, int targetPort)
        {
            _port = port;
            this.address = address;
            this.targetPort = targetPort;
        }

        private void OnUdpData(IAsyncResult result)
        {
            UdpClient socket = result.AsyncState as UdpClient;
            IPEndPoint source = new IPEndPoint(0, 0);
            var message = socket.EndReceive(result, ref source);
            Console.WriteLine($"LOGGER: Got {Encoding.UTF8.GetString(message)} bytes from  {source}");
            socket.BeginReceive(OnUdpData, socket);
        }

        public void ThreadProc()
        {
            var run = true;

            try
            {
                UdpClient socket = new UdpClient(_port);
                IPEndPoint target = new IPEndPoint(IPAddress.Parse(address), targetPort);

                byte[] message = Encoding.ASCII.GetBytes("START SIG");
                socket.Send(message, message.Length, target);

                while (true)
                {
                    socket.BeginReceive(OnUdpData, socket);
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