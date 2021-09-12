using System;
using MySettingsServer.Abstractions;
using MySettingsServer.Services.SettingsManagers;

namespace MySettingsServer
{
    public class SettingsModel
    {
        private const string AzureBlobType = "azure-blob";
        private const string File = "file";

        public string ValuesSource { get; set; } = File;
        
        public string AzureBlobValuesConnectionString { get; set; }
        public string Container { get; set; }
        public string PathToFileSecretKey { get; set; }
        
        public string SettingsKeyValueFile { get; set; }
        public string SettingsTemplateFile { get; set; }

        public ISettingsFileParser GetTargetFileParserByConfig()
        {
            return ValuesSource switch
            {
                AzureBlobType => new AzureBlobSettingsManager(AzureBlobValuesConnectionString, Container,
                    SettingsKeyValueFile),
                File => new FileSettingsManager(PathToFileSecretKey),
                _ => throw new Exception($"Not found provider for settings. Provider: {ValuesSource}")
            };
        }
    }
}