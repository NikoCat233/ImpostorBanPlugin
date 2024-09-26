using System.Text.Json;
using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Serilog;

namespace ImpostorBanPlugin
{
    [ImpostorPlugin("niko233.BanPlugin")]
    public class BanPlugin : PluginBase
    {
        private readonly ILogger _logger = Log.Logger;
        private readonly IEventManager _eventManager;
        private Config _config;
        private IDisposable _unregister;
        private string ConfigDirectoryPath = Path.Combine(Environment.CurrentDirectory, "config");
        private const string ConfigPath = "banconfig.json";
        private EacController.EACFunctions _eacFunctions;
        private Timer _eacSyncTimer;

        public BanPlugin(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public override ValueTask EnableAsync()
        {
            _config = LoadConfig();
            _eacFunctions = new EacController.EACFunctions();

            if (_config.UseEac)
            {
                _eacSyncTimer = new Timer(async _ => await _eacFunctions.UpdateEACListFromURLAsync("impostor_ban_plugin"), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            }

            _logger.Information("Ban plugin is enabled.");
            _unregister = _eventManager.RegisterListener(new BanPluginEventListener(_logger, _config, _eacFunctions));
            return default;
        }

        public override ValueTask DisableAsync()
        {
            _logger.Information("Ban plugin is disabled.");
            _unregister.Dispose();
            _eacSyncTimer?.Dispose();
            return default;
        }

        public Config LoadConfig()
        {
            if (!Directory.Exists(ConfigDirectoryPath))
            {
                Directory.CreateDirectory(ConfigDirectoryPath);
            }

            string config_path = Path.Combine(ConfigDirectoryPath, ConfigPath);
            Config config;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            if (File.Exists(config_path))
            {
                config = JsonSerializer.Deserialize<Config>(File.ReadAllText(config_path));
            }
            else
            {
                config = new Config();
                File.WriteAllText(config_path, JsonSerializer.Serialize(config, options));
            }

            // Ensure banlist.txt exists at the specified location
            string banListPath;
            if (string.IsNullOrEmpty(config?.BanListLocation))
            {
                banListPath = Path.Combine(Environment.CurrentDirectory, "banlist.txt");
            }
            else
            {
                banListPath = config.BanListLocation;
            }

            if (!File.Exists(banListPath))
            {
                File.WriteAllText(banListPath, string.Empty);
            }

            return config;
        }
    }
}