using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using MySettingsServer.Abstractions;
using MyYamlSettingsParser;

namespace MySettingsServer.Services.SettingsManagers
{
    public class AzureBlobSettingsManager : ISettingsFileParser
    {
        private readonly string _azureConnectionString;
        private readonly string _azureContainerName;
        private readonly string _azureContainerKeyValueFile;

        public AzureBlobSettingsManager(string azureConnectionString, string azureContainerName, string azureContainerKeyValueFile)
        {
            _azureConnectionString = azureConnectionString;
            _azureContainerName = azureContainerName;
            _azureContainerKeyValueFile = azureContainerKeyValueFile;
        }


        public async Task<IDictionary<string, string>> GetSecrets()
        {
            var cloudAccount = CloudStorageAccount.Parse(_azureConnectionString);
            var yamlKeyValue = await ReadYamlBlobAsync(cloudAccount);
            var keyValues = ISettingsFileParser.GetKeyValues(yamlKeyValue.ParseYaml());
            return keyValues;
        }
        
        private async Task<byte[]> ReadYamlBlobAsync(CloudStorageAccount cloudAccount)
        {
            var container = await cloudAccount.GetBlockBlobReferenceAsync(_azureContainerName);
            var blob = container.GetBlobReference(_azureContainerKeyValueFile);
            var stream = new MemoryStream();

            await blob.DownloadToStreamAsync(stream);
            return stream.ToArray();
        }
    }
}