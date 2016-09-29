using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentAdjudicator
{
    class SampleAI
    {
        public object BLANK_LETTER = null;
        
        private void FindNextMove() // This function finds the next possible move it can make, where a move is considered to be building an entirely new word off of an old one, rather than adding onto an old word or stacking letters to make a new word.
        { // CAP : The algorithms this function implements are literally the worst pieces of shit you have ever written and you should feel bad for writing them. You NEED to make them more efficient. You also need to FINISH this function so words can be placed in row 0 and column 0.
            List<PossibleWordPlacement> PossibleWordPlacements = new List<PossibleWordPlacement>();
            for (int r = 0; r < 9; r++) // Cycle through the rows, EXCEPT the last one. CAP : Requires special case checking.
                for (int c = 0; c < 9; c++) // Cycle through the columns, EXCEPT the last one. Requires special case checking.
                {
                    if (tilesc[r, c] != BLANK_LETTER) // Only interested in tiles with a letter on them, so that we may build a word from them.
                    {
                        if (r != 0 && c != 0) // These rows and columns require special case checking, so we check them in their own section of the for brackets
                        {
                            if (tilesc[r + 1, c] == BLANK_LETTER && tilesc[r - 1, c] == BLANK_LETTER) // Check if able to build an entirely new vertical word (rather than add onto an old one)
                            { // If this is true, we must check how many tiles are blank and available for us to work with. This means we must search upward and downward until hitting a tile that isn't blank,
                              // and then saving the starting position of a blank tile that has at least 1 blank tile of space away from other tiles on all other sides.
                              // Eventually it can be further optimized to look and see if words can be played that will make multiple connections. This function exists currently to make one connection.
                                int sr = -1, er = -1; // Starting row and ending row of the blank tiles
                                for (int i = r - 1; i >= 0; i--) // Search upwards in the rows // CAP : The for loop WAS borked, fixed now. Was a problem. Check the other for loops to see if this is happening there too
                                {
                                    if (tilesc[i, c] == BLANK_LETTER)// Check the current row tile to see if it is blank
                                    {
                                        if (tilesc[i, c + 1] == BLANK_LETTER && tilesc[i, c - 1] == BLANK_LETTER)// Check if the tiles to the sides are blank
                                        {
                                            sr = i;// Then we set this tile as the current best possible starting row
                                        }
                                        else break; // If we find a tile that is surrounded we cannot build a word off of it so simply CAP : Add this functionality later
                                    }
                                    else
                                    {
                                        sr = (sr == -1) ? -1 : sr += 1; // If we find a tile that is not blank then we've already reached our best possible starting row, but the best possible starting row connects with this tile so we must fix it
                                        break;
                                    }
                                }
                                for (int j = r + 1; j <= 9; j++) // Search downwards in the rows
                                {
                                    if (tilesc[j, c] == BLANK_LETTER) // Check the current tile to see if it is blank
                                    {
                                        if (tilesc[j, c + 1] == BLANK_LETTER && tilesc[j, c - 1] == BLANK_LETTER) // Check if the tiles to the sides are blank (if they're not we need a more complex algorithm to build word)
                                        {
                                            er = j; // Then we set this tile as the current best possible ending row
                                        }
                                        else break; // If we find a tile that is surrounded we cannot build a word off of it with this current algorithm CAP : Add this functionality later
                                    }
                                    else
                                    {
                                        er = (er == -1) ? -1 : er -= 1; // If we find a tile that is not blank then this tile connects with our last possible ending row, so we must fix it
                                        break;
                                    }
                                }

                                if (sr != -1 && er != -1) // CAP : This shouldn't be if BOTH didn't equal -1, it should be if one of them didn't equal -1 and then if one of them did adjust the sr/er
                                    PossibleWordPlacements.Add(new PossibleWordPlacement(true, tilesc[r, c], r, c, sr, er));
                                else if (er == -1 && tilesc[r + 1, c] == BLANK_LETTER && sr != -1) // If the tile next to the searched tile is blank but the ending column is set to -1
                                    PossibleWordPlacements.Add(new PossibleWordPlacement(false, tilesc[r, c], r, c, sr, r)); // Set the ending column to the current searched tile
                                else if (sr == -1 && tilesc[r - 1, c] == BLANK_LETTER && er != -1) // If the tile before the searched tile is blank but the starting column is set to -1
                                    PossibleWordPlacements.Add(new PossibleWordPlacement(false, tilesc[r, c], r, c, r, er)); // Set the starting column to the current searched tile

                                //else if (sr != -1) // If one of them was -1 and sr isn't -1, then sr is the good value and er is the adjustment needed value
                                //  ;//PossibleWordPlacements.Add(new PossibleWordPlacement(true, tilesc[r, c], r, c, sr, r)); // We set ending row to the row where the letter is because the word will above it
                                //else if (er != -1) // If one of them was -1 and er isn't -1, then er is the good value and sr is the adjustment needed value.
                                //  ;//PossibleWordPlacements.Add(new PossibleWordPlacement(true, tilesc[r, c], r, c, r, er)); // We set starting row to the row where letter is because the word will below it
                            }
                            else if (tilesc[r, c + 1] == BLANK_LETTER && tilesc[r, c - 1] == BLANK_LETTER) // Check if able to build an entirely new horizontal word (rather than add onto an old one)
                            { // If this is true we must check how many tiles are blank and available for us to work with.
                                int sc = -1, ec = -1; // Starting column and ending column of the blank tiles
                                for (int i = c - 1; i >= 0; i--) // Search leftwards in the columns
                                {
                                    if (tilesc[r, i] == BLANK_LETTER)// Check the current column tile to see if it is blank
                                    {
                                        if (tilesc[r + 1, i] == BLANK_LETTER && tilesc[r - 1, i] == BLANK_LETTER)// Check if the tiles above and below are blank
                                        {
                                            sc = i;// Then we set this tile as the current best possible starting column
                                        }
                                        else break; // If we find a tile that is surrounded we cannot build a word off of it so simply CAP : Add this functionality later
                                    }
                                    else
                                    {
                                        sc = (sc == -1) ? -1 : sc += 1; // If we find a tile that is not blank then we've already reached our best possible starting column, but the best possible starting column connects with this tile so we must fix it
                                        break;
                                    }
                                }
                                for (int j = c + 1; j <= 9; j++) // Search rightwards in the columns
                                {
                                    if (tilesc[r, j] == BLANK_LETTER) // Check the current tile to see if it is blank
                                    {
                                        if (tilesc[r + 1, j] == BLANK_LETTER && tilesc[r - 1, j] == BLANK_LETTER) // Check if the tiles above/below are blank (if they're not we need a more complex algorithm to build word)
                                        {
                                            ec = j; // Then we set this tile as the current best possible ending column
                                        }
                                        else break; // If we find a tile that is surrounded we cannot build a word off of it with this current algorithm CAP : Add this functionality later
                                    }
                                    else
                                    {
                                        ec = (ec == -1) ? -1 : ec -= 1; // If we find a tile that is not blank then this tile connects with our last possible ending column, so we must fix it
                                        break;
                                    }
                                }

                                if (sc != -1 && ec != -1)
                                    PossibleWordPlacements.Add(new PossibleWordPlacement(false, tilesc[r, c], r, c, sc, ec));
                                else if (ec == -1 && tilesc[r, c + 1] == BLANK_LETTER && sc != -1) // If the tile next to the searched tile is blank but the ending column is set to -1
                                    PossibleWordPlacements.Add(new PossibleWordPlacement(false, tilesc[r, c], r, c, sc, c)); // Set the ending column to the current searched tile
                                else if (sc == -1 && tilesc[r, c - 1] == BLANK_LETTER && ec != -1) // If the tile before the searched tile is blank but the starting column is set to -1
                                    PossibleWordPlacements.Add(new PossibleWordPlacement(false, tilesc[r, c], r, c, c, ec)); // Set the starting column to the current searched tile

                            }
                        }
                        else if (r == 0) // Row 0 means we can only build a word downward. This means we don't have to check the tiles above us to ensure they're blank. If we tried, we'd get an exception thrown at us. :(
                        {
                            if (tilesc[r + 1, c] == BLANK_LETTER) // If the tile below us contains a blank tile
                            {
                                // This if may be able to be replaced with a do while loop that increments some sort of value, and checks constantly if surrounding tiles are blank as well
                            }
                        }
                        else if (c == 0) // Column 0 means we can only build a word rightward. This means we don't have to check the tiles to the left of us to ensure they're blank.
                        {
                            if (tilesc[r, c + 1] == BLANK_LETTER) // If the tile to the right of us contains a blank tile
                            {
                                // This if may be able to be replaced with a do while loop that increments some sort of value, and checks constantly if surrounding tiles are blank as well.
                            }
                        }

                    }
                }
            PossibleWordPlacements = PossibleWordPlacements.OrderByDescending(x => x.end - x.start).ToList();
            List<string> words = new List<string>();
            foreach (PossibleWordPlacement p in PossibleWordPlacements)
            {
                int lpos = (p.dir) ? p.lrow : p.lcol;
                int maxlen = p.end - p.start + 1;

                //words.AddRange(dictionary.Where(x => x.Length <= maxlen && x.Contains(p.letter.ToString())).Where(x => x.IndexOf(p.letter) <= lpos && x.IndexOf(p.letter)+1 >= (lpos+1) - (maxlen - x.Length))); // CAP : Attempted to fix this issue by adding 1 to lpos, to make it a 1 based (starts at 1 instead of 0) number like the rest of the evals against it
                //words.AddRange(dictionary.Where(x => x.Length <= maxlen && x.Contains(p.letter.ToString())).Where(x => x.IndexOf(p.letter) <= lpos && lpos-x.IndexOf(p.letter) + x.Length-1 <= p.end)); // nah this one doesnt work
                words.AddRange(dictionary.Where(x => x.Length <= maxlen && x.Contains(p.letter.ToString())).Where(x => x.IndexOf(p.letter) <= lpos && x.Length - 1 - x.IndexOf(p.letter) <= p.end - lpos && x.IndexOf(p.letter) <= lpos - p.start)); // CAP : Evaluated lambdas, figured this one would work correctly
                words = words.OrderByDescending(x => x.Length).ToList(); // Simple explanation: These two lines save all possible playable words to a list of strings and sorts them so the longest ones come first. It is later evaluated to see which words CAN be played, based on tiles the AI has.
                /* BREAKDOWN OF THE WIZARDRY:
                * x.Length <= maxlen GETS ALL WORDS THAT'RE AS LONG AS THE MAX LENGTH OR LESS
                * x.IndexOf(p.letter) ENSURES THESE WORDS HAVE THE TILE WE'RE BUILDING OFF OF
                * x.IndexOf(p.letter) <= lpos ENSURES WE'RE NOT TRYING TO SHIFT THE TILE WE'RE BUILDING OFF OF AROUND. YOU MAY NEED TO DO THAT ONE OUT ON PAPER TO BELIEVE ME, I THOUGHT IT WAS BULLSHIT TOO. I HAD TO CHECK THIS PART LIKE 3 TIMES, LIKE WTF MAN. IT CHECKS OUT THO. ACTUALLY THIS SHIT IS WORTHLESS I THINK
                * x.Length - 1 - x.IndexOf(p.letter) <= p.end - lpos : Gets the number of tiles after the letter (Lefthand side) and then (p.end - lpos) gets the number of tiles that CAN come after the letter. The <= ensures that the number of tiles after the letter are as many as can be, and not more.
                * x.IndexOf(p.letter) <= lpos - p.start : Does the same thing as above, but with the tiles before the letter rather than after.
                * It then sorts these words with the largest ones coming first, as we always want to make the largest play we can for maximum points. This may change if we implement war gaming. */

                foreach (string w in words)
                {
                    if (HasTilesForWord(w, p.letter)) // If the AI has tiles to play the word
                        p.playablewords.Add(w); // Save the word as playable word
                }
            }

            if (PossibleWordPlacements.Count > 0)
            {
                PossibleWordPlacement p = PossibleWordPlacements[0];
                if (p.playablewords.Count > 0)
                {
                    if (p.dir)
                    {
                        int strt = p.lrow - p.playablewords[0].IndexOf(p.letter); // This piece of shit shifts the word start pos to where it needs to begin
                        AI_PlaceWord(p.playablewords[0], new int[] { strt, p.lcol }, p.dir);
                    }
                    else
                    {
                        int strt = p.lcol - p.playablewords[0].IndexOf(p.letter); // This piece of shit shifts the word start pos to where it needs to begin
                        AI_PlaceWord(p.playablewords[0], new int[] { p.lrow, strt }, p.dir);
                    }
                }
                else
                    Console.WriteLine("The AI cannot play a move, given the current tile hand.");
            }
        }




        // don't give to any other teams


    }
    class PossibleWordPlacement
    {

    }

}
