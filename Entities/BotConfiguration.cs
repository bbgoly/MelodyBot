using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace MelodyBot.Entities
{
    public struct BotConfiguration
    {
        [JsonProperty("Token")]
        public string Token { get; private set; }

        [JsonProperty("DefaultPrefix")]
        public string DefaultPrefix { get; private set; }

        [JsonProperty("DefaultWelcomeMessage")]
        public string DefaultWelcomeMessage { get; private set; }

        [JsonProperty("DefaultLeaveMessage")]
        public string DefaultLeaveMessage { get; private set; }

        private const string CONFIG_PATH = @"..\..\..\config.json";

        public static BotConfiguration Setup()
        {
            try
            {
                using (FileStream stream = new FileStream(CONFIG_PATH, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, new UTF8Encoding(false)))
                    {
                        return JsonConvert.DeserializeObject<BotConfiguration>(reader.ReadToEnd());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured when setting up config.json - {e.Message}");
                throw e;
            }
        }
    }
}
