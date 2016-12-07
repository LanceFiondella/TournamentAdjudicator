using TournamentAdjudicator.Models;
using TournamentAdjudicator.Controllers;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace TournamentAdjudicator
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 10000;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //Starts the game after 10 seconds if 2 or more players in game.
            if (!Gameplay.Game_Started && UserController.Players != null && UserController.Players.Count > 1)
            {
                Gameplay.Game_Started = true;
                Gameplay.initalize_bag();
                Gameplay.initial_draw();

            }
        }
    }
}
