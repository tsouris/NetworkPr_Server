using System.Net;
using System.Net.Sockets;


namespace NetworkPr_Server
{
    internal class Program
    {
        private static List<TcpClient> clients = new List<TcpClient>();
        private static List<string> quotes = new List<string>
        {
            "The only way to do great work is to love what you do.",
            "In three words I can sum up everything I've learned about life: it goes on.",
            "The best time to plant a tree was 20 years ago. The second best time is now.",
            "The future belongs to those who believe in the beauty of their dreams.",
            "Success is not final, failure is not fatal: It is the courage to continue that counts.",
            "The only limit to our realization of tomorrow will be our doubts of today.",
            "You are never too old to set another goal or to dream a new dream.",
            "The best revenge is to be unlike him who performed the injustice.",
            "It does not matter how slowly you go as long as you do not stop.",
            "You miss 100% of the shots you don't take."
        };

        static async Task Main()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 8888);
            server.Start();
            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                clients.Add(client);
                LogConnection(client);
                _ = Task.Run(() => HandleClient(client));
            }
        }

        static async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            try
            {
                while (true)
                {
                    string request = await reader.ReadLineAsync();
                    if (request == null)
                        break;

                    if (request == "GET_QUOTE")
                    {
                        string randomQuote = GetRandomQuote();
                        await writer.WriteLineAsync(randomQuote);
                    }
                }
            }
            catch (IOException)
            {
                clients.Remove(client);
                LogDisconnection(client);
            }
        }

        static string GetRandomQuote()
        {
            Random random = new Random();
            return quotes[random.Next(quotes.Count)];
        }

        static void LogConnection(TcpClient client)
        {
            Console.WriteLine($"Client connected from {((IPEndPoint)client.Client.RemoteEndPoint).Address} at {DateTime.Now}");
        }

        static void LogDisconnection(TcpClient client)
        {
            Console.WriteLine($"Client disconnected from {((IPEndPoint)client.Client.RemoteEndPoint).Address} at {DateTime.Now}");
        }
    }
}