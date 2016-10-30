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

        /*  static void ShowProduct(Product product)
          {
              Console.WriteLine($"Name: {product.Name}\tPrice: {product.Price}\tCategory: {product.Category}");
          }
          */
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

        static async Task<Payload> GetGamestate()
        {

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(client.BaseAddress.AbsoluteUri+$"api/game/{myPayload.ID}"),
                Method = HttpMethod.Get,
            };

            client.DefaultRequestHeaders.Add("Hash", myPayload.Hash);


            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                myPayload = await response.Content.ReadAsAsync<Payload>();
            }
            return myPayload;
        }

        static async Task<Payload> SendMove()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(client.BaseAddress.AbsoluteUri + $"api/game/{myPayload.ID}"),
                Method = HttpMethod.Get,
            };
            
            client.DefaultRequestHeaders.Add("Hash", myPayload.Hash);
            client.DefaultRequestHeaders.Add("Board", myPayload.Board.ToString());


            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                myPayload = await response.Content.ReadAsAsync<Payload>();
            }
            return myPayload;


            

            // Deserialize the updated product from the response body.

        }

        static async Task<HttpStatusCode> DeleteProductAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/products/{id}");
            return response.StatusCode;
        }
        public static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:62027/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                //join the game and get ID and Hash
                await JoinGame();
                Console.WriteLine("I am user number {0} with hash code {1}", myPayload.ID, myPayload.Hash);
                await GetGamestate();
                Console.WriteLine("I got the board state and letters");
                while (myPayload.Turn != myPayload.ID)
                {
                    await GetGamestate();
                    Thread.Sleep(100);
                }
                await SendMove();

                // Get the product
                //payload = await GetPayloadAsync(url.PathAndQuery);
                //ShowProduct(product);

                // Update the product
                // Console.WriteLine("Updating price...");
                //payload.Price = 80;
                //await UpdatePayloadAsync(payload);

                // Get the updated product
                //payload = await GetPayloadAsync(url.PathAndQuery);
                // ShowProduct(payload);

                // Delete the product
                //var statusCode = await DeleteProductAsync(payload.Id);
                // Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

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