using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentAdjudicator
{
    class Player
    {
        private List<string> letters;
        private int id;
        private string hash;

        public List<string> Letters
        {
            get
            {
                return letters;
            }
            set
            {
                letters = value;
            }
        }
        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        
        public string Hash
        {
            get
            {
                return hash;
            }
            set
            {
                hash = value;
            }
        }
        


        //intialize when user connects
        Player(int id, string hash, List<string> letters)
        {

            this.ID = id;
            this.Hash = hash;
            this.Letters = letters;

        }

        //called something like player1.addLetters(listOfNewLetters);
        public void addLetters(List< string > newletters)
        {
            this.letters.AddRange(newletters);
        }


    }
}
