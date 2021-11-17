using Newtonsoft.Json;
using Overwolf.Models.BehaviorChart;
using Overwolf.Models.PlayerData;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Overwolf.Core
{
    internal sealed class CoreMain
    {
        private readonly HttpClient StratzApi = new() { BaseAddress = new Uri("https://api.stratz.com/api/v1/") };

        private readonly int[] PlaerIds;

        public CoreMain(Context context)
        {
            //PlaerIds = plaerIds;
            PlaerIds = new int[] {
                //163737155,
                //163737156,
                //329157906
                169129246
            };
            //GetPlayerData();
        }

        public void Dispose()
        {

        }

        private void GetPlayerData()
        {
            Task.Run(async () =>
            {
                foreach (var playerId in PlaerIds)
                {
                    var response = await StratzApi.GetAsync($"Player/{playerId}");
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.IsSuccessStatusCode);

                        var jsonString = await response.Content.ReadAsStringAsync();
                        var playerData = JsonConvert.DeserializeObject<PlayerData>(jsonString);
                        Console.WriteLine($"SteamId: {playerData.SteamAccounId}\tIsAnonymous: {playerData.SteamAccount.IsAnonymous}");
                        if (!playerData.SteamAccount.IsAnonymous)
                        {
                            response = await StratzApi.GetAsync($"/api/v1/Player/{playerId}/behaviorChart?lobbyType=7&startDateTime={DateTimeOffset.Now.ToUnixTimeSeconds() - 2629743}&take=250");
                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine(response.IsSuccessStatusCode);

                                jsonString = await response.Content.ReadAsStringAsync();
                                var behaviorChart = JsonConvert.DeserializeObject<BehaviorChart>(jsonString);
                                foreach (var hero in behaviorChart.Heroes)
                                {
                                    Console.WriteLine($"Hero: {hero.HeroId} MatchCount: {hero.MatchCount}");
                                }
                                //Console.WriteLine(playerData.SteamAccounId);

                            }
                        }
                    }

                    //await Task.Delay(100);
                }
            });
        }
    }
}