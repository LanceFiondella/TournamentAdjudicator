using TournamentAdjudicator.Controllers;
using TournamentAdjudicator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace TournamentAdjudicator.Controllers
{
    public class GameController : ApiController
    {
        
 
        [HttpGet]
        public IHttpActionResult GetGame(int id)
        {
            
            var user = UserController.Players.FirstOrDefault((p) => p.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            //end game
            if (Gameplay.Pass_Count >= UserController.Players.Count)
            {
                string endgameString = "The game has ended. Final Scores:\n";


                if (!ScoreKeeping.endgame)
                {
                    foreach (Player p in UserController.Players)
                    {
                        p.Score -= p.Letters.Count * 5;
                    }
                    ScoreKeeping.LogEndGame();
                    ScoreKeeping.endgame = true;
                }

                foreach (Player p in UserController.Players)
                {
                    endgameString += "Player " + p.ID + ": " + p.Score + "\n";
                    
                }
                Player winner = UserController.Players.Find(q => q.Score == UserController.Players.Max(p => p.Score));
                endgameString += "The winner is Player " + winner.ID + "!";
               
                

              return Ok(endgameString);
            }
            Status report = new Status();
            report.Letters = user.Letters;
            report.Score = user.Score;
            try
            {
                if (Request.Headers.GetValues("Hash").ElementAt(0).ToString().Equals(user.Hash))
                {
                    
                    return Ok(report);

                    //Add code to return Game data and Player data
                }
            }
            catch
            {
                Ok("User Auth Failed");
            }

            return Ok("User Auth Failed");
        }

        [HttpPost]
        public IHttpActionResult PostGame(int id)
        {
            var user = UserController.Players.FirstOrDefault((p) => p.ID == id);
            if (user == null)
            {
                return NotFound();
            }
            if (Gameplay.Pass_Count >= UserController.Players.Count)
            {
                string endgameString = "The game has ended. Final Scores:\n";


                if (!ScoreKeeping.endgame)
                {
                    foreach (Player p in UserController.Players)
                    {
                        p.Score -= p.Letters.Count * 5;
                    }
                    ScoreKeeping.LogEndGame();
                    ScoreKeeping.endgame = true;
                }

                foreach (Player p in UserController.Players)
                {
                    endgameString += "Player " + p.ID + ": " + p.Score + "\n";

                }
                Player winner = UserController.Players.Find(q => q.Score == UserController.Players.Max(p => p.Score));
                endgameString += "The winner is Player " + winner.ID + "!";
                return Ok(endgameString);

                
            }
            try
            {
                if (Request.Headers.GetValues("Hash").ElementAt(0).ToString().Equals(user.Hash))
                {
                    JToken move;
                    JToken exchange;
                    try
                    {
                        JToken RequestValue = JObject.Parse(Request.Headers.GetValues("Move").ElementAt(0).ToString());
                        move = RequestValue.SelectToken("Board");
                        exchange = RequestValue.SelectToken("Letters");
                    }
                    catch
                    {
                        move = null;
                        exchange = null;
                    }
                    string[,,] dict =null;
                    string[] letter = null;
                    if (move != null)
                        try {
                            dict = JsonConvert.DeserializeObject<string[,,]>(move.ToString());
                        }
                        catch { return Ok("Something went wrong in deserializing for a board play"); }
                    if (exchange != null)
                        try {
                            letter = JsonConvert.DeserializeObject<string[]>(exchange.ToString());
                        }
                        catch { return Ok("Something went wrong in deserializing for an exchange play"); }

                    if (dict != null)
                    {
                        //var dict = JsonConvert.DeserializeObject<string[, ,]>(move.ToString());
                        if (dict != null)
                        {
                            //Send the data to the move checkers

                            Gameplay.Board_temp = dict;
                            var valid = Models.Gameplay.accept_or_reject_move(user); //Fill in player 
                            if (valid)
                            {
                                Status report = new Status();
                                report.Letters = user.Letters;
                                return Ok(report);
                            }
                            else
                            {
                                return Ok("Invalid Move");
                            }
                        }
                        else
                        {
                            return Ok("Something went wrong in deserializing for a board play");
                        }

                    }
                    else if (letter != null)
                    {
                        //The user has chosen to exchange
                        if (letter != null)
                        {
                            //letters stuff here
                            user.ExchangeLetter = letter[0];
                            Gameplay.exchange_move(user);
                            return Ok("You decided to turn in: "+string.Join("",letter) + "your new letters are:" + string.Join("", user.Letters));
                        }
                        else
                        {
                            return Ok("Something went wrong in deserializing for an exchange move");
                        }

                    }
                    else
                    {
                        //Player has passed
                        Gameplay.pass();
                        return Ok("You have passed your turn.");
                    }


                   

                    //Add code to return Game data and Player data
                }
                else
                {
                    return Ok("User Auth Failed");
                }
            }
            catch(Exception e)
            {
                return Ok(e.Message);
            }

           
        }
        

    }

    public class Status
    {
        public string[,,] Board = Gameplay.Board;
        public List<string> Letters { get; set; }

        public int Turn = Gameplay.Player_Turn;
        public int Score;

    }
}
