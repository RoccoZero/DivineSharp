using Divine.Entity.Entities.Units.Heroes.Components;
using Newtonsoft.Json;

namespace Overwolf.Models.BehaviorChart
{
    internal sealed class Hero
    {

        [JsonProperty("heroId")]
        public HeroId HeroId { get; set; }

        [JsonProperty("matchCount")]
        public int MatchCount { get; set; }

        [JsonProperty("winCount")]
        public int WinCount { get; set; }

        [JsonProperty("avgImp")]
        public int AvgImp { get; set; }

        [JsonProperty("mvpCount")]
        public int MvpCount { get; set; }

        [JsonProperty("topCoreCount")]
        public int TopCoreCount { get; set; }

        [JsonProperty("topSupportCount")]
        public int TopSupportCount { get; set; }

    }
}
