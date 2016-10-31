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
            Status report = new Status();
            report.Letters = user.Letters;
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

                    if (move != null)
                    {
                        var dict = JsonConvert.DeserializeObject<string[, ,]>(move.ToString());
                        if (dict != null)
                        {
                            //Send the data to the move checkers

                            Gameplay.Board_temp = dict;
                            var valid = Models.Gameplay.accept_or_reject_move(user); //Fill in player 
                            if (valid)
                            {
                                return Ok("Move Valid");
                            }
                            else
                            {
                                return Ok("Invalid Move");
                            }
                        }
                        else
                        {
                            return Ok("Something went wrong in deserializing");
                        }

                    }
                    else if (exchange != null)
                    {
                        //The user has chosen to pass
                        var letter = JsonConvert.DeserializeObject<string[]>(exchange.ToString());
                        if (letter != null)
                        {
                            //letters stuff here
                            return Ok("You decided to turn in: "+string.Join("",letter));
                        }
                        else
                        {
                            return Ok("Something went wrong in deserializing");
                        }

                    }
                    else
                    {
                        return Ok("You have passed.");
                    }


                   

                    //Add code to return Game data and Player data
                }
                else
                {
                    return Ok("User Auth Failed");
                }
            }
            catch
            {
                return Ok("Caught");
            }

           
        }
        

    }

    public class Status
    {
        public string[,,] Board = Gameplay.Board;
        public List<string> Letters { get; set; }

        public int Turn = Gameplay.Player_Turn;

    }
}
