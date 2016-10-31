using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Threading;
using System.Threading.Tasks; 
using Newtonsoft.Json;

namespace TournamentPlayerExample
{
    public class Payload
    {
        public int ID { get; set; }
        public string Hash { get; set; }
        public string[] Letters { get; set; }
        public string[,,] Board { get; set; }
        public int Turn { get; set; }
    }
    public static class GameNetworkCommuncation
    {
        static HttpClient client = new HttpClient();
        static Payload myPayload = new Payload();

        //this function will join the game for the first time and get the ID and Hash
        static async Task<Uri> JoinGame()
        {
            //sends a get request to http://localhost:62027/api/user to join game
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress.AbsoluteUri + "api/user");

            //makes sure the action was performed correctly
            response.EnsureSuccessStatusCode();

            //if it was performed correctly, write the response into the data structure.
            if (response.IsSuccessStatusCode)
            {
                myPayload = await response.Content.ReadAsAsync<Payload>();
            }

            //return location
            return (response.Headers.Location);
        }

        //gets the updated game state from the server and will update the board, letters, and turn
        static async Task<Payload> GetGamestate()
        {
            Payload tempPayload = new Payload();
            client.DefaultRequestHeaders.Clear();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(client.BaseAddress.AbsoluteUri+$"api/game/{myPayload.ID}"),
                Method = HttpMethod.Get,
            };

            client.DefaultRequestHeaders.Add("Hash", myPayload.Hash);

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                tempPayload = await response.Content.ReadAsAsync<Payload>();
            }


            updatePayload(tempPayload, myPayload);

            return myPayload;
        }

        //helper function to update the payload variable if the data exists
        static void updatePayload(Payload a, Payload b)
        {
            if (a.ID != 0)
                b.ID = a.ID;
            if (a.Hash != null)
                b.Hash = a.Hash;
            if (a.Letters != null)
                b.Letters = a.Letters;
            if (a.Board != null)
                b.Board = a.Board;
            if (a.Turn != 0)
                b.Turn = a.Turn;
        }

        //will send the move to the server
        static async Task<Payload> SendMove()
        {
            Payload tempPayload = new Payload();
            client.DefaultRequestHeaders.Clear();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(client.BaseAddress.AbsoluteUri + $"api/game/{myPayload.ID}"),
                Method = HttpMethod.Post,
            };

            string Move = JsonConvert.SerializeObject(myPayload.Board);
            client.DefaultRequestHeaders.Add("Hash", myPayload.Hash);
            client.DefaultRequestHeaders.Add("Move", Move);


            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                tempPayload = await response.Content.ReadAsAsync<Payload>();
            }

            updatePayload(tempPayload, myPayload);

            return myPayload;


            

            // Deserialize the updated product from the response body.

        }

        static async Task<Payload> SendExchangeLetters()
        {
            Payload tempPayload = new Payload();

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(client.BaseAddress.AbsoluteUri + $"api/game/{myPayload.ID}"),
                Method = HttpMethod.Post,
            };

            string Move = JsonConvert.SerializeObject(myPayload.Letters);
            client.DefaultRequestHeaders.Add("Hash", myPayload.Hash);
            client.DefaultRequestHeaders.Add("Move", Move);


            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                tempPayload = await response.Content.ReadAsAsync<Payload>();
            }

            updatePayload(tempPayload, myPayload);

            return myPayload;




            // Deserialize the updated product from the response body.

        }


        public static async Task RunAsync()
        {
            //set up the client to communicate with server
            client.BaseAddress = new Uri("http://localhost:62027/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                //join the game and get ID and Hash
                await JoinGame();
                Console.WriteLine("I am user number {0} with hash code {1}", myPayload.ID, myPayload.Hash);

                //get board state
                await GetGamestate();
               
                //wait until its your turn
                while (myPayload.Turn != myPayload.ID)
                {
                    await GetGamestate();
                    Thread.Sleep(100);
                }
                Console.WriteLine("I got the board state and letters");

                //make a move
                await SendMove();


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
    class Program
    {

        
        static void Main()
        {
            GameNetworkCommuncation.RunAsync().Wait();
        }

       

    }
}