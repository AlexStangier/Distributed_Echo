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
        private Logger.Logger _logger;
        
        public Knot(int port, string address)
        {
            Port = port;
            Address = address;
            Neighbours = new Knot[5];
        }

        public Knot(int port, string address, Logger.Logger logger)
        {
            Port = port;
            Address = address;
            _logger = logger;
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
            var currPort = Port;
            var level1Knots = Neighbours;

            var knot1 = new Knot(11110, Address); //11110
            var knot2 = new Knot(11120, Address); //11120
            var knot3 = new Knot(11130, Address); //11130
            var knot11 = new Knot(11111, Address); //11111
            var knot12 = new Knot(11112, Address); //11112
            var knot13 = new Knot(11113, Address); //11113
            var knot14 = new Knot(11114, Address); //11114
            var knot15 = new Knot(11115, Address); //11115
            var knot21 = new Knot(11121, Address); //11121
            var knot22 = new Knot(11122, Address); //11122
            var knot23 = new Knot(11123, Address); //11123
            var knot24 = new Knot(11124, Address); //11124
            var knot25 = new Knot(11125, Address); //11125
            var knot31 = new Knot(11131, Address); //11131
            var knot32 = new Knot(11132, Address); //11132
            var knot33 = new Knot(11133, Address); //11133
            var knot34 = new Knot(11134, Address); //11134
            var knot35 = new Knot(11135, Address); //11135

            Knots.Add(knot1);
            Knots.Add(knot2);
            Knots.Add(knot3);
            Knots.Add(knot11);
            Knots.Add(knot12);
            Knots.Add(knot13);
            Knots.Add(knot14);
            Knots.Add(knot15);
            Knots.Add(knot21);
            Knots.Add(knot22);
            Knots.Add(knot23);
            Knots.Add(knot24);
            Knots.Add(knot25);
            Knots.Add(knot31);
            Knots.Add(knot32);
            Knots.Add(knot33);
            Knots.Add(knot34);
            Knots.Add(knot35);

            //Set neighbours of root Level
            level1Knots[0] = knot1;
            level1Knots[1] = knot2;
            level1Knots[2] = knot3;

            var level11Knots = level1Knots[0].Neighbours;
            //Set neighbours of first Level
            level11Knots[0] = knot11;
            level11Knots[1] = knot12;
            level11Knots[2] = knot13;
            level11Knots[3] = knot14;
            level11Knots[4] = knot15;

            var level12Knots = level1Knots[1].Neighbours;
            //Set neighbours of first Level
            level12Knots[0] = knot21;
            level12Knots[1] = knot22;
            level12Knots[2] = knot23;
            level12Knots[3] = knot24;
            level12Knots[4] = knot25;

            var level13Knots = level1Knots[2].Neighbours;
            //Set neighbours of first Level
            level13Knots[0] = knot31;
            level13Knots[1] = knot32;
            level13Knots[2] = knot33;
            level13Knots[3] = knot34;
            level13Knots[4] = knot35;

            //Set neighbours of second Level
            knot11.Neighbours[0] = knot12; 
            knot11.Neighbours[1] = knot2;
            
            knot12.Neighbours[0] = knot13;
            knot12.Neighbours[1] = knot2;
            
            knot13.Neighbours[0] = knot21;
            knot13.Neighbours[1] = knot2;

            knot21.Neighbours[0] = knot22;
            knot21.Neighbours[1] = knot13;

            knot32.Neighbours[0] = knot21;
            knot32.Neighbours[1] = knot2;
            knot32.Neighbours[2] = knot32;
            knot32.Neighbours[3] = knot3;

            knot31.Neighbours[0] = knot32;
            knot31.Neighbours[1] = knot3;

            knot3.Neighbours[0] = knot31;
            knot3.Neighbours[1] = knot32;
            knot3.Neighbours[2] = knot2;
            knot3.Neighbours[3] = knot22;

            knot2.Neighbours[0] = knot1;
            knot2.Neighbours[1] = knot11;
            knot2.Neighbours[2] = knot12;
            knot2.Neighbours[3] = knot13;
            knot2.Neighbours[4] = knot21;

            knot1.Neighbours[0] = knot11;
            knot1.Neighbours[1] = knot12;
            knot1.Neighbours[2] = knot13;
            knot1.Neighbours[3] = knot2;
        }
    }
}