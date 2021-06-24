using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Distributed_Echo
{
    class Program
    {
        static void Main(string[] args)
        {
            var run = true;
            var ipv4 = "127.0.0.1";
            var port = 11110;
            try
            {
                while (run)
                {
                    Console.WriteLine("Start with default config: (127.0.0.1:11110)? [y|n]");
                    var cnf = Console.ReadLine();
                    if (!cnf.Equals("y"))
                    {
                        Console.WriteLine("Please enter a Command: ");
                        var cmd = Console.ReadLine();

                        if (cmd.Equals("SUM"))
                        {
                            Console.WriteLine("Please enter a IPv4 address: ");
                            ipv4 = Console.ReadLine();

                            Console.WriteLine($"Please enter a Port for {ipv4} to start command {cmd}: ");
                            port = Int32.Parse(Console.ReadLine());
                            run = false;
                        }
                        else
                        {
                            Console.WriteLine($"Command {cmd} couldn't be executed -> Unknown Command!");
                        }
                    }
                    else
                    {
                        run = false;
                    }
                }

                var listThreads = new List<Thread>();
                var logger = new Logger.Logger(port);

                var tLogger = logger.StartListening();
                tLogger.Start();

                var rootKnot = new Knot.Knot(ipv4);

                Console.Clear();

                rootKnot.BuildNetwork();

                foreach (var currThread in from knot in rootKnot.Knots select knot.StartListening())
                {
                    listThreads.Add(currThread);
                    currThread.Start();
                }

                tLogger.Join();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}