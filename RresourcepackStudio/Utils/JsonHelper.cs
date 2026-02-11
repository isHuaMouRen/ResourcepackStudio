using Newtonsoft.Json;
using System.IO;

namespace RresourcepackStudio.Utils
{
    public static class JsonHelper
    {
        public static void WriteJson<T>(string path, T jsonObj)
        {
            using var streamWriter = new StreamWriter(path);
            using var jsonWriter = new JsonTextWriter(streamWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            new JsonSerializer().Serialize(jsonWriter, jsonObj);
            streamWriter.Flush();
        }

        public static T? ReadJson<T>(string path)
        {
            if (!File.Exists(path))
                return default;

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
