using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GPT4
{
    public class APIresponse
    {
        public string content { get; set; }
    }

    public class Program
    {
        private const string ConfigFilePath = "config.json";
        private static string userId;

        public static async Task Main(string[] args)
        {
            InitializeUserId();

            Console.WriteLine("Hello! What's your name?");
            string userName = Console.ReadLine();
            Console.WriteLine($"Nice to meet you, {userName}!");

            Console.WriteLine("My name is \u001b[1;35mZIE AI\u001b[0m. Let's chat!"); 

            await ChatLoop();
        }

        private static void InitializeUserId()
        {
            if (File.Exists(ConfigFilePath))
            {
                userId = File.ReadAllText(ConfigFilePath);
            }
            else
            {
                userId = Guid.NewGuid().ToString();
                File.WriteAllText(ConfigFilePath, userId);
            }
        }

        public static async Task ChatLoop()
        {
            bool exitRequested = false;

            do
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("You: ");
                Console.ResetColor();
                string query = Console.ReadLine();

                if (query.ToLower() == "/exit")
                {
                    exitRequested = true;
                    continue; 
                }

                var response = await SendQuery(query);

                // Print the AI's response
                Console.ForegroundColor = ConsoleColor.Magenta; // Magenta color for AI response
                Console.Write("ZIE AI: ");
                Console.ResetColor();
                Console.WriteLine(response.content);
            } while (!exitRequested);
        }

        public static async Task<APIresponse> SendQuery(string query)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://gemini.ea-sy.tech/v2/chat/completion"),
                Headers =
                {
                    { "User-Agent", "insomnia/8.6.1" },
                },
                Content = new StringContent($" {{\n\t \"message\":\"{query}\",\n\t \"userId\":\"{userId}\"\n  }}")
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<APIresponse>(body);
            }
        }
    }
}
