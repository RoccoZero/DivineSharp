/*using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Divine.Core.Helpers;
using Divine.Core.Managers.Unit;
using Divine.Core.Services;
using Ensage;
using Ensage.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SharpDX;

namespace Divine.Core.Managers.Mesh
{
    public sealed class MeshVisionUpdater
    {
        public readonly Cell[,] Cells = new Cell[260, 260];

        private List<Vector3> ParticlePositions;

        private int Zero;

        public MeshVisionUpdater()
        {
            using (var fs = new FileStream(DivineDirectory.Data + "MeshManager.dat", FileMode.Open)) //TODO Could not find file
            {
                using (var br = new BinaryReader(fs))
                {
                    for (var x = 0; x < 260; x++)
                    {
                        for (var y = 0; y < 260; y++)
                        {
                            Cells[x, y] = new Cell(new Vector3(br.ReadInt16(), br.ReadInt16(), br.ReadInt16()), (CellFlags)br.ReadUInt16(), x - 130, y - 130);
                        }
                    }
                }
            }

            var tokenFinish = JsonConvert.DeserializeObject<Cell[,]>(File.ReadAllText(@"C:\Users\osman\Desktop\MeshFinish.json"));

            using (var fs = new FileStream(@"C:\Users\osman\Desktop\MeshManager.dat", FileMode.Create)) //TODO Could not find file
            {
                using (var br = new BinaryWriter(fs))
                {
                    for (var x = 0; x < 260; x++)
                    {
                        for (var y = 0; y < 260; y++)
                        {
                            var cell = tokenFinish[x, y];

                            br.Write((short)cell.WorldPosition.X);
                            br.Write((short)cell.WorldPosition.Y);

                            if (cell.WorldPosition.Z == -16384)
                            {
                                cell.WorldPosition = new Vector3(cell.WorldPosition.X, cell.WorldPosition.Y, 0);
                            }

                            br.Write((short)cell.WorldPosition.Z);
                            br.Write((sbyte)Math.Floor(cell.WorldPosition.Z / 128f));

                            if (cell.CellFlags.HasFlag(CellFlags.Tree))
                            {
                                cell.CellFlags &= ~CellFlags.Tree;
                            }

                            br.Write((ushort)cell.CellFlags);
                        }
                    }
                }
            }
            return;

            for (var x = 0; x < 260; x++)
            {
                for (var y = 0; y < 260; y++)
                {
                    Cells[x, y] = tokenFinish[x, y];

                    //var cell = NavMesh.GetCell(x - 130, y - 130);
                    //Cells[x, y] = new Cell(cell.WorldPosition, (CellFlags)cell.NavMeshCellFlags, cell.GridX, cell.GridY);
                }
            }

            AppDomain.CurrentDomain.DomainUnload += (e, r) =>
            {
                var token = JToken.FromObject(Cells);
                File.WriteAllText(@"C:\Users\osman\Desktop\Mesh.json", token.ToString());
            };

            Entity.OnParticleEffectAdded += (a, e) =>
            {
                if (!e.ParticleEffect.IsValid)
                {
                    return;
                }

                if ("particles/units/heroes/hero_wisp/wisp_relocate_marker_endpoint.vpcf" != e.Name)
                {
                    return;
                }

                DelayAction.Add(-1, () =>
                {
                    ParticlePositions.Add(e.ParticleEffect.GetControlPoint(0));
                };
            };

            var task = Task.Run(async () =>
            {
                await Task.Delay(7000);

                for (var x = 0; x < 260; x++)
                {
                    for (var y = 0; y < 260; y++)
                    {
                        var cell = Cells[x, y];

                        if (cell.WorldPosition.Z != 0)
                        {
                            continue;
                        }

                        if (!MultiSleeper<string>.Sleeping("ghost"))
                        {
                            var gost = UnitManager.Owner.Base.Inventory.Items.First().UseAbility();
                            MultiSleeper<string>.Sleep("ghost", 2000);
                        }

                        var a = cell.WorldPosition + new Vector3(0, 0, 0);
                        var b = a + new Vector3(0, 64, 0);
                        var c = a + new Vector3(64, 0, 0);
                        var d = c + new Vector3(0, 64, 0);

                        ParticlePositions = new List<Vector3>();

                        UnitManager.Owner.Base.Spellbook.SpellR.UseAbility(a);
                        await Task.Delay(50);
                        UnitManager.Owner.Base.Spellbook.SpellR.UseAbility(b);
                        await Task.Delay(50);
                        UnitManager.Owner.Base.Spellbook.SpellR.UseAbility(c);
                        await Task.Delay(50);
                        UnitManager.Owner.Base.Spellbook.SpellR.UseAbility(d);
                        await Task.Delay(50);

                        //await Task.Delay(100);

                        if (ParticlePositions.Count != 4)
                        {
                            Zero++;
                        }

                        var p = ParticlePositions.OrderByDescending(j => j.Z).FirstOrDefault();

                        var particlePosition = new Vector3((int)Math.Floor(p.X), (int)Math.Floor(p.Y), (int)Math.Floor(p.Z));

                        var pos = new Vector3(cell.WorldPosition.X - particlePosition.X, cell.WorldPosition.Y - particlePosition.Y, particlePosition.Z);
                        Console.WriteLine($"X:{x} Y:{y}" + " | " + pos + " | " + Zero);

                        cell.WorldPosition = new SharpDX.Mathematics.Interop.RawVector3(cell.WorldPosition.X, cell.WorldPosition.Y, particlePosition.Z);
                    }
                }

            };

            task.ContinueWith(x =>
            {
                File.WriteAllText(@"C:\Users\osman\Desktop\Mesh.json", JToken.FromObject(Cells).ToString());
            };
        }
    }
}*/