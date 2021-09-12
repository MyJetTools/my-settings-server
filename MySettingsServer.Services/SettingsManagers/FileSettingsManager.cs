using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MySettingsServer.Abstractions;
using MyYamlSettingsParser;

namespace MySettingsServer.Services.SettingsManagers
{
    public class FileSettingsManager : ISettingsFileParser
    {
        private readonly string _pathToSecretsFile;

        public FileSettingsManager(string pathToSecretsFile)
        {
            _pathToSecretsFile = pathToSecretsFile;
        }
        
        public async Task<IDictionary<string, string>> GetSecrets()
        {
            var fileBytes = await File.ReadAllBytesAsync(_pathToSecretsFile);
            var keyValues = ISettingsFileParser.GetKeyValues(fileBytes.ParseYaml());

            return keyValues;
        }
    }
}