using System;
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
        private SendPdu pdu = new SendPdu();

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

            var knotMessage = new SendPdu().fromBytes(message);


            Console.WriteLine(
                $"KNOT {address}:{_port}: Got Method {knotMessage.Method} and message {knotMessage.message} from  {source}");
            socket.BeginReceive(OnUdpData, socket);
            hasReceived = true;
        }

        public void ThreadProc()
        {
            try
            {
                UdpClient socket = new UdpClient(_port);
                IPEndPoint target = new IPEndPoint(IPAddress.Parse(address), 55555);
                
                while (true)
                {
                    socket.BeginReceive(OnUdpData, socket);
                    if (hasReceived)
                    {
                        var toSend = new SendPdu.KnotMessage();
                        toSend.Method = SendPdu.Method.INFO;
                        toSend.message = 1337;

                        var c = new SendPdu().getBytes(toSend);

                        //send message to logger
                        socket.Send(c, c.Length, target);

                        foreach (var neigh in neighbours)
                        {
                            if (neigh is not null)
                            {
                                IPEndPoint currTarget = new IPEndPoint(IPAddress.Parse(address), neigh._port);
                                //send message to neigbours
                                if (neigh._port != _port)
                                    socket.Send(c, c.Length, currTarget);
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