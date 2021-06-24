using System;
using System.Threading;
using Distributed_Echo.Threads;

namespace Distributed_Echo.Logger
{
    public class Logger
    {
        private const int LoggerPort = 55555;
        private const String Address = "127.0.0.1";
        private int _targetPort;

        public Logger(int targetPort)
        {
            this._targetPort = targetPort;
        }

        public Thread StartListening()
        {
            var lThread = new LoggerThread(LoggerPort, Address, _targetPort);
            return new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                lThread.ThreadProc();
            });
        }
    }
}