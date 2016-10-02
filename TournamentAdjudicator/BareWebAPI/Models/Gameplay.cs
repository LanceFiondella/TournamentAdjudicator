﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentAdjudicator.Controllers;

namespace TournamentAdjudicator.Models
{
    public static class Gameplay
    {
        static string[,,] board = new string[2, 10, 10]; // [letter assigned(1)/letter height(2) ,x,y]
        public static List<string> bag = new List<string>();
       
        /*
        private static List<string> p1 = new List<string>();
        private static List<string> p2 = new List<string>();
        private static List<string> p3 = new List<string>();
        private static List<string> p4 = new List<string>();
        */
        
        public static string[,,] Board
        {
            get
            {
                return board;
            }
            set
            {
                board = value;
            }
        }

        public static void example_board()
        {

            board[1, 5, 5] = "h";
            board[1, 6, 5] = "a";
            board[1, 7, 5] = "t";
            board[1, 5, 4] = "e";
            board[1, 5, 3] = "a";
            board[1, 6, 2] = "r";
            board[1, 5, 1] = "t";
            board[1, 7, 5] = "d";

            board[2, 5, 5] = "1";
            board[2, 6, 5] = "1";
            board[2, 7, 5] = "1";
            board[2, 5, 4] = "1";
            board[2, 5, 3] = "1";
            board[2, 6, 2] = "1";
            board[2, 5, 1] = "1";
            board[2, 7, 5] = "2";

        }


        public static void initalize_bag()
        {

            for(int i = 1; i <= 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    board[2, i, j] = "0";
                }
            }


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


        //George: does this remove the used letters frfom the bag? RFD
        //no, they go back in bag

       
        public static void initial_draw()
        {
            foreach (Player p in UserController.Players)
            {
                give_letters(p, 7);
            }

            
            
            

            /*     int start2 = rnd.Next(0, bag.Count);
                 p1.addSingleLetter(bag[start2]);
                 start2 = rnd.Next(0, bag.Count);
                p2.addSingleLetter(bag[start2]);
                 start2 = rnd.Next(0, bag.Count);
                 p3.addSingleLetter(bag[start2]);
                 start2 = rnd.Next(0, bag.Count);
                 p4.addSingleLetter(bag[start2]);*/



            // Console.WriteLine("p1: " + p1.Letters[0]);
            //Console.WriteLine("p2: " + p2.Letters[0]);
            //Console.WriteLine("p3: " + p3.Letters[0]);
            //Console.WriteLine("p4: " + p4.Letters[0]);

        }

        public static void give_letters(Player p, int needed)
        {
            Random rnd = new Random();
            int start2;


            for(int i = 0;  i < needed; i++)
            {
                start2 = rnd.Next(0, bag.Count);
                p.addSingleLetter(bag[start2]);
                Console.WriteLine("p: " + p.Letters[i]);
                bag.Remove(p.Letters[i]);
            }
        }


    }
}
