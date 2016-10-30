using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Threading.Tasks;

namespace TournamentPlayerExample
{
    public class Payload
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public string[] Letters { get; set; }
        public string[,,] Board { get; set; }
    }

    class Program
    {
        static HttpClient client = new HttpClient();

      /*  static void ShowProduct(Product product)
        {
            Console.WriteLine($"Name: {product.Name}\tPrice: {product.Price}\tCategory: {product.Category}");
        }
        */
        static async Task<Uri> CreatePayloadAsync(Payload payload)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/users", payload);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        static async Task<Payload> GetPayloadAsync(string path)
        {
            Payload payload = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                payload = await response.Content.ReadAsAsync<Payload>();
            }
            return payload;
        }

        static async Task<Payload> UpdateProductAsync(Payload payload)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/game/{payload.Id}", payload);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            payload = await response.Content.ReadAsAsync<Payload>();
            return payload;
        }

        static async Task<HttpStatusCode> DeleteProductAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/products/{id}");
            return response.StatusCode;
        }

        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:62027/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Create a new product
                Payload payload = new Payload();

                var url = await CreatePayloadAsync(payload);
                Console.WriteLine($"Created at {url}");

                // Get the product
                payload = await GetPayloadAsync(url.PathAndQuery);
                //ShowProduct(product);

                // Update the product
               // Console.WriteLine("Updating price...");
                //payload.Price = 80;
                await UpdateProductAsync(payload);

                // Get the updated product
                payload = await GetPayloadAsync(url.PathAndQuery);
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
}