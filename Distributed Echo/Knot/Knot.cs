using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Distributed_Echo.Threads;

namespace Distributed_Echo.Knot
{
    public class Knot
    {
        public int _port;
        public String _address;
        public Knot[] _neighbours;
        private Logger.Logger _logger;
        public List<Knot> _knots = new List<Knot>();


        public Knot(int port, string address)
        {
            _port = port;
            _address = address;
            _neighbours = new Knot[5];
        }

        public Knot(int port, string address, Logger.Logger logger)
        {
            _port = port;
            _address = address;
            _logger = logger;
            _neighbours = new Knot[5];
        }

        /**
         * Returns a Knot Thread
         */
        public Thread startListening()
        {
            var kThread = new KnotThread(_port, _address, _neighbours);
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
            var currPort = _port;
            var level1Knots = _neighbours;

            var knot1 = new Knot(11110, _address); //11110
            var knot2 = new Knot(11120, _address); //11120
            var knot3 = new Knot(11130, _address); //11130
            var knot11 = new Knot(11111, _address); //11111
            var knot12 = new Knot(11112, _address); //11112
            var knot13 = new Knot(11113, _address); //11113
            var knot14 = new Knot(11114, _address); //11114
            var knot15 = new Knot(11115, _address); //11115
            var knot21 = new Knot(11121, _address); //11121
            var knot22 = new Knot(11122, _address); //11122
            var knot23 = new Knot(11123, _address); //11123
            var knot24 = new Knot(11124, _address); //11124
            var knot25 = new Knot(11125, _address); //11125
            var knot31 = new Knot(11131, _address); //11131
            var knot32 = new Knot(11132, _address); //11132
            var knot33 = new Knot(11133, _address); //11133
            var knot34 = new Knot(11134, _address); //11134
            var knot35 = new Knot(11135, _address); //11135

            _knots.Add(knot1);
            _knots.Add(knot2);
            _knots.Add(knot3);
            _knots.Add(knot11);
            _knots.Add(knot12);
            _knots.Add(knot13);
            _knots.Add(knot14);
            _knots.Add(knot15);
            _knots.Add(knot21);
            _knots.Add(knot22);
            _knots.Add(knot23);
            _knots.Add(knot24);
            _knots.Add(knot25);
            _knots.Add(knot31);
            _knots.Add(knot32);
            _knots.Add(knot33);
            _knots.Add(knot34);
            _knots.Add(knot35);

            //Set neighbours of root Level
            level1Knots[0] = knot1;
            level1Knots[1] = knot2;
            level1Knots[2] = knot3;

            var level11Knots = level1Knots[0]._neighbours;
            //Set neighbours of first Level
            level11Knots[0] = knot11;
            level11Knots[1] = knot12;
            level11Knots[2] = knot13;
            level11Knots[3] = knot14;
            level11Knots[4] = knot15;

            var level12Knots = level1Knots[1]._neighbours;
            //Set neighbours of first Level
            level12Knots[0] = knot21;
            level12Knots[1] = knot22;
            level12Knots[2] = knot23;
            level12Knots[3] = knot24;
            level12Knots[4] = knot25;

            var level13Knots = level1Knots[2]._neighbours;
            //Set neighbours of first Level
            level13Knots[0] = knot31;
            level13Knots[1] = knot32;
            level13Knots[2] = knot33;
            level13Knots[3] = knot34;
            level13Knots[4] = knot35;

            //Set neighbours of second Level
            /*knot11._neighbours[0] = knot12; 
            knot11._neighbours[1] = knot2;
            
            knot12._neighbours[0] = knot13;
            knot12._neighbours[1] = knot2;
            
            knot13._neighbours[0] = knot21;
            knot13._neighbours[1] = knot2;

            knot21._neighbours[0] = knot22;
            knot21._neighbours[1] = knot13;

            knot32._neighbours[0] = knot21;
            knot32._neighbours[1] = knot2;
            knot32._neighbours[2] = knot32;
            knot32._neighbours[3] = knot3;

            knot31._neighbours[0] = knot32;
            knot31._neighbours[1] = knot3;

            knot3._neighbours[0] = knot31;
            knot3._neighbours[1] = knot32;
            knot3._neighbours[2] = knot2;
            knot3._neighbours[3] = knot22;

            knot2._neighbours[0] = knot1;
            knot2._neighbours[1] = knot11;
            knot2._neighbours[2] = knot12;
            knot2._neighbours[3] = knot13;
            knot2._neighbours[4] = knot21;

            knot1._neighbours[0] = knot11;
            knot1._neighbours[1] = knot12;
            knot1._neighbours[2] = knot13;
            knot1._neighbours[3] = knot2;*/
        }
    }
}