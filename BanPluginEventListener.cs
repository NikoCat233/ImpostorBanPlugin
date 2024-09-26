using Impostor.Api.Events;
using Serilog;

namespace ImpostorBanPlugin
{
    public class BanPluginEventListener : IEventListener
    {
        private readonly ILogger _logger;
        private Config _config;
        private EacController.EACFunctions _eacFunctions;

        public BanPluginEventListener(ILogger logger, Config config, EacController.EACFunctions eacFunctions)
        {
            _logger = logger;
            _config = config;
            _eacFunctions = eacFunctions;
        }

        [EventListener]
        public async void OnPlayerJoined(IGamePlayerJoinedEvent e)
        {
            var player = e.Player;

            if (player.Client.Puid == string.Empty)
                return;

            string puid = player.Client.Puid;
            string friendcode = player.Client.FriendCode;

            if (_eacFunctions.CheckHashPUIDExists(puid) || _eacFunctions.CheckFriendCodeExists(friendcode))
            {
                _logger.Warning("{0} - Player {1} [{2}] is banned by EAC.", e.Game.Code, player.Client.Name, player.Client.Id);

                if (player.Client != null)
                {
                    await player.Client.DisconnectAsync(Impostor.Api.Innersloth.DisconnectReason.Custom, _config.EacBanMessage);
                }
            }
        }
    }
}
