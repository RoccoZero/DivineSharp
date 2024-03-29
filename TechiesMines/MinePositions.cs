﻿using Divine.Numerics;
using System.Collections.Generic;
using TechiesMines.Enums;

namespace TechiesMines
{
    internal static class MinePositions
    {
        public static Dictionary<Vector3, Mines> Dire = new()
        {
            { new Vector3(1393.5f, -498.75f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-790.3438f, 2633.156f, 256f), Mines.LandMine },
            { new Vector3(1462.313f, 4438.938f, 256f), Mines.LandMine },
            { new Vector3(1512.938f, 3823.313f, 256f), Mines.LandMine },
            { new Vector3(5415.188f, -6411.375f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(5006.063f, -5174.563f, 128f), Mines.LandMine },
            { new Vector3(4647f, -5476f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(4042f, -5513f, 128f), Mines.LandMine },
            { new Vector3(4270.5f, -4784.75f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-4123f, -64f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-2871.219f, 462.9063f, 128.0938f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-2129f, 240f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-3365f, -1560f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-541.125f, -5451.531f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(855.5f, -3665.281f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-2528f, -3552f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-677f, -2833f, 128f), Mines.LandMine },
            { new Vector3(-2019f, -2766f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-1381f, -2062f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-886.7813f, -1497.625f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(767.5f, -1870.281f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(2738.813f, -1627.406f, 256f), Mines.LandMine },
            { new Vector3(2451.188f, -1368.344f, 251.4063f), Mines.LandMine },
            { new Vector3(2855f, -1227f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(3687f, -846f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(4105.5f, -943.75f, 256f), Mines.LandMine },
            { new Vector3(3810f, -1253f, 256f), Mines.LandMine },
            { new Vector3(4038f, -2500f, 0f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(5279f, -1644f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(5235f, -2388f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(5431.5f, -3278.281f, 128f), Mines.LandMine },
            { new Vector3(5495.5f, -3830.281f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(5495.5f, -4391.75f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(6544f, -5684f, 128f), Mines.LandMine },
            { new Vector3(6577f, -5231f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(7124f, -4468f, 128f), Mines.LandMine },
            { new Vector3(7055f, -4049f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(6921f, -3645f, 128f), Mines.LandMine },
            { new Vector3(7216.438f, 2662.656f, 256f), Mines.LandMine },
            { new Vector3(4669f, 2140f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(4397.125f, 236.1875f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(4842f, 1222f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(3807f, 1001f, 256f), Mines.LandMine },
            { new Vector3(3318f, 990f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(3040f, 352f, 256f), Mines.LandMine_RemoteMine },
            { new Vector3(1094f, -77.0625f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(1860f, 106f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(2051.719f, 486.375f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(2955f, 1333f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(2514f, 1089f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(1184f, 1632f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-730.5f, 1392.719f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-7048.5f, 3977.25f, 128f), Mines.LandMine },
            { new Vector3(-6984f, 4387f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-6985f, 4857f, 128f), Mines.LandMine },
            { new Vector3(-6605.5f, 5028.719f, 128f), Mines.LandMine },
            { new Vector3(-5522.5f, 4363.25f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-4683.438f, 4259f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-5055f, 4385f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-5303.5f, 4722.719f, 128f), Mines.LandMine },
            { new Vector3(-4876f, 4969f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-4349f, 5168f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-2421f, 5513f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-1436f, 3367f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(37.5f, 2063.25f, 256f), Mines.LandMine },
            { new Vector3(471.5f, 2059.25f, 256f), Mines.LandMine },
            { new Vector3(283.5f, 2417.719f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(743.5625f, 2933.25f, 256f), Mines.LandMine },
            { new Vector3(743.5625f, 2442.719f, 256f), Mines.LandMine },
            { new Vector3(1036.781f, 2027.906f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(2188.781f, 2712.906f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(2523f, 3406f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(3034.813f, 6705.188f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(3874.719f, 5776.938f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(3894.656f, 4644.938f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(3018.563f, 4781.156f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(3395.313f, 4961.875f, 256f), Mines.LandMine },
            { new Vector3(3361.438f, 4539.25f, 256f), Mines.LandMine },
            { new Vector3(3280.938f, 4130.563f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(3686.281f, 4266.75f, 256f), Mines.LandMine },
            { new Vector3(6307.813f, 3341.156f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(5155.125f, 3387.875f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(5514.281f, 2640.188f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(5108.781f, 2775.281f, 256f), Mines.LandMine },
            { new Vector3(5432.375f, 3049.219f, 256f), Mines.LandMine },
            { new Vector3(4665.906f, 2763.906f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(4867.188f, 3127.031f, 256f), Mines.LandMine },
            { new Vector3(4480.125f, 3999.219f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-2413f, -2002f, 128f), Mines.RemoteMine },
            { new Vector3(-1952f, -2272f, 128f), Mines.RemoteMine },
            { new Vector3(6157f, -4629f, 128f), Mines.RemoteMine },
            { new Vector3(4957f, -4310f, 128f), Mines.RemoteMine },
            { new Vector3(6735.906f, -460.4063f, 128f), Mines.RemoteMine },
            { new Vector3(-2526f, 1907f, 27.4375f), Mines.RemoteMine },
            { new Vector3(-2339f, 2244f, 18.625f), Mines.RemoteMine },
            { new Vector3(3069.969f, 5788.625f, 255f), Mines.RemoteMine },
            { new Vector3(3984.125f, 3449.094f, 254.3438f), Mines.RemoteMine },
            { new Vector3(6388.063f, 2577.5f, 256f), Mines.RemoteMine },
            { new Vector3(1475f, 927f, 128f), Mines.RemoteMine },
        };

        public static Dictionary<Vector3, Mines> Radiant = new Dictionary<Vector3, Mines>()
        {
            { new Vector3(2307.031f, -3105.563f, 256f), Mines.LandMine },
            { new Vector3(1797.656f, -3086.469f, 256f), Mines.LandMine },
            { new Vector3(2068.5f, -4526.75f, 256f), Mines.LandMine },
            { new Vector3(1719.5f, -4876.281f, 256f), Mines.LandMine },
            { new Vector3(-2328.781f, 2280.344f, 13.21875f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-2526f, 1907f, 27.4375f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-2421f, 5513f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-4290.563f, -6098.969f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(-7336.531f, -3088.969f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(-1436f, 3367f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-4349f, 5168f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-4683.438f, 4259f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-5055f, 4385f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-4876f, 4969f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-5303.5f, 4722.719f, 128f), Mines.LandMine },
            { new Vector3(-5522.5f, 4363.25f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-6605.5f, 5028.719f, 128f), Mines.LandMine },
            { new Vector3(-6985f, 4857f, 128f), Mines.LandMine },
            { new Vector3(-6984f, 4387f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-7048.5f, 3954.719f, 128f), Mines.LandMine },
            { new Vector3(-5612f, 2162f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-4227.5f, 1839.719f, 128f), Mines.LandMine },
            { new Vector3(-4543.5f, 2157.719f, 128f), Mines.LandMine },
            { new Vector3(-4635f, 1755f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-3365f, -1560f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-4123f, -64f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-2871.219f, 505.0625f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-2129f, 240f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-730.5f, 1392.719f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(1184f, 1632f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(1036.781f, 2027.906f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(2523f, 3406f, 128f), Mines.LandMine_RemoteMine },
            { new Vector3(4669f, 2140f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(4842f, 1222f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(4495.5f, 268.125f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(2514f, 1089f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(2955f, 1333f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(3807f, 1001f, 256f), Mines.LandMine },
            { new Vector3(3318f, 990f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(3040f, 352f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(2051.719f, 486.375f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(1860f, 106f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(1094f, -77.0625f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(1393.5f, -498.75f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(3810f, -1253f, 256f), Mines.LandMine },
            { new Vector3(5279f, -1644f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(4038f, -2500f, 0f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(5235f, -2388f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(6921f, -3645f, 128f), Mines.LandMine },
            { new Vector3(7055f, -4049f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(7124f, -4468f, 128f), Mines.LandMine },
            { new Vector3(6577f, -5231f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(6544f, -5684f, 128f), Mines.LandMine },
            { new Vector3(5415.188f, -6411.375f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(2016.469f, -3414.313f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(2411.781f, -3567.625f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(2767.188f, -3336.375f, 256f), Mines.LandMine },
            { new Vector3(2847.5f, -3730.281f, 256f), Mines.LandMine },
            { new Vector3(3740f, -3330f, 128f), Mines.LandMine },
            { new Vector3(3974f, -3702f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(5431.5f, -3255.75f, 128f), Mines.LandMine },
            { new Vector3(5495.5f, -3830.281f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(5495.5f, -4391.75f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(4270.5f, -4784.75f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(5006.063f, -5174.563f, 128f), Mines.LandMine },
            { new Vector3(4647f, -5476f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(4042f, -5513f, 128f), Mines.LandMine },
            { new Vector3(1663f, -4434f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(1598f, -3917f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(855.5f, -3665.281f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(767.5f, -1870.281f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-886.7813f, -1497.625f, 128f), Mines.LandMine_RemoteMine },
            { new Vector3(-1381f, -2062f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-865.5f, -2384.281f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-677f, -2833f, 128f), Mines.LandMine },
            { new Vector3(-1561f, -3209f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-2463f, -4079f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-999f, -4181f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-541.125f, -5451.531f, 128f), Mines.LandMine_StasisTrap },
            { new Vector3(-1038.094f, -4998.281f, 256f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(-1458.5f, -5053.156f, 200.875f), Mines.LandMine },
            { new Vector3(-1201.813f, -5389.375f, 128.1563f), Mines.LandMine },
            { new Vector3(-3528.594f, -5555.25f, 256f), Mines.LandMine },
            { new Vector3(-3544.375f, -5145.563f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(-3959.406f, -5269.563f, 256f), Mines.LandMine },
            { new Vector3(-3855.156f, -4858.563f, 256f), Mines.LandMine },
            { new Vector3(-4346.906f, -5052.188f, 256f), Mines.LandMine },
            { new Vector3(-3872f, -4448f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(-4240.938f, -4641.625f, 256f), Mines.LandMine },
            { new Vector3(-5604.469f, -3847.281f, 256f), Mines.LandMine_StasisTrap },
            { new Vector3(-5198.75f, -3682f, 256f), Mines.LandMine },
            { new Vector3(-5359.594f, -3289.688f, 256f), Mines.LandMine },
            { new Vector3(-5677.625f, -3022.813f, 256f), Mines.LandMine },
            { new Vector3(-5755.219f, -3442.25f, 256f), Mines.LandMine },
            { new Vector3(-2019f, -2766f, 128f), Mines.LandMine_StasisTrap_RemoteMine },
            { new Vector3(1475f, 927f, 128f), Mines.RemoteMine },
            { new Vector3(2278.219f, 1121.063f, 128f), Mines.RemoteMine },
            { new Vector3(6752.438f, -432.5625f, 128f), Mines.RemoteMine },
            { new Vector3(6157f, -4629f, 128f), Mines.RemoteMine },
            { new Vector3(4957f, -4310f, 128f), Mines.RemoteMine },
            { new Vector3(-1952f, -2272f, 128f), Mines.RemoteMine },
            { new Vector3(-2274.219f, -1962.656f, 128f), Mines.RemoteMine },
            { new Vector3(-6562.688f, -3118.406f, 256f), Mines.RemoteMine },
            { new Vector3(-4306.75f, -3953.219f, 256f), Mines.RemoteMine },
            { new Vector3(-3577.438f, -6131.438f, 252.7813f), Mines.RemoteMine },
            { new Vector3(-4091.094f, -6217.219f, 256f), Mines.RemoteMine },
        };
    }
}
