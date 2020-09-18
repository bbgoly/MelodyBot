using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MelodyBot.IO
{
    public class FileIO
    {
        private static readonly Dictionary<string, JToken> CachedJson = new Dictionary<string, JToken>();

        public async static Task<T> ReadFileAsync<T>(string path, string key, string pathToKey) where T : ICollection<T>
        {
            if (!CachedJson.ContainsKey(key))
            {
                using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, new UTF8Encoding(false)))
                    {
                        JObject obj = JObject.Parse(await reader.ReadToEndAsync());
                        JToken token = obj.SelectToken(pathToKey);
                        CachedJson.Add(key, token);
                        //return token.ToObject<T>();JsonConvert.DeserializeObject<T>(json);
                    }
                }
            }
            return CachedJson[key].ToObject<T>();
        }

        public async static Task WriteFileAsync<T>(string path, T[] data) where T : ICollection<T>
        {

            await Task.CompletedTask;
        }
    }
}