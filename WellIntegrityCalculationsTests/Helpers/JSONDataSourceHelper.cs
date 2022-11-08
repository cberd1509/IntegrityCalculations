using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WellIntegrityCalculations.Models;

namespace WellIntegrityCalculationsTests.Helpers
{
    public static class JSONDataSourceHelper
    {
        /// <summary>
        /// Retrieves a File from the TestDataSources folder and returns it as the specified type
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="fileName">Filename with extension of the JSON File to be loaded</param>
        /// <returns>A serialized object from the JSON File</returns>
        public static T LoadJSONToObject<T>(string fileName)
        {
            try
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"TestDataSources/{fileName}");
                var json = File.ReadAllText(filePath);
               
                T deserializedObj = JsonSerializer.Deserialize<T>(json)!;
                return deserializedObj!;
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}
