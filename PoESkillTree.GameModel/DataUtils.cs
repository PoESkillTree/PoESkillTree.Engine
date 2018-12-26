﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PoESkillTree.GameModel
{
    /// <summary>
    /// Deserializes data files and contains related constants.
    /// </summary>
    public static class DataUtils
    {
        private const string ResourceRoot = "PoESkillTree.GameModel.Data.";
        public const string RePoEDataUrl = "https://raw.githubusercontent.com/brather1ng/RePoE/master/data/";
        public const string RePoEFileSuffix = ".min.json";

        /// <summary>
        /// Asynchronously deserializes the data file with the given name (without extension).
        /// </summary>
        /// <typeparam name="T">type to deserialize the json as</typeparam>
        /// <param name="fileName">the data file to deserialize</param>
        /// <returns>a task returning the deserialized object</returns>
        public static Task<T> LoadRePoEAsync<T>(string fileName)
            => LoadJsonAsync<T>(RePoEFileToResource(fileName));

        public static Task<string> LoadRePoEAsync(string fileName)
            => LoadTextAsync(RePoEFileToResource(fileName));

        private static string RePoEFileToResource(string fileName)
            => "RePoE." + fileName.Replace("/", ".") + RePoEFileSuffix;

        public static async Task<T> LoadJsonAsync<T>(string fileName)
        {
            var text = await LoadTextAsync(fileName).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(text);
        }

        public static async Task<T> LoadXmlAsync<T>(string fileName)
        {
            var xmlString = await LoadTextAsync(fileName).ConfigureAwait(false);
            using (var reader = new StringReader(xmlString))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T) serializer.Deserialize(reader);
            }
        }

        private static async Task<string> LoadTextAsync(string resourceName)
        {
            var name = ResourceRoot + resourceName;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                if (stream is null)
                    throw new ArgumentException("Unknown resource " + name);

                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }
        }
    }
}