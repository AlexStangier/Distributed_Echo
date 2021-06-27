using System;
using System.Collections.Generic;
using System.Threading;
using Distributed_Echo.Threads;

namespace Distributed_Echo.Knot
{
    public class Knot
    {
        public int Port;
        public String Address;
        public Knot[] Neighbours;
        public List<Knot> Knots = new List<Knot>();
        private int _memorySize;

        public Knot(String address)
        {
            Address = address;
            Neighbours = new Knot[5];
        }

        public Knot(int port, string address, int memorySize = 0, short size = 5)
        {
            Port = port;
            Address = address;
            Neighbours = new Knot[size];
            _memorySize = memorySize;
        }

        /**
         * Returns a Knot Thread
         */
        public Thread StartListening()
        {
            var kThread = new KnotThread(Port, Address, Neighbours, _memorySize);
            return new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = false;
                kThread.ThreadProc();
            });
        }

        /**
         * Creates a static predefined Network composed of Knots
         */
        public void BuildNetwork()
        {
            var knot1 = new Knot(11110, Address, 1, 3);
            var knot2 = new Knot(11111, Address, 2, 4);
            var knot3 = new Knot(11112, Address, 3, 2);
            var knot4 = new Knot(11113, Address, 4, 2);
            var knot5 = new Knot(11114, Address, 5, 2);
            var knot6 = new Knot(11115, Address, 6, 2);
            var knot7 = new Knot(11116, Address, 7, 2);
            var knot8 = new Knot(11117, Address, 8, 2);
            var knot9 = new Knot(11118, Address, 9, 3);
            var knot10 = new Knot(11119, Address, 10, 1);

            Knots.Add(knot1);
            Knots.Add(knot2);
            Knots.Add(knot3);
            Knots.Add(knot4);
            Knots.Add(knot5);
            Knots.Add(knot6);
            Knots.Add(knot7);
            Knots.Add(knot8);
            Knots.Add(knot9);
            Knots.Add(knot10);

            knot1.Neighbours[0] = knot6;
            knot1.Neighbours[1] = knot2;
            knot1.Neighbours[2] = knot3;

            knot2.Neighbours[0] = knot6;
            knot2.Neighbours[1] = knot3;
            knot2.Neighbours[2] = knot5;
            knot2.Neighbours[3] = knot7;

            knot3.Neighbours[0] = knot4;
            knot3.Neighbours[1] = knot5;

            knot4.Neighbours[0] = knot5;
            knot4.Neighbours[1] = knot8;

            knot5.Neighbours[0] = knot8;
            knot5.Neighbours[1] = knot7;

            knot6.Neighbours[0] = knot7;
            knot6.Neighbours[1] = knot10;bef

            knot7.Neighbours[0] = knot8;
            knot7.Neighbours[1] = knot10;

            knot8.Neighbours[0] = knot9;
            knot8.Neighbours[1] = knot10;

            knot9.Neighbours[0] = knot7;
            knot9.Neighbours[1] = knot10;
            knot9.Neighbours[2] = knot1;

            knot10.Neighbours[0] = knot1;
        }
    }
}