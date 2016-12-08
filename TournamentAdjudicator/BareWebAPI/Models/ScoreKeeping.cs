using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentAdjudicator.Models
{
    public class ScoreKeeping
    {
        // Vidhya's algorithm for calculating scores
       
        public int CalculateScore(int letterCount, bool[] OneTileHigh, bool Letters7, bool QuOneTile)
        {
            int result;
	        int Score;

            if (QuOneTile) { 

                result = letterCount;
                foreach (bool l in OneTileHigh)
                {
                    if (l) result += 1;
                }
			    if(Letters7)
				{
				    Score=result+20;
                }
                else
				{
				    Score=result;
                }

                return Score;
   	        }	
            else
   	        {
                result=letterCount;
                foreach (bool l in OneTileHigh)
                {
                    if (l) result += 1;
                }
                if (Letters7)
		        {
	    	        Score=result+20;
                }
                else
	            {
		            Score=result;
                }

                return Score;
   	        }
        }

        //logs data to log.txt
        public void DataLogging(int TeamNum, int Score, List<string> words, List<string> letters)
        {
            //string[] TeamNum = { "Team 1" };
            //string date = DateTime.Today.ToShortDateString();
            //DateTime localDate = DateTime.Now;  
            string timestamp = DateTime.Now.ToString();
            //int score = ScoreKeeping();
            //string word = ;
            //string letters = ;

            using (System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt", true))
            {
                file.WriteLine("TeamNum: "+TeamNum);
                file.WriteLine(timestamp);
                file.WriteLine(Score);
                foreach(string word in words)
                {
                    file.WriteLine(word);
                }
                foreach (string letter in letters)
                {
                    file.WriteLine(letters);
                }
            }
        }
    }
}
