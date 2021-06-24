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
        public bool Informed = false;

        public Knot(String address)
        {
            Address = address;
            Neighbours = new Knot[5];
        }

        public Knot(int port, string address)
        {
            Port = port;
            Address = address;
            Neighbours = new Knot[5];
        }

        /**
         * Returns a Knot Thread
         */
        public Thread StartListening()
        {
            var kThread = new KnotThread(Port, Address, Neighbours);
            return new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                kThread.ThreadProc();
            });
        }

        /**
         * Creates a static predefined Network composed of Knots
         */
        public void BuildNetwork()
        {
            var knot1 = new Knot(11110, Address);
            var knot2 = new Knot(11111, Address);
            var knot3 = new Knot(11112, Address);
            var knot4 = new Knot(11113, Address);
            var knot5 = new Knot(11114, Address);
            var knot6 = new Knot(11115, Address);
            var knot7 = new Knot(11116, Address);
            var knot8 = new Knot(11117, Address);
            var knot9 = new Knot(11118, Address);
            var knot10 = new Knot(11119, Address);

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

            knot2.Neighbours[0] = knot1;
            knot2.Neighbours[1] = knot6;
            knot2.Neighbours[2] = knot3;
            knot2.Neighbours[3] = knot5;
            knot2.Neighbours[4] = knot7;

            knot3.Neighbours[0] = knot1;
            knot3.Neighbours[1] = knot2;
            knot3.Neighbours[2] = knot4;
            knot3.Neighbours[3] = knot5;

            knot4.Neighbours[0] = knot3;
            knot4.Neighbours[1] = knot5;
            knot4.Neighbours[2] = knot8;

            knot5.Neighbours[0] = knot2;
            knot5.Neighbours[1] = knot3;
            knot5.Neighbours[2] = knot4;
            knot5.Neighbours[3] = knot8;
            knot5.Neighbours[4] = knot7;

            knot6.Neighbours[0] = knot1;
            knot6.Neighbours[1] = knot2;
            knot6.Neighbours[2] = knot7;
            knot6.Neighbours[3] = knot10;

            knot7.Neighbours[0] = knot2;
            knot7.Neighbours[1] = knot5;
            knot7.Neighbours[2] = knot6;
            knot7.Neighbours[3] = knot8;
            knot7.Neighbours[4] = knot10;

            knot8.Neighbours[0] = knot5;
            knot8.Neighbours[1] = knot7;
            knot8.Neighbours[2] = knot9;
            knot8.Neighbours[3] = knot10;

            knot9.Neighbours[0] = knot7;
            knot9.Neighbours[1] = knot8;
            knot9.Neighbours[2] = knot10;

            knot10.Neighbours[0] = knot6;
            knot10.Neighbours[1] = knot7;
            knot10.Neighbours[2] = knot8;
            knot10.Neighbours[3] = knot9;
        }
    }
}