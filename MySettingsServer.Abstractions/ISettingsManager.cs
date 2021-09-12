using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyYamlSettingsParser;

namespace MySettingsServer.Abstractions
{
    public interface ISettingsManager
    {
        Task<string> SetupSecretsValues(IEnumerable<YamlLine> yamlLines);
    }

    public class SettingsReader : ISettingsManager
    {
        private ISettingsFileParser _settingsManager;
        
        public SettingsReader(ISettingsFileParser settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public async Task<string> SetupSecretsValues(IEnumerable<YamlLine> yamlLines)
        {
            var secrets = await _settingsManager.GetSecrets();
            var responseYaml = ApplyYaml(yamlLines, secrets);
            return responseYaml;

        }
        
        private static string ApplyYaml(IEnumerable<YamlLine> yamlLines, IDictionary<string, string> keyValues)
        {

            var result = new StringBuilder();

            foreach (var yamlLine in yamlLines)
            {
                if (yamlLine.Keys.Length > 1)
                    result.Append(new string(' ', yamlLine.Keys.Length - 1));


                var currentKey = yamlLine.Keys[^1];

                result.Append(currentKey);

                if (string.IsNullOrEmpty(yamlLine.Value))
                {
                    result.AppendLine(!currentKey.StartsWith('-') ? ":" : "");
                }
                else
                    result.AppendLine(": " + yamlLine.Value);
            }

            var resultYaml = result.ToString();
            
            foreach (var (key, value) in keyValues)
                resultYaml = resultYaml.Replace("${" + key + "}", value);

            return resultYaml;
        }

    }
}