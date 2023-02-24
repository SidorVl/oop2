using System.Data;
using Newtonsoft.Json;
using HttpContent = System.Net.Http.HttpContent;

namespace Program;

class Player
{
    public string name { get; set; }
    public string url { get; set; }
}

class Response
{
    public Player[] results { get; set; }
}

class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello!");

        var task = GetUsers();
        task.Wait();

        var players = task.Result;

        var playersWithNameStartsWithC = players.ToList().Where(player => player.name.ToLower().StartsWith("p"));

        var orderedByName = players.ToList();
        orderedByName.Sort((player1, player2) => String.Compare(player1.name, player2.name, StringComparison.Ordinal));

        var playersGroupedByNameFirstLetter = players.ToList().GroupBy((player) => player.name.ToLower()[0]);

        Comparison<Player> sortByNameLength = (Player player1, Player player2) => player1.name.Length - player2.name.Length;

        var orderedByNameLength = players.ToList();
        orderedByNameLength.Sort(sortByNameLength);

        Console.ReadKey();
    }

    public static async Task<Player[]> GetUsers()
    {
        try
        {
            string baseUrl = "https://playapi.co/api/v2/player";

            using HttpClient client = new HttpClient();
            using HttpResponseMessage res = await client.GetAsync(baseUrl);
            using HttpContent content = res.Content;

            var data = await content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response>(data);

            if (response == null)
            {
                throw new NoNullAllowedException();
            }

            return response.results;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return new Player[] { };
        }
    }
}
