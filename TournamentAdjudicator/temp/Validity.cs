using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveAnalysis
{
    class Validity
    {
        //---------------------------fields--------------------------
        //-----------------------------------------------------------
        static string[, ,] oldBoard = new string[2, 10, 10];
        static string[, ,] newBoard = new string[2, 10, 10];

        static int moveCount;

        static int[,] moveOrigin = new int[1, 2];

        // Stores the coordinates of the changed game squares
        static int numChangedSquares = 0;
        static int[,] changedSquares = new int[7, 2];
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
        static bool FindChangedSquares()
        {
            // Compare the new and old game boards to see which game squares have been changed
            for(int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    if(oldBoard[1,i,j] != newBoard[1,i,j])
                    {
                        changedSquares[numChangedSquares,0] = i;
                        changedSquares[numChangedSquares,1] = j;
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

            return true;
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
                if (changedSquares[i, 0] != changedSquares[i - 1, 0])
                    rowAltered = true;
                else if(changedSquares[i, 1] != changedSquares[i - 1, 1])
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
        static string GetWord()
        {

            return "";
        }// end GetWord


        //--------------------------------------------------------------------
        // Summary:
        // Check that the played word is in the game dictionary.
        //
        // Output: 
        // returns true if the word was found in the dictionary, else returns
        // false
        //--------------------------------------------------------------------
        static bool CheckWord()
        {

            return true;
        }// end GetWord


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
            
        }


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
            if (!FindChangedSquares())
                return false;

            // Check that all the changes were made in either 1 row or column
            string MoveDirection = CheckRowColumnValidity();

            // If it is the first move, checks that one of the 4 centermost
            // game squares is played on
            if (moveCount == 1)
            {

            }


            return true;
        }//end CheckMoveValidity


        //---------------------------------------------------------------
        //-------------------------Method Testing------------------------
        //---------------------------------------------------------------

        // Used to test newly developed methods
        public string MethodTester()
        {
            string word = GetWord();

            return word;
        }
    }
}
