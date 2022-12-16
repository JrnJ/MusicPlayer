using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MusicPlayer.Classes
{
    internal static class FileHandler<T>
    {
        internal static async Task<T> GetJSON(string path)
        {
            try
            {
                FileStream stream = File.OpenRead(path);
                T result = await JsonSerializer.DeserializeAsync<T>(stream);
                await stream.DisposeAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
                return default;
            }
        }

        internal static async Task SaveJSON(string path, T obj)
        {
            try
            {
                FileStream stream = File.Create(path);
                await JsonSerializer.SerializeAsync(stream, obj);
                await stream.DisposeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
            }
        }
    }
}
