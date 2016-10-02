using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentAdjudicator.Models
{
    class Program
    {
        static void Main(string[] args)
        {
            Gameplay P = new Gameplay();
            P.initalize_bag();
            P.initial_draw();

            //P.board();
            //Console.WriteLine("{0}", P.board().ToString());
            //Console.ReadKey();



        }
    }
}
