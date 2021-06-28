using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Distributed_Echo.PDU;


namespace Distributed_Echo.Threads
{
    public class KnotThread
    {
        private int Port;
        private string Address;
        private Knot.Knot[] Neighbours;
        private short _neighsInformed = 0;
        private String _upwardKnotIPv4;
        private int _upwardKnotPort;
        private UdpClient _socket;
        private bool _initiator = false;
        private bool _informed = false;
        private int _memorySize;

        public KnotThread(Knot.Knot knot, int memorySize = 0)
        {
            Port = knot.Port;
            Address = knot.Address;
            Neighbours = knot.Neighbours;
            _socket = new UdpClient(knot.Port);
            if (memorySize is 0) new Random().Next();
            _memorySize = memorySize;
        }


        /**
         * Contains handling for incoming messages 
         */
        private void OnUdpData(IAsyncResult result)
        {
            try
            {
                IPEndPoint source = new IPEndPoint(0, 0);
                var message = _socket?.EndReceive(result, ref source);

                var knotMessage = new SendPdu().fromBytes(message);
                SendToLog($"Received: {knotMessage.Method} from {source.Port}");

                Thread.Sleep(new Random().Next(0, 100));

                if (knotMessage.Method == SendPdu.Method.START)
                {
                    _initiator = true;
                    _neighsInformed--;
                }

                EchoAlgorithm(knotMessage, source);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _socket?.BeginReceive(OnUdpData, _socket);
        }

        private void EchoAlgorithm(SendPdu.KnotMessage message, IPEndPoint source)
        {
            _neighsInformed++;
            if (!_informed)
            {
                _informed = true;
                _upwardKnotPort = source.Port;
                _upwardKnotIPv4 = source.Address.ToString();
                SendToLog(
                    $"Parent: {_upwardKnotPort} started Algorithm with memory Size: {_memorySize} is Initiator: {_initiator.ToString().ToLower()}");
                SendToLog($"Amount neighbours: {Neighbours.Length}");
                
                foreach (var neigh in Neighbours)
                {
                    if (neigh.Port != _upwardKnotPort)
                    {
                        SendToTarget(SendPdu.Method.INFO, neigh.Port, $"{_memorySize}");
                        SendToLog(
                            $"Sending INFO to {neigh.Port}");
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

            SendToLog($"{Port}: Neighbours informed: {_neighsInformed}/{Neighbours.Length}");
            if (_neighsInformed == Neighbours.Length)
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
                _socket.BeginReceive(OnUdpData, _socket);
                while (true)
                {
                    Thread.Sleep(100);
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
            _socket.Send(infoArr, infoArr.Length, new IPEndPoint(0, port));
        }

        /**
         * Send a specified message to the Logger instance.
         */
        private void SendToLog(string message)
        {
            var info = new SendPdu.KnotMessage {Method = SendPdu.Method.LOG, message = message};
            var infoArr = new SendPdu().getBytes(info);
            _socket.Send(infoArr, infoArr.Length, new IPEndPoint(0, 55555));
        }
    }
}