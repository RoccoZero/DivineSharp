using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overwolf.Models.BehaviorChart
{
    internal sealed class Match
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("didRadiantWin")]
        public bool DidRadiantWin { get; set; }

        [JsonProperty("endDateTime")]
        public int EndDateTime { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("player")]
        public Player Player { get; set; }

    }
}
