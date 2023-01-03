using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace PasswordProject
{
    public static class FileManager
    {
        public static void CreateJson(string path)
        {
            try
            {
                if (!File.Exists(path) || String.IsNullOrEmpty(File.ReadAllText(path)) || File.ReadAllText(path) == "[]")
                {
                    using (var fs = File.Create(path)) { }
                    File.WriteAllText(path, @"[{""Username"":""Admin"",""Email"":""admin@gmail.com"",""Password"":""S"",""Access"":""Admin""}]");

                }
                
            }
            catch (IOException)
            {
                throw new IOException();
                
            }
           
        } //sätt in fler try catch?

        public static List<T> GetJson<T>(string path)
        {
            var jsonData = File.ReadAllText(path);

            var lists = JsonSerializer.Deserialize<List<T>>(jsonData);

            return lists;
        }
        public static void UpdateJson<T>(string path, T data)
        {
            var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions() { WriteIndented = true});

            File.WriteAllText(path, jsonData);
        }
    }
}