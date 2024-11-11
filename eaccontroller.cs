using System.Text.Json;

namespace ImpostorBanPlugin;

public class EacController
{
    public class EacData
    {
#pragma warning disable IDE1006
        public required string friendcode { get; set; }

        public required string hashPUID { get; set; }

        public required string name { get; set; }

        public required string reason { get; set; }
#pragma warning restore IDE1006
    }

    public class EACList
    {
        public List<EacData>? EACDataList { get; set; } = [];
    }

    public class EACFunctions
    {
        private readonly ILogger<BanPlugin> _logger;
        private string EndPointURL = "https://tohre.niko233.me/eac?token=";
        public static EACList? _eacList = new();

        public EACFunctions(ILogger<BanPlugin> logger)
        {
            _logger = logger;
        }

        public async Task UpdateEACListFromURLAsync(string token)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                string url = EndPointURL + token;
                string json = await client.GetStringAsync(url);
                List<EacData> eacDataList = JsonSerializer.Deserialize<List<EacData>>(json);
                _eacList = new EACList { EACDataList = eacDataList };
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError("The request timed out while retrieving EAC data.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while retrieving EAC data: " + ex.Message);
            }
        }

        public bool CheckHashPUIDExists(string hashPUID)
        {
            if (_eacList == null)
            {
                _logger.LogWarning("EACList is null.");
                return false;
            }

            foreach (var eacData in _eacList.EACDataList)
            {
                if (eacData.hashPUID != null && eacData.hashPUID.Equals(hashPUID, StringComparison.CurrentCultureIgnoreCase))
                {
                    _logger.LogInformation("HashPUID {0} exists in EACList. Reason {1}", hashPUID, eacData.reason);
                    return true;
                }
            }

            return false;
        }

        public bool CheckFriendCodeExists(string friendcode)
        {
            if (_eacList == null)
            {
                _logger.LogWarning("EACList is null.");
                return false;
            }

            foreach (var eacData in _eacList.EACDataList)
            {
                if (eacData.friendcode != null && eacData.friendcode.Equals(friendcode, StringComparison.CurrentCultureIgnoreCase))
                {
                    _logger.LogInformation("HashPUID {0} exists in EACList. Reason {1}", friendcode, eacData.reason);
                    return true;
                }
            }

            return false;
        }
    }
}