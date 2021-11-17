using Divine.Entity;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Helpers;
using Divine.Map;
using Divine.Numerics;
using Divine.Order.Orders.Components;
using Divine.Particle;
using Divine.Particle.Components;
using Divine.Renderer;
using Divine.Update;

using JungleStacker.Data;
using JungleStacker.Enums;
using JungleStacker.Managers;

using System;
using System.Collections.Generic;
using System.Linq;

namespace JungleStacker.Core
{
    internal sealed class Stacker
    {
        public Hero LocalHero;
        public List<Camp> Camps = new List<Camp> { };
        public List<Camp> ActiveCamps = new List<Camp> { };
        public Menu Menu;

        public Stacker(Context context)
        {
            LocalHero = EntityManager.LocalHero;
            Menu = context.Menu;
            Menu.Enabled.ValueChanged += Enabled_ValueChanged;

            CampBoundsDraw();
        }

        private void Enabled_ValueChanged(Divine.Menu.Items.MenuSwitcher switcher, Divine.Menu.EventArgs.SwitcherEventArgs e)
        {
            if (e.Value)
            {
                UpdateManager.IngameUpdate += UpdateManager_IngameUpdate;
                RendererManager.Draw += RendererManager_Draw;
                Menu.AutoSelectUnits.ValueChanged += AutoSelectUnits_ValueChanged;
            }
            else
            {
                UpdateManager.IngameUpdate -= UpdateManager_IngameUpdate;
                RendererManager.Draw -= RendererManager_Draw;
                Menu.AutoSelectUnits.ValueChanged -= AutoSelectUnits_ValueChanged;
            }
        }

        private void AutoSelectUnits_ValueChanged(Divine.Menu.Items.MenuHoldKey holdKey, Divine.Menu.EventArgs.HoldKeyEventArgs e)
        {
            if (e.Value)
            {
                foreach (var unit in EntityManager.LocalPlayer.SelectedUnits)
                {
                    var nearestCamp = Camps.Where(x => x.Unit == null).OrderBy(x => unit.Distance2D(x.Position)).FirstOrDefault();
                    nearestCamp.Unit ??= unit;
                }
                EntityManager.LocalHero.Select();
            }
        }

        private void RendererManager_Draw()
        {
            foreach (var camp in Camps)
            {
                camp.DrawInfo();
            }

        }

        private void UpdateManager_IngameUpdate()
        {
            //var Minutes = (int)(GameManager.GameTime / 60) < 10 ? "0" + ((int)(GameManager.GameTime / 60)).ToString() : ((int)(GameManager.GameTime / 60)).ToString();
            //var Seconds = (int)(GameManager.GameTime % 60f) < 10 ? "0" + ((int)(GameManager.GameTime % 60)).ToString() : ((int)(GameManager.GameTime % 60f)).ToString();
            //int.TryParse(Seconds, out var seconds);
            var seconds = (float)(int)(GameManager.GameTime % 60 * 10) / 10;
            //Console.WriteLine(seconds);
            foreach (var camp in Camps)
            {

                if (camp.Unit != null && (!camp.Unit.IsValid || !camp.Unit.IsAlive))
                {
                    StackerOrderManager.LastOrders[camp.Unit] = (OrderType.None, (Vector3.Zero, null));
                    camp.Unit = null;
                    camp.StackState = StackState.None;
                    //ActiveCamps.Remove(camp);
                }
                if (!camp.IsActive || camp.Unit == null)
                {
                    continue;
                }

                var nearestUnit = EntityManager.GetEntities<Unit>().Where(x => x.Distance2D(camp.Unit.Position) <= 700 && !x.IsWaitingToSpawn && x.Handle != camp.Unit.Handle && x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral).OrderBy(x => x.Distance2D(camp.Unit.Position)).FirstOrDefault();

                //Console.WriteLine($"Camp Name: {camp.Name} Camp Unit: {camp.Unit?.Name} State: {camp.StackState}");
                //Console.WriteLine($"nearestName: {nearestUnit?.Name} Activity: {nearestUnit?.NetworkActivity}");
                switch (camp.StackState)
                {
                    case StackState.None:
                        if (seconds >= 5 && seconds < 50)
                        {
                            camp.StackState = StackState.WaitPosition;
                        }
                        break;
                    case StackState.WaitPosition:
                        StackerOrderManager.Move(camp.Unit, camp.WaitPosition, false);
                        var unitsInCamp = EntityManager.GetEntities<Unit>().Where(x => x.Distance2D(camp.Position) <= 700 && !x.IsWaitingToSpawn && x.Handle != camp.Unit.Handle && x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral).Count();
                        var calcTimeOffset = 0f;
                        if (nearestUnit != null)
                        {
                            calcTimeOffset = ((nearestUnit.Distance2D(camp.Unit) - camp.Unit.AttackRange) / camp.Unit.MovementSpeed * 10f + camp.Unit.AttackPoint()) / 10f;
                        }
                        Console.WriteLine(camp.Unit.AttackPoint());
                        switch (unitsInCamp)
                        {
                            case <= 6:
                                camp.StackTime = 55f - calcTimeOffset + camp.TimeOffset;
                                break;
                            case >= 7 and < 13:
                                camp.StackTime = 54f - calcTimeOffset + camp.TimeOffset;
                                break;
                            case >= 13:
                                camp.StackTime = 53f - calcTimeOffset + camp.TimeOffset;
                                break;
                        }

                        //Console.WriteLine($"Units in camp: {unitsInCamp} Stack Time: {camp.StackTime}");
                        if (seconds == camp.StackTime || seconds + 0.1f == camp.StackTime)
                        {
                            if (nearestUnit == null)
                            {
                                camp.StackState = StackState.StackPosition;
                                continue;
                            }
                            camp.StackState = StackState.HitPosition;
                        }
                        break;
                    case StackState.HitPosition:
                        if (camp.Unit.Position.Distance2D(camp.Position) <= 700)
                        {
                            StackerOrderManager.Move(camp.Unit, nearestUnit.Position, false);
                        }

                        if (camp.Unit.Distance2D(nearestUnit) <= camp.Unit.AttackRange)
                        {
                            StackerOrderManager.Attack(camp.Unit, nearestUnit, false);
                            MultiSleeper<string>.Sleeper($"Stacker.{camp.Unit.Handle}").Sleep(camp.Unit.AttackPoint() * 1000f + GameManager.AvgPing + 150f);
                            camp.StackState = StackState.StackPosition;
                        }
                        break;
                    case StackState.StackPosition:
                        if (!MultiSleeper<string>.Sleeper($"Stacker.{camp.Unit.Handle}").Sleeping)
                        {
                            StackerOrderManager.Move(camp.Unit, camp.StackPosition, false);
                            camp.StackState = StackState.None;
                        }
                        break;
                }
            }
        }

        private void CampBoundsDraw()
        {
            foreach (var camp in GameManager.NeutralCamps)
            {
                StackInfo.CampInfo.TryGetValue(camp.Name, out Vector3[] outInfo);
                Camps.Add(new Camp
                {
                    Name = camp.Name,
                    Position = camp.Box.Center,
                    WaitPosition = outInfo[0],
                    StackPosition = outInfo[1],
                    TimeOffset = outInfo[2].X
                });

                var corners = camp.Box.GetCorners();

                if (camp.Box.Intersects(new BoundingSphere(LocalHero.Position, LocalHero.HullRadius)))
                {
                    ParticleManager.CreateOrUpdateParticle(
                        camp.Name,
                        "particles/ui_mouseactions/bounding_area_view.vpcf",
                        ParticleAttachment.AbsOrigin,
                        new ControlPoint(0, MapManager.GetAbsolutePosition(corners[0])),
                        new ControlPoint(1, MapManager.GetAbsolutePosition(corners[1])),
                        new ControlPoint(2, MapManager.GetAbsolutePosition(corners[2])),
                        new ControlPoint(3, MapManager.GetAbsolutePosition(corners[3])),
                        new ControlPoint(15, Color.Red)
                    );
                }
                else
                {
                    ParticleManager.CreateOrUpdateParticle(
                        camp.Name,
                        "particles/ui_mouseactions/bounding_area_view.vpcf",
                        ParticleAttachment.AbsOrigin,
                        new ControlPoint(0, MapManager.GetAbsolutePosition(corners[0])),
                        new ControlPoint(1, MapManager.GetAbsolutePosition(corners[1])),
                        new ControlPoint(2, MapManager.GetAbsolutePosition(corners[2])),
                        new ControlPoint(3, MapManager.GetAbsolutePosition(corners[3])),
                        new ControlPoint(15, Color.Yellow)
                    );
                }

            }
        }

        internal void Dispose()
        {
            UpdateManager.IngameUpdate -= UpdateManager_IngameUpdate;
            RendererManager.Draw -= RendererManager_Draw;
            Menu.AutoSelectUnits.ValueChanged -= AutoSelectUnits_ValueChanged;
        }
    }
}