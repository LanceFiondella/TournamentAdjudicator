using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Threading;
using System.Threading.Tasks; 
using Newtonsoft.Json;

namespace TournamentPlayerExample
{
    //Payload class contains all data sent to and received from the server
    public class Payload
    {
        public int ID { get; set; }
        public string Hash { get; set; }
        public string[] Letters { get; set; }
        public string[,,] Board { get; set; }
        public int Turn { get; set; }//1,2,3,4
    }
    //Move class is only used for packaging a move command to send to the server
    public class Move
    {
        public string Board { get; set; }
        public string Letters { get; set; }
        public Move(string board,string letters)
        {
            Board = board;
            Letters = letters;

        }
    }

    //primary class used for communication with the server
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
            //initializing the variables to be sent to derver
            Payload tempPayload = new Payload();
            client.DefaultRequestHeaders.Clear();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(client.BaseAddress.AbsoluteUri+"api/game/"+myPayload.ID),
                Method = HttpMethod.Get,
            };

            //only header needed to be added is the hash
            client.DefaultRequestHeaders.Add("Hash", myPayload.Hash);

            //sending the get message
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {//if the message response was a success
                tempPayload = await response.Content.ReadAsAsync<Payload>();
            }

            //safely update the payload
            updatePayload(tempPayload, myPayload);

            return myPayload;
        }

        //helper function to update the payload variable if the data exists
        //otherwise if data does not exist, it will overwrite it with 0.
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
            //initialization
            Payload tempPayload = new Payload();
            client.DefaultRequestHeaders.Clear();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(client.BaseAddress.AbsoluteUri + "api/game/" + myPayload.ID),
                Method = HttpMethod.Post,
            };

            //adds data to header
            string board = JsonConvert.SerializeObject(myPayload.Board);
            ///string letters = JsonConvert.SerializeObject(myPayload.Letters);
            Move move = new Move(board, null);
            string jmove = JsonConvert.SerializeObject(move);
            client.DefaultRequestHeaders.Add("Hash", myPayload.Hash);
            client.DefaultRequestHeaders.Add("Move", jmove);

            //sends data to server
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                tempPayload = await response.Content.ReadAsAsync<Payload>();
            }

            //update the payload variable
            updatePayload(tempPayload, myPayload);
            return myPayload;

        }

        //send letter exchange to server 
        static async Task<Payload> SendExchangeLetters()
        {
            //initialization
            Payload tempPayload = new Payload();
            client.DefaultRequestHeaders.Clear();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(client.BaseAddress.AbsoluteUri + "api/game/" + myPayload.ID),
                Method = HttpMethod.Post,
            };

            //adds data to header
            //string board = JsonConvert.SerializeObject(myPayload.Board);
            string letters = JsonConvert.SerializeObject(myPayload.Letters);
            Move move = new Move(null, letters);
            string jmove = JsonConvert.SerializeObject(move);
            client.DefaultRequestHeaders.Add("Hash", myPayload.Hash);
            client.DefaultRequestHeaders.Add("Move", jmove);

            //sends data to server
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                tempPayload = await response.Content.ReadAsAsync<Payload>();
            }

            //update the payload variable
            updatePayload(tempPayload, myPayload);
            return myPayload;

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
                while (true)
                {
                    //get board state
                    await GetGamestate();

                    //wait until its your turn
                    while (myPayload.Turn != myPayload.ID)
                    {
                        await GetGamestate();
                        Thread.Sleep(100);
                    }
                    string lettersString = "";
                    foreach (string letter in myPayload.Letters)
                    {
                        lettersString += letter;
                    }
                    for (int r = 0; r < 10; r++)
                    {
                        for (int c = 0; c < 10; c++)
                        {
                            Console.Write("{0,-3}", myPayload.Board[0, r, c] == null ? "~" : myPayload.Board[0, r, c]);
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("I got letters: " + lettersString);


                    Console.Write("Enter move: ");
                    string move = "";

                    string[] moves = new string[7];
                    int i = 0, j = 0;

                    do
                    {
                        move = Console.ReadLine();
                        moves[i++] = move;
                    } while (move != "");

                    string[] letNumNum = new string[3];
                    foreach (string m in moves)
                    {
                        if (m == "") break;

                        letNumNum = m.Split(' ');
                        myPayload.Board[0, Int32.Parse(letNumNum[1]), Int32.Parse(letNumNum[2])] = letNumNum[0];
                        myPayload.Board[1, Int32.Parse(letNumNum[1]), Int32.Parse(letNumNum[2])] = (Int32.Parse(myPayload.Board[1, Int32.Parse(letNumNum[1]), Int32.Parse(letNumNum[2])].ToString()) + 1).ToString();
                        Console.WriteLine(letNumNum[0]);

                    }
                    /*
                                    foreach (char letter in move)
                                    {
                                        myPayload.Board[0, 4, i] = letter.ToString();
                                        myPayload.Board[1, 4, i] = (Int32.Parse(myPayload.Board[1, 4, i].ToString())+1).ToString();//I AM REFUSING TO COMMENT THIS LINE
                                        i++;
                                    }
                                    myPayload.Board[0, 1, 0] = "C";
                                    myPayload.Board[0, 2, 0] = "A";
                                    myPayload.Board[0, 3, 0] = "T";*/
                    //make a move
                    await SendMove();
                }

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