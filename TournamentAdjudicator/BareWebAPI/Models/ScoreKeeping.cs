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
        public static bool endgame = false;
        static string path = "";
        static int turnNum = 0;
        public ScoreKeeping()
        {
            string pathEnd = DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString()
                                + "-" + DateTime.Now.Year.ToString() + "_" + "H" + DateTime.Now.Hour.ToString()
                                + "M" + DateTime.Now.Minute.ToString() + "S" + DateTime.Now.Second.ToString()
                                + "GameLog.txt";

            path = Path.Combine(HttpRuntime.AppDomainAppPath, "GameLogs", pathEnd);
        }

        //logs data to log.txt
        public void DataLogging(string[,,] Board, int TeamNum, int Score, List<string> words, List<string> letters, bool invalidMove, string errorMsg)
        {
            string timestamp = DateTime.Now.ToString();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true, Encoding.UTF8))
            {
                if (!invalidMove)
                {
                    file.WriteLine("VALID MOVE!");
                    file.WriteLine();
                }
                else
                {
                    file.WriteLine("%%%%% ERROR INVALID MOVE!!! %%%%%");
                    file.WriteLine("Error Message: ");
                    file.WriteLine(errorMsg);
                    file.WriteLine();
                }
                file.WriteLine("Turn: " + turnNum++);
                file.WriteLine("TeamNum: " + TeamNum);
                file.WriteLine("Timestamp: " + timestamp);
                file.WriteLine("Team Score: " + Score);
                file.WriteLine("Playable Letters: ");

                

                foreach (string letter in letters)
                {
                    file.Write(letter + " ");
                }
                file.WriteLine();
                file.WriteLine("Word(s) Played: ");
                foreach (string word in words)
                {
                    file.WriteLine(word);
                }


                file.WriteLine();
                file.WriteLine("Board:");
                file.WriteLine("\n      0    1    2    3    4    5    6    7    8    9");
                for (int r = 0; r < 10; r++)
                {
                    file.Write(r.ToString() + "  ");

                    for (int c = 0; c < 10; c++)
                    {
                        string tmp = " ";
                        switch (Board[1, r, c])
                        {
                            case "1":
                                tmp = "₁";
                                break;
                            case "2":
                                tmp = "₂";
                                break;
                            case "3":
                                tmp = "₃";
                                break;
                            case "4":
                                tmp = "₄";
                                break;
                            case "5":
                                tmp = "₅";
                                break;
                            default:
                                tmp = " ";
                                break;
                        }
                        file.Write("{0,4}", Board[0, r, c] == null ? "~" : Board[0, r, c]);
                        file.Write(tmp);
                    }
                    file.WriteLine();
                }


                file.WriteLine();
                file.WriteLine("**********************************");
                file.WriteLine();
            }
        }

        //logs data to log.txt
        public void LogPass(int TeamNum, int Score, List<string> letters)
        {
            string timestamp = DateTime.Now.ToString();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true, Encoding.UTF8))
            {
                file.WriteLine("PASS");
                file.WriteLine();
                file.WriteLine("Turn: " + turnNum++);
                file.WriteLine("TeamNum: " + TeamNum);
                file.WriteLine("Timestamp: " + timestamp);
                file.WriteLine("Team Score: " + Score);
                file.WriteLine("Playable Letters: ");
                foreach (string letter in letters)
                {
                    file.Write(letter + " ");
                }
                file.WriteLine();
                file.WriteLine();
                file.WriteLine("**********************************");
                file.WriteLine();
            }
        }
        public static void LogEndGame()
        {
            string timestamp = DateTime.Now.ToString();
            List<Player> players = TournamentAdjudicator.Controllers.UserController.Players;
            string[,,] Board = Gameplay.Board;
            Player winner = players.Find(q => q.Score == players.Max(p => p.Score));

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true, Encoding.UTF8))
            {
                file.WriteLine("**********************************");
                file.WriteLine("**********************************");
                file.WriteLine("THE GAME HAS ENDED.");
                file.WriteLine("Timestamp: " + timestamp);
                file.WriteLine("Number of turns: " + turnNum);
                foreach (Player p in players)
                {
                    file.WriteLine();
                    file.WriteLine("TeamNum: " + p.ID + " had a score of: " + p.Score);
                    if (p.Letters.Count > 0)
                    {
                        file.Write("Letters remaining in hand: ");
                        foreach (string letter in p.Letters)
                        {
                            file.Write(letter + " ");
                        }
                    }
                    file.WriteLine();
                }
                file.WriteLine();
                file.WriteLine();
                file.WriteLine("Board:");
                file.WriteLine("\n      0    1    2    3    4    5    6    7    8    9");
                for (int r = 0; r < 10; r++)
                {
                    file.Write(r.ToString() + "  ");

                    for (int c = 0; c < 10; c++)
                    {
                        string tmp = " ";
                        switch (Board[1, r, c])
                        {
                            case "1":
                                tmp = "₁";
                                break;
                            case "2":
                                tmp = "₂";
                                break;
                            case "3":
                                tmp = "₃";
                                break;
                            case "4":
                                tmp = "₄";
                                break;
                            case "5":
                                tmp = "₅";
                                break;
                            default:
                                tmp = " ";
                                break;
                        }
                        file.Write("{0,4}", Board[0, r, c] == null ? "~" : Board[0, r, c]);
                        file.Write(tmp);
                    }
                    file.WriteLine();
                }
                file.WriteLine();
                file.WriteLine("The winner is Player " + winner.ID);
                file.WriteLine("**********************************");
                file.WriteLine("**********************************");
            }
        }
    }
}