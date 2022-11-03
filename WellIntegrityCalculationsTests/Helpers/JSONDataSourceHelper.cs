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
