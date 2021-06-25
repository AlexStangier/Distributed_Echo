using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Distributed_Echo.PDU;

namespace Distributed_Echo.Threads
{
    public class KnotThread
    {
        public int Port;
        public string Address;
        public Knot.Knot[] Neighbours;
        private short _neighsInformed = 0;
        private String _upwardKnotIPv4;
        private int _upwardKnotPort;
        private UdpClient _socket;
        private bool _initiator = false;
        private int _result = 0;
        private bool firstInformed = false;

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

            //Thread.Sleep(new Random().Next(0, 100));

            switch (knotMessage.Method)
            {
                case SendPdu.Method.START:
                    _initiator = true;
                    _neighsInformed--;
                    InformNeighs();
                    break;
                case SendPdu.Method.INFO:
                    if (!firstInformed)
                    {
                        firstInformed = true;
                        _upwardKnotPort = source?.Port ?? 0;
                        _upwardKnotIPv4 = source?.Address.ToString() ?? "";
                    }
                    InformNeighs();
                    break;
                case SendPdu.Method.ECHO:
                    _result += int.Parse(knotMessage.message);
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
                        _neighsInformed++;
                        if (!neigh.Informed)
                        {
                            neigh.Informed = true;

                            if (neigh.Port != Port && neigh.Port != _upwardKnotPort)
                            {
                                SendToLog($"Relaying {SendPdu.Method.INFO} to {neigh.Port}.");
                                SendToTarget(SendPdu.Method.INFO, neigh.Port, "Relayed INFO message.");
                            }
                        }

                        if (_neighsInformed == Neighbours.Where(x => x != null).ToArray().Length)
                        {
                            if (_initiator)
                            {
                                SendToLog($"ECHO terminated with value: {_result}");
                            }
                            else
                            {
                                SendToTarget(SendPdu.Method.ECHO, _upwardKnotPort, $"{_result}");
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

        /**
         * Send a specified message to a specified target.
         */
        private void SendToTarget(SendPdu.Method method, int port, string message)
        {
            var info = new SendPdu.KnotMessage {Method = method, message = message};
            var infoArr = new SendPdu().getBytes(info);
            _socket.Send(infoArr, infoArr.Length, new IPEndPoint(IPAddress.Parse(Address), port));
        }

        /**
         * Send a specified message to the Logger instance.
         */
        private void SendToLog(string message)
        {
            var info = new SendPdu.KnotMessage {Method = SendPdu.Method.LOG, message = message};
            var infoArr = new SendPdu().getBytes(info);
            _socket.Send(infoArr, infoArr.Length, new IPEndPoint(IPAddress.Parse(Address), 55555));
        }
    }
}