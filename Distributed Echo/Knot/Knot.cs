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
            var knot1 = new Knot(11110, Address); //11110
            var knot2 = new Knot(11111, Address); //11120
            var knot3 = new Knot(11112, Address); //11130
            var knot4 = new Knot(11113, Address); //11111
            var knot5 = new Knot(11114, Address); //11112
            var knot6 = new Knot(11115, Address); //11113
            //var knot7 = new Knot(11116, Address); //11114
            //var knot8 = new Knot(11117, Address); //11115
            //var knot9 = new Knot(11118, Address); //11121
            //var knot10 = new Knot(11119, Address); //11122
            
            Knots.Add(knot1);
            Knots.Add(knot2);
            Knots.Add(knot3);
            Knots.Add(knot4);
            Knots.Add(knot5);
            Knots.Add(knot6);
            //Knots.Add(knot7);
            //Knots.Add(knot8);
            //Knots.Add(knot9);
            //Knots.Add(knot10);

            knot1.Neighbours[0] = knot6;
            knot1.Neighbours[1] = knot2;
            knot1.Neighbours[2] = knot3;

            knot2.Neighbours[0] = knot1;
            knot2.Neighbours[1] = knot6;
            knot2.Neighbours[2] = knot3;
            knot2.Neighbours[3] = knot5;

            knot3.Neighbours[0] = knot1;
            knot3.Neighbours[1] = knot2;
            knot3.Neighbours[2] = knot4;
            knot3.Neighbours[3] = knot5;

            knot4.Neighbours[0] = knot3;
            knot4.Neighbours[1] = knot5;

            knot5.Neighbours[0] = knot2;
            knot5.Neighbours[1] = knot3;
            knot5.Neighbours[2] = knot4;

            knot6.Neighbours[0] = knot1;
            knot6.Neighbours[1] = knot2;
        }
    }
}