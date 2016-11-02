using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentAdjudicator.Controllers;
using TournamentAdjudicator.Models;

namespace TournamentAdjudicator.Models
{
    public class Validity
    {
        //---------------------------fields--------------------------
        //-----------------------------------------------------------
        static string[, ,] oldBoard = new string[2, 10, 10];
        static string[, ,] newBoard = new string[2, 10, 10];

        // Set to begin at 2 for testing
        static int moveCount = 1;

        static int[,] moveOrigin = new int[1, 2];

        // Stores the coordinates of the changed game squares
        static int numChangedSquares = 0;
        static int[] changedSquaresDown = new int[7];
        static int[] changedSquaresRight = new int[7];

        
        // Stores the coordinates of the changed stack heights
        static int numChangedHeights = 0;
        static int[] changedHeightsDown = new int[7];
        static int[] changedHeightsRight = new int[7];

        // List of words that were part of the player's move
        static List<string> words = new List<string>();
        //-------------------------end fields------------------------
        //-----------------------------------------------------------


        //-------------------------Accessors-------------------------
        //-----------------------------------------------------------
        public int MoveCount
        {
            get { return moveCount; }
            set { moveCount = value; }
        }

        public string[, ,] OldBoard
        {
            get { return oldBoard; }
            set { oldBoard = value; }
        }

        public string[, ,] NewBoard
        {
            get { return newBoard; }
            set { newBoard = value; }
        }

        public List<string> Words
        {
            get { return words; }
            set { words = value; }
        }
        //-----------------------end Accessors-----------------------
        //-----------------------------------------------------------


        //--------------------------------------------------------------------
        // Summary: 
        // Finds all of the game squares that were changed on a single
        // turn. It also checks whether more than 7 tiles were changed on the 
        // current turn(implying that a player played more tiles than possible
        // on a single turn)
        //
        // Ouput: 
        // Returns the coordinates for the changed squares to a field
        // called changedSquares, and then if 7 or less tiles were changed the
        // function will return true, else it returns false
        //--------------------------------------------------------------------
        static bool ValidChangedSquares()
        {
            // Compare the new and old game boards to see which game squares have been changed
            for(int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    if(oldBoard[1,i,j] != newBoard[1,i,j])
                    {
                        changedSquaresDown[numChangedSquares] = i;
                        changedSquaresRight[numChangedSquares] = j;
                        numChangedSquares++;

                        if (numChangedSquares >= 7)
                            return false;
                    }
                }
            }

            return true;
        }//end FindMove


        //--------------------------------------------------------------------
        // Summary: 
        // Checks that the same letter was not stacked on itself,
        // that more than one letter was not played on a single stack on any
        // any given turn, and that no stack is more than 5 tiles high
        //
        // Ouput: 
        // Returns true as long as none of the above rules were not
        // broken, else it returns false
        //--------------------------------------------------------------------
        static bool CheckStacks()
        {
            int heightDifference = 0;
            int newHeight = 0;
            int oldHeight = 0;

            for(int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    // Checks that more than one letter was not played on a stack
                    // on any given turn.
                    newHeight = Int32.Parse(newBoard[1,i,j]);
                    oldHeight = Int32.Parse(oldBoard[1,i,j]);

                    heightDifference = newHeight - oldHeight;
                    if((heightDifference > 1) || (heightDifference < 0))
                        return false;

                    // Checks that no stack heights exceed 5
                    if(newHeight > 5)
                        return false;

                    // checks that both levels match
                    if (!changedSquaresDown[numChangedHeights].Equals(i) ||
                       !changedSquaresRight[numChangedHeights].Equals(j))
                    {
                        return false;
                    }

                    if(!newHeight.Equals(oldHeight))
                    {
                        changedHeightsDown[numChangedHeights] = i;
                        changedHeightsRight[numChangedHeights] = j;
                        numChangedHeights++;

                        // Checks that the same letter was not stacked on itself
                        if (oldBoard[0, i, j].Equals(newBoard[0, i, j]))
                            return false;
                    }
                }
            }

            return true;
        }//end CheckStacks()


        //--------------------------------------------------------------------
        // Summary: 
        // When called it ensures that current move was made on one 
        // of the 4 center game board squares.
        //
        // Output:
        // returns true if a move was made on one of the 4 center squares,
        // else it returns false
        //--------------------------------------------------------------------
        static bool Check4CenterSquares()
        {
            int X;
            int Y;

            bool xBool = false;
            bool yBool = false;

            for (int i = 0; i < numChangedSquares; i++)
            {
                Y = changedSquaresDown[i];
                X = changedSquaresRight[i];

                if (Y.Equals(4) || Y.Equals(5))
                    yBool = true;

                if (X.Equals(4) || X.Equals(5))
                    xBool = true;
            }
            
            return (xBool && yBool);
        } // end Check4CenterSquares


        //--------------------------------------------------------------------
        // Summary:
        // Checks whether all of the changed game squares were changed in the
        // either the same row or column. 
        //
        // Output: 
        // returns the string "right" or "down" if all changes were 
        // made strictly in either a left to right horizontal or top to bottom 
        // vertical direction, if changes were made in both then the string 
        // "invalid" is returned
        //--------------------------------------------------------------------
        static string CheckRowColumnValidity()
        {
            bool rowAltered = false;
            bool columnAltered = false;
            for(int i = 1; i < numChangedSquares; i++)
            {
                if (changedSquaresRight[i] != changedSquaresRight[i - 1])
                    rowAltered = true;
                else if(changedSquaresDown[i] != changedSquaresDown[i - 1])
                    columnAltered = true;
            }

            if (rowAltered && !columnAltered)
                return "right";
            else if (!rowAltered && columnAltered)
                return "down";
            else
                return "invalid";
        }//end CheckRowColumnValidity


        //--------------------------------------------------------------------
        // Summary:
        // Figures out the word that has been played by the player
        //
        // Output: 
        // Returns a string containing the word played
        //--------------------------------------------------------------------
        static void GetWords(string moveDirection)
        {
            string currentSquare;

            for(int i = 0; i < numChangedSquares; i++)
            {
                currentSquare = newBoard[0, changedSquaresDown[i], changedSquaresRight[i]];

                if (moveDirection == "right")
                {
                    GetTopBottomWords(i, currentSquare);

                    if(i == 0)
                    {
                        GetLeftRightWords(i, currentSquare);
                    }
                }
                else
                {
                    GetLeftRightWords(i, currentSquare);

                    if (i == 0)
                    {
                        GetTopBottomWords(i, currentSquare);
                    }
                }   
            }
        }// end GetWord


        //--------------------------------------------------------------------
        // Summary:
        // Check that the played word is in the game dictionary.
        //
        // Output: 
        // returns true if the word was found in the dictionary, else returns
        // false
        //--------------------------------------------------------------------
        static bool CheckWords()
        {

            return true;
        }// end CheckWords


        //--------------------------------------------------------------------
        // Summary:
        // Assembles data gather about the move into the format expected by the
        // ScoreKeeper
        //
        // Output: 
        // Edits the properties the ScoreKeeper will need accordingly
        //--------------------------------------------------------------------
        static void ScoreKeeperData()
        {
            
        }// end ScoreKeeperData


        //--------------------------------------------------------------------
        // Summary:
        // The function to be called from outside the class to check the
        // validity of a move once the OldBoard and NewBoard Properties
        // have been updated to reflect the changes made by a player
        //
        // Output: 
        // Returns true if the move was a valid one, else it returns false
        //--------------------------------------------------------------------
        public bool CheckMoveValidity()
        {
            if (!ValidChangedSquares())
                return false;

            // If it is the first move, checks that one of the 4 centermost
            // game squares is played on
            if (moveCount == 1)
            {
                if (!Check4CenterSquares())
                    return false;
            }

            if (!CheckStacks())
                return false;

            // Check that all the changes were made in either 1 row or column
            string moveDirection = CheckRowColumnValidity();

            // Get the word played by the player
            GetWords(moveDirection);

            foreach(string s in words)
            {
                Console.WriteLine(s);
            }

            return true;
        }//end CheckMoveValidity


        //----------------------------------------------------------------------
        //-------------------------Game Square Surrounds------------------------
        //----------------------------------------------------------------------


        //--------------------------------------------------------------------
        // Summary:
        // Checks if there are letters above the game square specified by X 
        // and Y
        //
        // Output: 
        // Returns a string of the letters found above the game square. If none
        // were found, then an empty string is returned.
        //--------------------------------------------------------------------
        static string CheckAbove(int X, int Y)
        {
            int myX = X;
            int myY = Y;
            string myString = "";
            string letter = "";

            while (!(myY.Equals(0)) && ((letter = newBoard[0, myY-1, myX]) != "-"))
            {
                myString += letter;
                myY--;
            }

            return ReverseString(myString);
        }


        //--------------------------------------------------------------------
        // Summary:
        // Checks if there are letters below the game square specified by X 
        // and Y
        //
        // Output: 
        // Returns a string of the letters found above the game square. If none
        // were found, then an empty string is returned.
        //--------------------------------------------------------------------
        static string CheckBelow(int X, int Y)
        {
            int myX = X;
            int myY = Y;
            string myString = "";
            string letter = "";

            while (!(myY.Equals(9)) && ((letter = newBoard[0, myY + 1, myX]) != "-"))
            {
                myString += letter;
                myY++;
            }

            return myString;
        }


        //--------------------------------------------------------------------
        // Summary:
        // Checks if there are letters right the game square specified by X 
        // and Y
        //
        // Output: 
        // Returns a string of the letters found above the game square. If none
        // were found, then an empty string is returned.
        //--------------------------------------------------------------------
        static string CheckRight(int X, int Y)
        {
            int myX = X;
            int myY = Y;
            string myString = "";
            string letter = "";

            while (!(myX.Equals(9)) && ((letter = newBoard[0, myY, myX + 1]) != "-"))
            {
                myString += letter;
                myX++;
            }

            return myString;
        }


        //--------------------------------------------------------------------
        // Summary:
        // Checks if there are letters left the game square specified by X 
        // and Y
        //
        // Output: 
        // Returns a string of the letters found above the game square. If none
        // were found, then an empty string is returned.
        //--------------------------------------------------------------------
        static string CheckLeft(int X, int Y)
        {
            int myX = X;
            int myY = Y;
            string myString = "";
            string letter = "";

            while (!(myX.Equals(0)) && ((letter = newBoard[0, myY, myX - 1]) != "-"))
            {
                myString += letter;
                myX--;
            }

            return myString;
        }


        //--------------------------------------------------------------------
        // Summary:
        // Finds word that can be made in the left-to-right direction from the
        // changesSquare(Right and Down) at index i.
        //
        // Output: 
        // Returns nothing, but it does update the list of words for the turn
        //--------------------------------------------------------------------
        static void GetLeftRightWords(int i, string currentSquare)
        {
            string leftStr;
            string rightStr;
            string word;

            leftStr = CheckLeft(changedSquaresRight[i], changedSquaresDown[i]);
            rightStr = CheckRight(changedSquaresRight[i], changedSquaresDown[i]);
            if (!(rightStr.Equals("")) || !(leftStr.Equals("")))
                leftStr += currentSquare;

            word = leftStr + rightStr;
            if (!word.Equals(""))
                words.Add(word);
        }


        //--------------------------------------------------------------------
        // Summary:
        // Finds word that can be made in the top-to-bottom direction from the
        // changesSquare(Right and Down) at index i.
        //
        // Output: 
        // Returns nothing, but it does update the list of words for the turn
        //--------------------------------------------------------------------
        static void GetTopBottomWords(int i, string currentSquare)
        {
            string aboveStr;
            string belowStr;
            string word;

            aboveStr = CheckAbove(changedSquaresRight[i], changedSquaresDown[i]);
            belowStr = CheckBelow(changedSquaresRight[i], changedSquaresDown[i]);
            if (!aboveStr.Equals("") || !belowStr.Equals(""))
                aboveStr += currentSquare;

            word = aboveStr + belowStr;
            if (!word.Equals(""))
                words.Add(word);
        }


        //--------------------------------------------------------------------
        // Summary:
        // Reverses the order of the characters which make up the string
        //
        // Output: 
        // The reversed string
        //--------------------------------------------------------------------
        static string ReverseString(string myString)
        {
            char[] tempChar = myString.ToCharArray();
            Array.Reverse(tempChar);
            return new string(tempChar);
        }


        //---------------------------------------------------------------
        //-------------------------Method Testing------------------------
        //---------------------------------------------------------------

        // Used to test newly developed methods
        public string MethodTester()
        {   
            //string wordP1 = CheckAbove(5, 5);
            //string wordP2 = CheckBelow(5, 5);
            //string wordP3 = CheckRight(5, 5);
            //string wordP4 = CheckLeft(5, 5);

            //return wordP1 + wordP2 + "   " + wordP3 + wordP4;
            return "";
        }
    }
}
