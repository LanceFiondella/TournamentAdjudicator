using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TournamentPlayerExample
{
    class Program
    {
        static HttpClient client = new HttpClient();

        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            Console.ReadLine();
        }
    }
}
