using Newtonsoft.Json;

namespace Overwolf.Models.PlayerData
{
    internal sealed class SteamAccount
    {
        [JsonProperty("steamId")]
        public int SteamId { get; set; }

        [JsonProperty("isAnonymous")]
        public bool IsAnonymous { get; set; }

    }
}