using System;
using System.Net;
using System.Net.Sockets;
using Distributed_Echo.PDU;

namespace Distributed_Echo.Threads
{
    public class KnotThread
    {
        public readonly int Port;
        public readonly String Address;
        private Boolean _hasReceived;
        public readonly Knot.Knot[] Neighbours;
        private short _neighsInformed = 0;
        private bool _informed = false;
        private String _upwardKnotIPv4;
        private int _upwardKnotPort;

        public KnotThread(int port, string address, Knot.Knot[] neighbours)
        {
            Port = port;
            Address = address;
            Neighbours = neighbours;
        }

        private void OnUdpData(IAsyncResult result)
        {
            UdpClient socket = result.AsyncState as UdpClient;
            IPEndPoint source = new IPEndPoint(0, 0);
            var message = socket?.EndReceive(result, ref source);

            var knotMessage = new SendPdu().fromBytes(message);
            
            switch (knotMessage.Method)
            {
                case SendPdu.Method.START:
                    break;
                case SendPdu.Method.INFO:
                    _informed = true;
                    _upwardKnotPort = source?.Port ?? 0;
                    _upwardKnotIPv4 = source?.Address.ToString() ?? "";
                    break;
                case SendPdu.Method.ECHO:
                    break;
            }

            Console.WriteLine(
                $"KNOT {Address}:{Port}: Got Method {knotMessage.Method} and message {knotMessage.message} from  {source}");

            socket?.BeginReceive(OnUdpData, socket);
            _hasReceived = true;
        }

        public void ThreadProc()
        {
            try
            {
                UdpClient socket = new UdpClient(Port);
                IPEndPoint target = new IPEndPoint(IPAddress.Parse(Address), 55555);

                while (true)
                {
                    socket.BeginReceive(OnUdpData, socket);
                    if (_hasReceived)
                    {
                        var loggerMessage = new SendPdu.KnotMessage();
                        loggerMessage.Method = SendPdu.Method.INFO;
                        loggerMessage.message = 1337;

                        //send message to logger
                        var c = new SendPdu().getBytes(loggerMessage);
                        socket.Send(c, c.Length, target);

                        foreach (var neigh in Neighbours)
                        {
                            if (neigh is not null)
                            {
                                if (!_informed)
                                {
                                    IPEndPoint currTarget = new IPEndPoint(IPAddress.Parse(Address), neigh.Port);
                                    //send message to neigbours
                                    if (neigh.Port != Port && neigh.Port != _upwardKnotPort)
                                    {
                                        _neighsInformed++;

                                        //send i to all neighs except upwardKnot
                                        var info = new SendPdu.KnotMessage();
                                        info.Method = SendPdu.Method.INFO;
                                        info.message = 0;
                                        var infoArr = new SendPdu().getBytes(info);
                                        socket.Send(infoArr, infoArr.Length, currTarget);
                                        
                                    }
                                }
                            }
                        }

                        _hasReceived = false;
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