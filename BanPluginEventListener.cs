using Impostor.Api.Events;
using Microsoft.Extensions.Logging;

namespace ImpostorBanPlugin
{
    public class BanPluginEventListener :IEventListener
    {
        private readonly ILogger<BanPlugin> _logger;

        public BanPluginEventListener(ILogger<BanPlugin> logger)
        {
            _logger = logger;
        }

        [EventListener]
        public void OnPlayerJoined(IGamePlayerJoinedEvent e)
        {
            var player = e.Player;

            if (player.Client.Puid == string.Empty)
                return;

            string puid = player.Client.Puid;

        }
    }
}
