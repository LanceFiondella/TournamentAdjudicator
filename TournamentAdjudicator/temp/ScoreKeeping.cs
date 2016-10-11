using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentAdjudicator
{
    class ScoreKeeping
    {
        // Vidhya's algorithm for calculating scores
        // Still needs to be fully tested
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
    }
}
