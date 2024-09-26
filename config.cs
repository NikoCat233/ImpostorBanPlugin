namespace ImpostorBanPlugin
{
    public class Config
    {
        public bool UseEac { get; set; } = false;
        public string EacBanMessage { get; set; } = "You are found on the TEN EAC cloud.\nYou are banned from this server.\n你因为在TEN EAC云上而被封禁。";
        public string CustomBanMessage { get; set; } = "You are on the server's ban list.\nYou are banned from this server.\n你已被此服务器封禁。";
        public string BanListLocation { get; set; } = "";
    }
}