using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Distributed_Echo.PDU;

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

            var m = new SendPdu().fromBytes(message);

            Console.WriteLine($"LOGGER: {source}: Got {m.Method} and: '{m.message}'");
            socket.BeginReceive(OnUdpData, socket);
        }

        public void ThreadProc()
        {
            try
            {
                var socket = new UdpClient(_port);
                var target = new IPEndPoint(IPAddress.Parse(address), targetPort);

                var knot = new SendPdu.KnotMessage();
                knot.message = "START from Logger";
                knot.Method = SendPdu.Method.START;
                
                var message = new SendPdu().getBytes(knot);

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