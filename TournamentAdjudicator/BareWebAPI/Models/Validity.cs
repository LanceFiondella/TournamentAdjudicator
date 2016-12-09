using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
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

        static int[,] moveOrigin = new int[1, 2];

        static List<string> usedLetters = new List<string>();

        static List<Letter> letters = new List<Letter>();

        // Stores the coordinates of the changed game squares
        static int numChangedSquares = 0;
        static int[] changedSquaresDown = new int[7];
        static int[] changedSquaresRight = new int[7];


        // Stores the coordinates of the changed stack heights
        static int numChangedHeights = 0;
        static int[] changedHeightsDown = new int[7];
        static int[] changedHeightsRight = new int[7];

        // Stores all of the letters the player had to maker their move with
        static List<string> playerLetters = new List<string>();

        // List of words that were part of the player's move
        static List<string> words = new List<string>();

        // Stores the dictionary being used to check the words played by each player
        static Dictionary<string, int> dictionary = new Dictionary<string, int>();
        //-------------------------end fields------------------------
        //-----------------------------------------------------------


        //-------------------------Accessors-------------------------
        //-----------------------------------------------------------
        public List<string> UsedLetters
        {
            get { return usedLetters; }
            set { usedLetters = value; }
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

        public List<string> PlayerLetters
        {
            get { return playerLetters; }
            set { playerLetters = value; }
        }

        public List<string> Words
        {
            get { return words; }
            set { words = value; }
        }
        //-----------------------end Accessors-----------------------
        //-----------------------------------------------------------


        //----------------Global Class Instantiations----------------
        //-----------------------------------------------------------

        // Instantiate the ScoreKeeping class
        public static ScoreKeeping scoreKeeper = new ScoreKeeping();

        //--------------end Global Class Instantiations--------------
        //-----------------------------------------------------------


        //--------------------------------------------------------------------
        // Class Constructors
        //--------------------------------------------------------------------

        // This class constructor fetches the game dictionary
        public Validity()
        {
            ScoreKeeping scoreKeeper = new ScoreKeeping();

            int count = 1;
            try
            {
                string path = Path.Combine(HttpRuntime.AppDomainAppPath, "dictionary.txt");
                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        dictionary.Add(line, count);
                        count++;
                    }
                }
                Console.Write("There are ");
                Console.Write(count);
                Console.WriteLine(" words in the dictionary.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error, the dictionary.txt file was not found, or is in the wrong directory.");
            }
        }

        //--------------------------------------------------------------------
        // end Class Constructors
        //--------------------------------------------------------------------


        //--------------------------------------------------------------------
        // Public Methods
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        // Summary:
        // Calculates the score for the current turn
        //
        // Output: 
        // Returns an integer with the score for the turn
        //--------------------------------------------------------------------
        static void GetTurnScore(Player p, bool invalidMove, string errorMsg)
        {
            if (!invalidMove)
            {
                int oneTileHigh = 0;
                foreach (Letter l in letters)
                {
                    p.Score += l.height;
                    oneTileHigh += l.height;
                }
                if (oneTileHigh == letters.Count)
                {
                    p.Score += oneTileHigh;
                    if (letters.Exists(q => q.l == "Qu")) p.Score += 2;

                }
                if (usedLetters.Count == 7)
                    p.Score += 20;
            }

            scoreKeeper.DataLogging(p.ID, p.Score, words, playerLetters, invalidMove, errorMsg);
        }// end GetTurnScore


        //--------------------------------------------------------------------
        // Summary:
        // Log when a player decides to pass
        //
        // Output: 
        // Logs that the player passed to the GameLog file
        //--------------------------------------------------------------------
        public void LogPassMove(Player p)
        {
            scoreKeeper.LogPass(p.ID, p.Score, p.Letters);
        }// end LogPassMove


        //--------------------------------------------------------------------
        // Summary:
        // The function to be called from outside the class to check the
        // validity of a move once the OldBoard and NewBoard Properties
        // have been updated to reflect the changes made by a player
        //
        // Output: 
        // Returns true if the move was a valid one, else it returns false
        //--------------------------------------------------------------------
        public bool CheckMoveValidity(bool firstTurn, Player player)
        {
            bool invalidMove = false;
            string errorMsg = "";

            // Reinitialize all variables used to store the changes between 
            // the old and new game boards to prepare for the next move
            ReinitChangeTrackers();

            // Find the game squares that were changed by the player, and check
            // if any invalid game squares were changed
            // See the comments above the function for more information
            if (!ValidChangedSquares())
            {
                invalidMove = true;
                errorMsg = "Some of the changed game squares were invalid.";
            }

            // check if letters layer matches height layer

            // If it is the first move, checks that one of the 4 centermost
            // game squares is played on
            if (firstTurn)
            {
                if (!Check4CenterSquares() && !invalidMove)
                {
                    invalidMove = true;
                    errorMsg = "An invalid First Move was made.";
                }
            }

            // Check that the letters played by the player were in their 
            // letter pool
            if (!CheckLetters() && !invalidMove)
            {
                invalidMove = true;
                errorMsg = "Letter(s) played were not from the player's letter pool.";
            }

            // Check that no invalid moves were performed on stacks
            // See the comments above the function for more information
            if (!CheckStacks() && !invalidMove)
            {
                invalidMove = true;
                errorMsg = "An error was encountered with a stack.";
            }

            // Check that all the changes were made in either 1 row or column
            string moveDirection = CheckRowColumnValidity();
            if (moveDirection == "invalid" && !invalidMove)
            {
                invalidMove = true;
                errorMsg = "The direction of the move could not be determined.";
            }

            // Get the word played by the player
            GetWords(moveDirection);

            // Check that all words part of the turn are in the dictionary
            if (!CheckWords() && !invalidMove)
            {
                invalidMove = true;
                errorMsg = "Word(s) played were not found within the dictionary.";
            }

            // Find all of the letters that were used by the player during their
            // move, so that they can removed and replaced with new letters.
            GetUsedLetters();

            // Calculate the score the player deserves for the move they made
            GetTurnScore(player, invalidMove, errorMsg);

            return !invalidMove;
        }//end CheckMoveValidity

        //--------------------------------------------------------------------
        // end Public Methods
        //--------------------------------------------------------------------


        //--------------------------------------------------------------------
        // Summary: 
        // This function should be used to reinitialize the arrays used to 
        // store the changed game square state on both layers of the game board
        //
        // Ouput: 
        // The function is void, it does not return anything, however it does
        // reinitialize the changedSquaresDown, changedSquaresRight,
        // changedHeightsDown, and changedHeightsRight arrays.
        //--------------------------------------------------------------------
        static void ReinitChangeTrackers()
        {
            // Compare the new and old game boards to see which game squares have been changed
            for (int i = 0; i < 7; i++)
            {
                changedHeightsDown[i] = 0;
                changedHeightsRight[i] = 0;
                changedSquaresDown[i] = 0;
                changedSquaresRight[i] = 0;
            }

            numChangedHeights = 0;
            numChangedSquares = 0;

            // clear used letters list
            usedLetters.Clear();

            // empty list of words from previous move
            words.Clear();

            // empty list of words from previous move
            letters.Clear();
        }//end FindMove


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
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (oldBoard[1, i, j] != newBoard[1, i, j])
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
            string newLetter;
            string oldLetter;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    // Checks that more than one letter was not played on a stack
                    // on any given turn.
                    newHeight = Int32.Parse(newBoard[1, i, j]);
                    oldHeight = Int32.Parse(oldBoard[1, i, j]);
                    newLetter = newBoard[0, i, j];
                    oldLetter = oldBoard[0, i, j];

                    if (!(newLetter == null && oldLetter == null))
                    {
                        heightDifference = newHeight - oldHeight;
                        if ((heightDifference > 1) || (heightDifference < 0))
                            return false;

                        // Checks that no stack heights exceed 5
                        if (newHeight > 5)
                            return false;

                        // Checks that the same letter was not stacked on itself
                        if (newLetter.Equals(oldLetter) && heightDifference > 0)
                            return false;

                        if (!newHeight.Equals(oldHeight))
                        {
                            changedHeightsDown[numChangedHeights] = i;
                            changedHeightsRight[numChangedHeights] = j;
                            numChangedHeights++;
                        }
                        // Check that is the heights are the same the letters are not different
                        else if (!newLetter.Equals(oldLetter))
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

            if (numChangedSquares < 2)
                return false;

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
            for (int i = 1; i < numChangedSquares; i++)
            {
                if (changedSquaresRight[i] != changedSquaresRight[i - 1])
                    rowAltered = true;
                else if (changedSquaresDown[i] != changedSquaresDown[i - 1])
                    columnAltered = true;
            }

            if (rowAltered && !columnAltered)
                return "right";
            else if (!rowAltered && columnAltered)
                return "down";
            else if (numChangedSquares == 1)
                return "singleLetter";
            else
                return "invalid";
        }//end CheckRowColumnValidity


        //--------------------------------------------------------------------
        // Summary:
        // Checks that the letters played by the player were actually in their
        // pool of letters
        //
        // Output: 
        // Returns true if all letters played were found in the player's pool
        // of letters, else returns false
        //--------------------------------------------------------------------
        static bool CheckLetters()
        {
            bool validLetters = false;
            List<string> tempPlayerLetters = new List<string>();
            foreach(string s in playerLetters)
                tempPlayerLetters.Add(s);

            for (int i = 0; i < numChangedSquares; i++)
            {
                validLetters = false;

                foreach (string s in tempPlayerLetters)
                {
                    validLetters |= s.Equals(newBoard[0, changedSquaresDown[i], changedSquaresRight[i]]);
                    if (validLetters)
                    {
                        tempPlayerLetters.Remove(s);
                        break;
                    }
                }

                if (!validLetters)
                    return false;
            }

            return validLetters;
        }//end CheckLetters

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


            for (int i = 0; i < numChangedSquares; i++)
            {
                
                currentSquare = newBoard[0, changedSquaresDown[i], changedSquaresRight[i]];
                if(currentSquare!=null&&i==0)letters.Add(new Letter(currentSquare, changedSquaresRight[i], changedSquaresDown[i], Int32.Parse(newBoard[1, changedSquaresDown[i], changedSquaresRight[i]])));

                if (moveDirection == "right")
                {
                    GetTopBottomWords(i, currentSquare);

                    if (i == 0)
                        GetLeftRightWords(i, currentSquare);
                }
                else if (moveDirection == "down")
                {
                    GetLeftRightWords(i, currentSquare);

                    if (i == 0)
                        GetTopBottomWords(i, currentSquare);
                }
                else if (moveDirection == "singleLetter")
                {
                    GetTopBottomWords(i, currentSquare);
                    GetLeftRightWords(i, currentSquare);
                }
            }
        }// end GetWord

        //--------------------------------------------------------------------
        // Summary:
        // Finds all of the letters played by the player during their move.
        //
        // Output: 
        // returns nothing, but adds the played letters to the string list
        // usedLetters.
        //--------------------------------------------------------------------
        static void GetUsedLetters()
        {
            for (int i = 0; i < numChangedSquares; i++)
            {
                usedLetters.Add(newBoard[0, changedSquaresDown[i], changedSquaresRight[i]]);
            }
        }// end CheckWords

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
            foreach (string s in words)
            {
                if (!dictionary.ContainsKey(s.ToUpper()))
                    return false;
            }

            return true;
        }// end CheckWords


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

           
            while (!(myY.Equals(0)) && ((letter = newBoard[0, myY - 1, myX]) != null))
            {
                letters.Add(new Letter(letter, myX, myY - 1, Int32.Parse(newBoard[1, myY - 1, myX])));
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
            
            while (!(myY.Equals(9)) && ((letter = newBoard[0, myY + 1, myX]) != null))
            {
                letters.Add(new Letter(letter, myX, myY + 1, Int32.Parse(newBoard[1, myY + 1, myX])));
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
            
            while (!(myX.Equals(9)) && ((letter = newBoard[0, myY, myX + 1]) != null))
            {
                letters.Add(new Letter(letter, myX+1, myY, Int32.Parse(newBoard[1, myY, myX+1])));
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

            while (!(myX.Equals(0)) && ((letter = newBoard[0, myY, myX - 1]) != null))
            {
                letters.Add(new Letter(letter, myX-1, myY, Int32.Parse(newBoard[1, myY, myX-1])));
                myString += letter;
                myX--;
            }

            return ReverseString(myString);
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
    }
    public class Letter
    {
        public string l;
        public int x;
        public int y;
        public int height;
        public Letter(string l, int x, int y,int height)
        {
            this.l = l;
            this.x = x;
            this.y = y;
            this.height = height;
        }
    }
}