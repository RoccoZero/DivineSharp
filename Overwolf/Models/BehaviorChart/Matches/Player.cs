using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overwolf.Models.BehaviorChart
{
    internal sealed class Player
    {

        [JsonProperty("playerSlot")]
        public int PlayerSlot { get; set; }

        [JsonProperty("heroId")]
        public int HeroId { get; set; }

        [JsonProperty("award")]
        public int Award { get; set; }

        [JsonProperty("isVictory")]
        public int IsVictory { get; set; }

        [JsonProperty("imp")]
        public int Imp { get; set; }

    }
}
