using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyYamlSettingsParser;

namespace MySettingsServer.Abstractions
{
    public interface ISettingsFileParser
    {
        Task<IDictionary<string, string>> GetSecrets();
        
        public static IDictionary<string, string> GetKeyValues(IEnumerable<YamlLine> yamlLines)
        {

            var result = new Dictionary<string, string>();

            foreach (var yamlLine in yamlLines.Where(itm => itm.Keys.Length == 2 && itm.Keys[0] == "KeyValue"))
            {
                var key = yamlLine.Keys[1];
                if (!result.ContainsKey(key))
                    result.Add(key, yamlLine.Value ?? "");
            }

            return result;
        }
    }
}