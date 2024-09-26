using Impostor.Api.Events;
using Impostor.Api.Games;
using Microsoft.Extensions.Logging;

namespace ImpostorBanPlugin
{
    public class BanPluginEventListener : IEventListener
    {
        private readonly ILogger<BanPlugin> _logger;
        private Config _config;
        private EacController.EACFunctions _eacFunctions;

        public BanPluginEventListener(ILogger<BanPlugin> logger, Config config, EacController.EACFunctions eacFunctions)
        {
            _logger = logger;
            _config = config;
            _eacFunctions = eacFunctions;
        }

        [EventListener]
        public void OnPlayerJoining(IGamePlayerJoiningEvent e)
        {
            var player = e.Player;

            if (player.Client.Puid == string.Empty)
                return;

            string puid = player.Client.Puid;
            string friendcode = player.Client.FriendCode;

            if (_eacFunctions.CheckHashPUIDExists(puid) || _eacFunctions.CheckFriendCodeExists(friendcode))
            {
                _logger.LogWarning("{0} - Player {1} [{2}] is banned by EAC.", e.Game.Code, player.Client?.Name, player.Client?.Id);

                e.JoinResult = GameJoinResult.CreateCustomError(_config.EacBanMessage);
                return;
            }

            if (CheckBanList(puid, friendcode))
            {
                _logger.LogWarning("{0} - Player {1} [{2}] is banned by BanList.", e.Game.Code, player.Client?.Name, player.Client?.Id);

                e.JoinResult = GameJoinResult.CreateCustomError(_config.CustomBanMessage);
                return;
            }
        }

        public bool CheckBanList(string code, string hashedpuid = "")
        {
            bool OnlyCheckPuid = false;
            if (code == "" && hashedpuid != "") OnlyCheckPuid = true;
            else if (code == "") return false;

            try
            {
                if (!File.Exists(_config.BanListLocation)) File.Create(_config.BanListLocation).Close();
                using StreamReader sr = new(_config.BanListLocation);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "") continue;
                    if (!OnlyCheckPuid)
                    {
                        if (line.Contains(code)) return true;
                    }
                    if (line.Contains(hashedpuid)) return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckBanList");
            }
            return false;
        }
    }
}
