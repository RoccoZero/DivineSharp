using Divine;
using Divine.SDK.Extensions;
using Divine.SDK.Helpers;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JungleStacker
{
    internal sealed class Stacker
    {
        public Hero LocalHero;
        public UpdateHandler UpdateHandler;
        public List<Camp> Camps = new List<Camp> { };
        public Menu Menu;

        public Stacker(Context context)
        {
            LocalHero = EntityManager.LocalHero;
            Menu = context.Menu;
            Menu.Enabled.ValueChanged += Enabled_ValueChanged;
            

            foreach (var camp in GameManager.NeutralCamps)
            {
                StackInfo.Positions.TryGetValue(camp.Name, out List<(string, Vector3)> outInfo);
                Camps.Add(new Camp
                {
                    Name = camp.Name, 
                    Position = camp.Box.Center,
                    WaitPosition = outInfo[0].Item2,
                    StackPosition = outInfo[1].Item2,
                    TimeOffset = (int)outInfo[2].Item2.X
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

        private void Enabled_ValueChanged(Divine.Menu.Items.MenuSwitcher switcher, Divine.Menu.EventArgs.SwitcherEventArgs e)
        {
            if (e.Value)
            {
                UpdateHandler = UpdateManager.CreateIngameUpdate(100, UpdateManager_IngameUpdate);
                RendererManager.Draw += RendererManager_Draw;
                Menu.AutoSelectUnits.ValueChanged += AutoSelectUnits_ValueChanged;
            }
            else
            {
                UpdateManager.DestroyIngameUpdate(UpdateHandler);
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
                    nearestCamp.Unit = unit;
                    EntityManager.LocalHero.Select();
                }
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
            var Minutes = (int)(GameManager.GameTime / 60) < 10 ? "0" + ((int)(GameManager.GameTime / 60)).ToString() : ((int)(GameManager.GameTime / 60)).ToString();
            var Seconds = (int)(GameManager.GameTime % 60f) < 10 ? "0" + ((int)(GameManager.GameTime % 60f)).ToString() : ((int)(GameManager.GameTime % 60f)).ToString();
            Console.WriteLine(Minutes + ":" + Seconds );
            foreach (var camp in Camps)
            {
                if (camp.Unit != null && (!camp.Unit.IsValid || !camp.Unit.IsAlive))
                {
                    StackerOrderManager.LastOrders[camp.Unit] = (OrderType.None, (Vector3.Zero, null));
                    camp.Unit = null;
                }
                if (!camp.IsActive || camp.Unit == null)
                {
                    camp.StackState = StackState.None;
                    continue;
                }

                Console.WriteLine($"Camp Name: {camp.Name} Camp Unit: {camp.Unit.Name} State: {camp.StackState}");
                int.TryParse(Seconds, out var seconds);

                var nearestUnit = EntityManager.GetEntities<Unit>().Where(x => !x.IsWaitingToSpawn && x.Handle != camp.Unit.Handle && x.Distance2D(camp.Unit.Position) <= 700 && x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral).OrderBy( x => x.Distance2D(camp.Unit.Position)).FirstOrDefault();
                Console.WriteLine($"nearestName: {nearestUnit?.Name} Activity: {nearestUnit?.NetworkActivity}");
                switch (camp.StackState)
                {
                    case StackState.None:
                        if (seconds >= 3 && seconds < 50)
                        {
                            camp.StackState = StackState.WaitPosition;
                        }
                        break;
                    case StackState.WaitPosition:
                        StackerOrderManager.Move(camp.Unit, camp.WaitPosition, false);
                        var unitsInCamp = EntityManager.GetEntities<Unit>().Where(x => !x.IsWaitingToSpawn && x.Handle != camp.Unit.Handle && x.Distance2D(camp.Unit.Position) <= 700 && x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral).Count();
                        switch (unitsInCamp)
                        {
                            case <= 7:
                                camp.StackTime = 55 - (camp.Unit.IsRanged ? 0 : 1) + camp.TimeOffset;
                                break;
                            case (>= 8 and < 17):
                                camp.StackTime = 54 - (camp.Unit.IsRanged ? 0 : 1) + camp.TimeOffset;
                                break;
                            case >= 17:
                                camp.StackTime = 53 - (camp.Unit.IsRanged ? 0 : 1) + camp.TimeOffset;
                                break;
                            default:
                                break;
                        }

                        Console.WriteLine($"Units in camp: {unitsInCamp} Stack Time: {camp.StackTime}");
                        if (seconds == camp.StackTime)
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
                        if (camp.Unit.Position.Distance2D(camp.WaitPosition) <= 10)
                        {
                            StackerOrderManager.Move(camp.Unit, nearestUnit.Position, false);
                        }

                        if (camp.Unit.Distance2D(nearestUnit) <= camp.Unit.AttackRange)
                        {
                            StackerOrderManager.Attack(camp.Unit, nearestUnit, false);
                            MultiSleeper<string>.Sleeper($"Stacker.{camp.Unit.Handle}").Sleep(((camp.Unit.AttackPoint() + GameManager.AvgPing) * 1000f) + 75f);
                            camp.StackState = StackState.StackPosition;
                        }

                        //if (nearestUnit.NetworkActivity != NetworkActivity.Idle)
                        {
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

        internal void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(UpdateHandler);
            RendererManager.Draw -= RendererManager_Draw;
        }
    }
}