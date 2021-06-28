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

        public Knot(String address)
        {
            Address = address;
            Neighbours = new Knot[5];
        }

        public Knot(int port, string address, int size = 5)
        {
            Port = port;
            Address = address;
            Neighbours = new Knot[size];
        }

        /**
         * Returns a Knot Thread
         */
        public Thread StartListening(Knot knot, short _memorySize)
        {
            var kThread = new KnotThread(knot, _memorySize);
            return new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                kThread.ThreadProc();
            });
        }

        /**
         * Creates a dynamic Network composed of Knots
         */
        public List<Knot> BuildNetwork(int startingPort, int amountKnots)
        {
            var port = startingPort;
            var random = new Random();
            var knots = new List<Knot>();
            knots.Add(new Knot(port++, Address, random.Next(1, amountKnots)));
            knots.Add(new Knot(port++, Address, random.Next(1, amountKnots)));
            knots.Add(new Knot(port++, Address, random.Next(1, amountKnots)));
            knots.Add(new Knot(port++, Address, random.Next(1, amountKnots)));
            knots.Add(new Knot(port++, Address, random.Next(1, amountKnots)));
            knots.Add(new Knot(port++, Address, random.Next(1, amountKnots)));
            knots.Add(new Knot(port++, Address, random.Next(1, amountKnots)));
            knots.Add(new Knot(port++, Address, random.Next(1, amountKnots)));
            knots.Add(new Knot(port++, Address, random.Next(1, amountKnots)));
            knots.Add(new Knot(port++, Address, random.Next(1, amountKnots)));

            for (short i = 0; i <= knots.Count - 1; i++)
            {
                var exclude = new List<int>();
                exclude.Add(i);
                for (var j = 0; j <= knots[i].Neighbours.Length - 1; j++)
                {
                    var rand = RandomExcept(exclude);
                    knots[i].Neighbours[j] = knots[rand];
                    exclude.Add(rand);
                }
            }

            return knots;
        }

        /**
         * Creates a semi dynamic Network composed of Knots
         */
        public List<Knot> BuildNetwork()
        {
            var knots = new List<Knot>();
            knots.Add(new Knot(11110, Address));
            knots.Add(new Knot(11111, Address));
            knots.Add(new Knot(11112, Address));
            knots.Add(new Knot(11113, Address));
            knots.Add(new Knot(11114, Address, 4));
            knots.Add(new Knot(11115, Address, 4));
            knots.Add(new Knot(11116, Address, 3));
            knots.Add(new Knot(11117, Address, 3));
            knots.Add(new Knot(11118, Address, 3));
            knots.Add(new Knot(11119, Address, 3));


            knots[0].Neighbours[0] = knots[1];
            knots[0].Neighbours[1] = knots[2];
            knots[0].Neighbours[2] = knots[3];
            knots[0].Neighbours[3] = knots[9];
            knots[0].Neighbours[4] = knots[8];

            knots[1].Neighbours[0] = knots[0];
            knots[1].Neighbours[1] = knots[4];
            knots[1].Neighbours[2] = knots[2];
            knots[1].Neighbours[3] = knots[6];
            knots[1].Neighbours[4] = knots[9];

            knots[2].Neighbours[0] = knots[1];
            knots[2].Neighbours[1] = knots[4];
            knots[2].Neighbours[2] = knots[5];
            knots[2].Neighbours[3] = knots[3];
            knots[2].Neighbours[4] = knots[0];

            knots[3].Neighbours[0] = knots[0];
            knots[3].Neighbours[1] = knots[2];
            knots[3].Neighbours[2] = knots[5];
            knots[3].Neighbours[3] = knots[7];
            knots[3].Neighbours[4] = knots[8];

            knots[4].Neighbours[0] = knots[5];
            knots[4].Neighbours[1] = knots[2];
            knots[4].Neighbours[2] = knots[1];
            knots[4].Neighbours[3] = knots[6];

            knots[5].Neighbours[0] = knots[4];
            knots[5].Neighbours[1] = knots[3];
            knots[5].Neighbours[2] = knots[2];
            knots[5].Neighbours[3] = knots[7];

            knots[6].Neighbours[0] = knots[1];
            knots[6].Neighbours[1] = knots[4];
            knots[6].Neighbours[2] = knots[9];

            knots[7].Neighbours[0] = knots[3];
            knots[7].Neighbours[1] = knots[5];
            knots[7].Neighbours[2] = knots[8];

            knots[8].Neighbours[0] = knots[3];
            knots[8].Neighbours[1] = knots[7];
            knots[8].Neighbours[2] = knots[0];

            knots[9].Neighbours[0] = knots[6];
            knots[9].Neighbours[1] = knots[1];
            knots[9].Neighbours[2] = knots[0];
            
            return knots;
        }

        private int RandomExcept(List<int> exclude)
        {
            var random = new Random();
            var curr = random.Next(0, 9);
            while (exclude.Contains(curr))
            {
                curr = random.Next(0, 9);
            }

            return curr;
        }
    }
}