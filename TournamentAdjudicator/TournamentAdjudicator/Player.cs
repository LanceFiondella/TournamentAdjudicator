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
        public Player(int id, string hash, List<string> letters)
        {

            this.ID = id;
            this.Hash = hash;
            this.Letters = letters;

        }
        public Player()
        {

            this.ID = 1;
            this.Hash = "";
            this.Letters = new List<string>();

        }

        //called something like player1.addLetters(listOfNewLetters);
        public void addLetters(List< string > newletters)
        {
            this.letters.AddRange(newletters);
        }
        public void addSingleLetter(string newletter)
        {
            this.letters.Add(newletter);
        }

    }
}
