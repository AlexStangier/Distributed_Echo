using System;
using System.Threading;
using Distributed_Echo.Threads;

namespace Distributed_Echo.Logger
{
    public class Logger
    {
        private const int LoggerPort = 55555;
        private const String address = "127.0.0.1";
        private int targetPort;

        public Logger(int targetPort)
        {
            this.targetPort = targetPort;
        }

        public Thread startListening()
        {
            var lThread = new LoggerThread(LoggerPort, address, targetPort);
            return new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                lThread.ThreadProc();
            });
        }
    }
}