using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Distributed_Echo.PDU;

namespace Distributed_Echo.Threads
{
    public class KnotThread
    {
        public readonly int Port;
        public readonly String Address;
        public readonly Knot.Knot[] Neighbours;
        private short _neighsInformed = 0;
        private String _upwardKnotIPv4;
        private int _upwardKnotPort;
        private UdpClient _socket;
        private bool _initiator = false;
        private int _result = 0;

        public KnotThread(int port, string address, Knot.Knot[] neighbours)
        {
            Port = port;
            Address = address;
            Neighbours = neighbours;
            _socket = new UdpClient(port);
        }

        /**
         * Contains handling for incoming messages 
         */
        private void OnUdpData(IAsyncResult result)
        {
            IPEndPoint source = new IPEndPoint(0, 0);
            var message = _socket?.EndReceive(result, ref source);

            var knotMessage = new SendPdu().fromBytes(message);
            SendToLog($"Received: {knotMessage.Method}");

            switch (knotMessage.Method)
            {
                case SendPdu.Method.START:
                    _initiator = true;
                    InformNeighs();
                    break;
                case SendPdu.Method.INFO:
                    _upwardKnotPort = source?.Port ?? 0;
                    _upwardKnotIPv4 = source?.Address.ToString() ?? "";
                    InformNeighs();
                    break;
                case SendPdu.Method.ECHO:
                    _result += Int32.Parse(knotMessage.message);
                    if (_initiator) SendToLog($"Calculated Memory Size: {_result}");
                    break;
            }

            _socket?.BeginReceive(OnUdpData, _socket);
        }

        private void InformNeighs()
        {
            foreach (var neigh in Neighbours)
            {
                try
                {
                    if (neigh is not null)
                    {
                        if (!neigh.Informed)
                        {
                            neigh.Informed = true;
                            //send message to neigbours
                            if (neigh.Port != Port && neigh.Port != _upwardKnotPort)
                            {
                                _neighsInformed++;
                                SendToLog(
                                    $"Relaying {SendPdu.Method.INFO} to {neigh.Port}.");
                                SendToTarget(SendPdu.Method.INFO, neigh.Port, "Relayed INFO message.");
                            }
                        }

                        if (_neighsInformed != Neighbours.Where(x => x != null).ToArray().Length)
                        {
                            if (_initiator)
                            {
                                SendToLog("ECHO terminated.");
                            }
                            else
                            {
                                SendToTarget(SendPdu.Method.ECHO, _upwardKnotPort, "1");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void ThreadProc()
        {
            try
            {
                while (true)
                {
                    _socket.BeginReceive(OnUdpData, _socket);
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

        private void SendToTarget(SendPdu.Method method, int port, String message)
        {
            var info = new SendPdu.KnotMessage {Method = method, message = message};
            var infoArr = new SendPdu().getBytes(info);
            _socket.Send(infoArr, infoArr.Length, new IPEndPoint(IPAddress.Parse(Address), port));
        }

        private void SendToLog(String message)
        {
            var info = new SendPdu.KnotMessage {Method = SendPdu.Method.LOG, message = message};
            var infoArr = new SendPdu().getBytes(info);
            _socket.Send(infoArr, infoArr.Length, new IPEndPoint(IPAddress.Parse(Address), 55555));
        }
    }
}