using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overwolf.Models.BehaviorChart
{
    internal sealed class BehaviorChart
    {
        [JsonProperty("matchCount")]
        public int MatchCount { get; set; }

        [JsonProperty("winCount")]
        public int WinCount { get; set; }

        [JsonProperty("heroes")]
        public Hero[] Heroes { get; set; }

        [JsonProperty("matches")]
        public Match[] Matches { get; set; }
    }
}
