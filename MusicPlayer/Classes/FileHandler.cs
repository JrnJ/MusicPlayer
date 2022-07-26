using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Classes
{
    public static class FileHandler<T>
    {
        /// <summary>
        /// Gets JSON from a file
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>T</returns>
        public static async Task<T> GetJSON(string path)
        {
            try
            {
                string json = await File.ReadAllTextAsync(path);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
                return default;
            }
        }

        /// <summary>
        /// Saves JSON to a file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="obj">T to save</param>
        public static async void SaveJSON(string path, T obj)
        {
            try
            {
                string json = JsonConvert.SerializeObject(obj);
                await File.WriteAllTextAsync(path, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
            }
        }
    }
}
