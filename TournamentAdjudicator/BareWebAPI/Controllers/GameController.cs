using TournamentAdjudicator.Controllers;
using TournamentAdjudicator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;

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

            try
            {
                if (Request.Headers.GetValues("Hash").ElementAt(0).ToString().Equals(user.Hash))
                {
                    return Ok(Gameplay.Board);

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
                    return Ok(Request.Content);

                    //Add code to return Game data and Player data
                }
            }
            catch 
            {
                Ok("User Auth Failed");
            }

            return Ok("User Auth Failed");
        }
        

    }
}
