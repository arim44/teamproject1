
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;


namespace ExcelToJsonPipeline
{
    public static class JsonLoader
    {
        public static List<T> LoadJsonList<T>(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"Cannot find file: {jsonFilePath}");
            }

            string json = File.ReadAllText(jsonFilePath);
            var result = JsonConvert.DeserializeObject<List<T>>(json);
            if (result == null)
            {
                throw new InvalidDataException("Failed to deserialize JSON into the specified type.");
            }
            return result;
        }

        public static T LoadJson<T>(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"Cannot find file: {jsonFilePath}");
            }

            string json = File.ReadAllText(jsonFilePath);
            var result = JsonConvert.DeserializeObject<T>(json);
            if (result == null)
            {
                throw new InvalidDataException("Failed to deserialize JSON into the specified type.");
            }
            return result;
        }
    }


}
