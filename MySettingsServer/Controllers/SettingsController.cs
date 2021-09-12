using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.AspNetCore.Mvc;
using MySettingsServer.Abstractions;
using MyYamlSettingsParser;

namespace MySettingsServer.Controllers
{
    public class SettingsController : ControllerBase
    {
        private SettingsModel _settings;
        private ISettingsManager _settingsManager;

        public SettingsController(SettingsModel settings, ISettingsManager settingsManager)
        {
            _settings = settings;
            _settingsManager = settingsManager;
        }

        [HttpGet("{service}")]
        public async Task<IActionResult> GetSettings(string service)
        {
            var file = (await System.IO.File.ReadAllBytesAsync(_settings.SettingsTemplateFile))
                .ParseYaml().AsReadOnlyList();

            var requestYaml = GetYamlAccordingToRequest(service, file).AsReadOnlyList();

            if (requestYaml.Count == 0)
                return NotFound($"Settings for {service} not found");


            var settingsResult = await _settingsManager.SetupSecretsValues(requestYaml);

            return Content(settingsResult, "application/yaml");
        }
        
        public static IEnumerable<YamlLine> GetYamlAccordingToRequest(string path, IEnumerable<YamlLine> yamlLines)
        {
            var rootKey = string.Empty;
            
            foreach (var yamlLine in yamlLines.Where(itm => itm.Keys[0] != "KeyValue"))
            {
                if (yamlLine.Keys.Length == 1)
                {
                    if (yamlLine.Value == path)
                    {
                        rootKey = yamlLine.Keys[0];
                        yield return new YamlLine(new[]{rootKey}, null);
                    }
                    else
                    {
                        rootKey = string.Empty;
                    }
                    
                    continue;
                }
                
                
                if (yamlLine.Keys[0] == rootKey)
                    yield return yamlLine;
            }
        }
    }
}