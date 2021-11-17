using Newtonsoft.Json;

namespace Overwolf.Models.PlayerData
{
    internal sealed class PlayerData
    {
        [JsonProperty("steamAccountId")]
        public int SteamAccounId { get; set; }

        [JsonProperty("steamAccount")]
        public SteamAccount SteamAccount { get; set; }
    }
}