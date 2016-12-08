using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

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
        
        //
        static string path = "";
        public ScoreKeeping()
        {
            string pathEnd = DateTime.Now.Month.ToString() + "-"+ DateTime.Now.Day.ToString()
                                + "-" + DateTime.Now.Year.ToString() + "_" + "H" + DateTime.Now.Hour.ToString()
                                + "M" +DateTime.Now.Minute.ToString() + "S" + DateTime.Now.Second.ToString()
                                + "GameLog.txt";

            path = Path.Combine(HttpRuntime.AppDomainAppPath, "GameLogs", pathEnd);
        }

        //logs data to log.txt
        public void DataLogging(int TeamNum, int Score, List<string> words, List<string> letters)
        { 
            string timestamp = DateTime.Now.ToString();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                file.WriteLine("TeamNum: " + TeamNum);
                file.WriteLine("Timestamp: " + timestamp);
                file.WriteLine("Team Score: " + Score);
                file.WriteLine("Playable Letters: ");
                foreach (string letter in letters)
                {
                    file.Write(letter + " ");
                }
                file.WriteLine();
                file.WriteLine("Words Played: ");
                foreach (string word in words)
                {
                    file.WriteLine(word);
                }
                file.WriteLine();
                file.WriteLine("**********************************");
                file.WriteLine();
            }
        }
    }
}
