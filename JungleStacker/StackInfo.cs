using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JungleStacker
{
    internal static class StackInfo
    {
        public static Dictionary<string, Vector3[]> CampInfo = new()
        {
            {
                "neutralcamp_good_1",
                new Vector3[] {
                    new(3518, -5043, 128),
                    new(3426, -6347, 128),
                    new(-1f)
                }
            },
            {
                "neutralcamp_good_2",
                new Vector3[] {
                    new(4245, -3891, 128),
                    new(3114, -3614, 128),
                    new(-1f)
                }
            },
            {
                "neutralcamp_good_3",
                new Vector3[] {
                    new(2057, -4134, 256),
                    new(1145, -3849, 256),
                    new(0)
                }
            },
            {
                "neutralcamp_good_4",
                new Vector3[] {
                    new(-36, -3817, 256),
                    new(90, -3055, 255),
                    new(0)
                }
            },
            {
                "neutralcamp_good_5",
                new Vector3[] {
                    new(-1889, -3592, 128),
                    new(-1801, -2888, 128),
                    new(0)
                }
            },
            {
                "neutralcamp_good_6",
                new Vector3[] {
                    new(-2744, -178, 128),
                    new(-3947, -836, 256),
                    new(0)
                }
            },
            {
                "neutralcamp_good_7",
                new Vector3[] {
                    new(-3870, 615, 256),
                    new(-3896, -450, 256),
                    new(0)
                }
            },
            {
                "neutralcamp_good_8",
                new Vector3[] {
                    new(-4287, -110, 256),
                    new(-4884, 1702, 128),
                    new(-2f)
                }
            },
            {
                "neutralcamp_good_9",
                new Vector3[] {
                    new(831, -3034, 256),
                    new(1218, -3850, 256),
                    new(0)
                }
            },
            {
                "neutralcamp_evil_1",
                new Vector3[] {
                    new(-4679, 3847, 128),
                    new(-5858, 3554, 128),
                    new(-1f)
                }
            },
            {
                "neutralcamp_evil_2",
                new Vector3[] {
                    new(-3252, 5198, 128),
                    new(-3160, 6002, 128),
                    new(-1f)
                }
            },
            {
                "neutralcamp_evil_3",
                new Vector3[] {
                    new(-1625, 4404, 256),
                    new(-419, 4704, 256),
                    new(0)
                }
            },
            {
                "neutralcamp_evil_4",
                new Vector3[] {
                    new(-240, 3664, 256),
                    new(6, 4742, 256),
                    new(0)
                }
            },
            {
                "neutralcamp_evil_5",
                new Vector3[] {
                    new(1507, 3063, 128),
                    new(1330, 1903, 128),
                    new(0)
                }
            },
            {
                "neutralcamp_evil_6",
                new Vector3[] {
                    new(-1666, 3808, 256),
                    new(-2752, 3874, 256),
                    new(0)
                }
            },
            {
                "neutralcamp_evil_7",
                new Vector3[] {
                    new(2289, 122, 128),
                    new(2413, 934, 128),
                    new(0)
                }
            },
            {
                "neutralcamp_evil_8",
                new Vector3[] {
                    new(3811, 30, 256),
                    new(2918, 60, 256),
                    new(-1f)
                }
            },
            {
                "neutralcamp_evil_9",
                new Vector3[] {
                    new(3301, -871, 256),
                    new(3048, 133, 256),
                    new(0)
                }
            }
        };
    }
}
