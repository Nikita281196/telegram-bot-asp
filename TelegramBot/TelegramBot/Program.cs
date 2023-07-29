using System.Configuration;

namespace TelegramBot
{
    internal class Program
    {
        static void Main()
        {
            var token = ConfigurationManager.AppSettings["Token"];

            var bot = new BotService(token);
            BotService.Start();
        }
    }
}