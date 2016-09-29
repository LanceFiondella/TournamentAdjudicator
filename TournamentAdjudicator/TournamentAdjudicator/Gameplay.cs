using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        int[,,] board_status = new int[2, 10, 10]; // [letter assigned(1)/count ,x,y]
        public static List<string> bag = new List<string>();
        private static List<string> p1 = new List<string>();
        private static List<string> p2 = new List<string>();
        private static List<string> p3 = new List<string>();
        private static List<string> p4 = new List<string>();


        static void Main(string[] args)
        {
            Program P = new Program();
            P.initalize_bag();
            P.initial_draw();




        }

        void initalize_bag()
        {
            int temp = 0;
            bag.Add("V");
            bag.Add("Qu");
            bag.Add("J");
            bag.Add("X");
            bag.Add("Z");

            while (temp < 2)
            {
                bag.Add("K");
                bag.Add("W");
                bag.Add("Y");
                temp++;
            }
            temp = 0;
            while (temp < 3)
            {
                bag.Add("B");
                bag.Add("F");
                bag.Add("G");
                bag.Add("H");
                bag.Add("P");
                temp++;
            }
            temp = 0;
            while (temp < 4)
            {
                bag.Add("C");
                temp++;
            }
            temp = 0;
            while (temp < 5)
            {
                bag.Add("D");
                bag.Add("L");
                bag.Add("M");
                bag.Add("N");
                bag.Add("R");
                bag.Add("T");
                bag.Add("U");
                temp++;
            }
            temp = 0;
            while (temp < 6)
            {
                bag.Add("S");
                temp++;
            }
            temp = 0;
            while (temp < 7)
            {
                bag.Add("A");
                bag.Add("I");
                bag.Add("O");
                temp++;
            }
            temp = 0;
            while (temp < 8)
            {
                bag.Add("E");
                temp++;
            }
        }

        void initial_draw()
        {
            Random rnd = new Random();

            int start2 = rnd.Next(0, bag.Count);
            p1.Add(bag[start2]);
            start2 = rnd.Next(0, bag.Count);
            p2.Add(bag[start2]);
            start2 = rnd.Next(0, bag.Count);
            p3.Add(bag[start2]);
            start2 = rnd.Next(0, bag.Count);
            p4.Add(bag[start2]);

            Console.WriteLine("p1: " + p1[0]);
            Console.WriteLine("p2: " + p2[0]);
            Console.WriteLine("p3: " + p3[0]);
            Console.WriteLine("p4: " + p4[0]);
        }

        int[,,] board()
        {
            return board_status;
        }
    }
}
