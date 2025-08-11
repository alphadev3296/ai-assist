using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ai_assist
{
    public static class JsonConfig<T> where T : class, new()
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        /// <summary>
        /// Load config of type T from a JSON file.
        /// Returns a new T() if file missing or on error.
        /// </summary>
        public static async Task<T> LoadAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new T();
            }

            try
            {
                using FileStream fs = File.OpenRead(filePath);
                T? config = await JsonSerializer.DeserializeAsync<T>(fs);
                return config ?? new T();
            }
            catch
            {
                // Could log error
                return new T();
            }
        }

        /// <summary>
        /// Save config object to JSON file.
        /// </summary>
        public static async Task SaveAsync(string filePath, T config)
        {
            try
            {
                using FileStream fs = File.Create(filePath);
                await JsonSerializer.SerializeAsync(fs, config, _jsonOptions);
            }
            catch
            {
                // Could log or rethrow
                throw;
            }
        }
    }

}
