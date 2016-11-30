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
       
        public int CalculateScore(int letterCount, bool OneTileHigh, bool Letters7, bool QuOneTile)
        {
            int result;
	        int Score; 
            
            if(OneTileHigh && QuOneTile)
   	        {
     	        result=(letterCount*2)+2;
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
                 
            else if(OneTileHigh)
   	        {
     	        result=letterCount*2;
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
        }

        //logs data to log.txt
        public static void DataLogging(int TeamNum, int Score, string[] words, string[] letters)
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
