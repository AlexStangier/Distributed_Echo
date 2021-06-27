using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Distributed_Echo.PDU;
using Microsoft.VisualBasic.CompilerServices;

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
        private bool informed = false;
        private int _memorySize;
        private int _amountNeighbours;

        public KnotThread(int port, string address, Knot.Knot[] neighbours, int memorySize = 0)
        {
            Port = port;
            Address = address;
            Neighbours = neighbours;
            _socket = new UdpClient(port);
            if (memorySize is 0) new Random().Next();
            _memorySize = memorySize;
            _amountNeighbours = Neighbours?.Where(x => x != null).ToArray().Length ?? 0;
        }


        /**
         * Contains handling for incoming messages 
         */
        private void OnUdpData(IAsyncResult result)
        {
            IPEndPoint source = new IPEndPoint(0, 0);
            var message = _socket?.EndReceive(result, ref source);

            var knotMessage = new SendPdu().fromBytes(message);
            SendToLog($"Received: {knotMessage.Method} from {source.Port}");

            Thread.Sleep(new Random().Next(0, 100));

            if (knotMessage.Method == SendPdu.Method.START)
            {
                _initiator = true;
                //_neighsInformed--;
            }

            EchoAlgorithm(knotMessage, source);

            _socket?.BeginReceive(OnUdpData, _socket);
        }

        private void EchoAlgorithm(SendPdu.KnotMessage message, IPEndPoint source)
        {
            _neighsInformed++;
            if (!informed)
            {
                informed = true;
                _upwardKnotPort = source.Port;
                _upwardKnotIPv4 = source.Address.ToString();
                SendToLog(
                    $"Parent: {_upwardKnotPort} IsInitiator:{_initiator.ToString().ToLower()} started Algorithm with memory Size:{_memorySize}");
                SendToLog($"Amount neighbours: {_amountNeighbours}");

                foreach (var neigh in Neighbours)
                {
                    
                    if (neigh.Port != _upwardKnotPort)
                    {
                        SendToTarget(SendPdu.Method.INFO, neigh.Port, $"{_memorySize}");
                        SendToLog(
                            $"Sending INFO to {neigh.Port} -> Neighs informed: {_neighsInformed}/{_amountNeighbours}");
                    }
                }
            }

            if (message.Method == SendPdu.Method.ECHO)
            {
                try
                {
                    _memorySize += int.Parse(message.message);
                    SendToLog($"Reported network memory size: {_memorySize}");
                }
                catch (Exception e)
                {
                    SendToLog($"Failed with: {e.Message}");
                }
            }

            if (_neighsInformed == _amountNeighbours)
            {
                if (_initiator)
                {
                    SendToLog($"ECHO terminated with value: {_memorySize}");
                }
                else
                {
                    SendToTarget(SendPdu.Method.ECHO, _upwardKnotPort, $"{_memorySize}");
                    SendToLog($"Sending Echo to: {_upwardKnotPort} with value: {_memorySize}");
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