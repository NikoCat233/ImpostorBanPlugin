using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace ImpostorBanPlugin
{
    [ImpostorPlugin("niko233.BanPlugin")]
    public class BanPlugin : PluginBase
    {
        private readonly ILogger<BanPlugin> _logger;
        private readonly IEventManager _eventManager;
        private IDisposable _unregister;

        public BanPlugin(ILogger<BanPlugin> logger, IEventManager eventManager)
        {
            _logger = logger;
            _eventManager = eventManager;
        }

        public override ValueTask EnableAsync()
        {
            _logger.LogInformation("Ban plugin is enabled.");
            _unregister = _eventManager.RegisterListener(new BanPluginEventListener(_logger));
            return default;
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("Ban plugin is disabled.");
            _unregister.Dispose();
            return default;
        }
    }
}