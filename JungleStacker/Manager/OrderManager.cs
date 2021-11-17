using System.Collections.Generic;

using Divine.Entity.Entities.Units;
using Divine.Numerics;
using Divine.Order.Orders.Components;

namespace JungleStacker.Managers
{
    static internal class StackerOrderManager
    {
        public static Dictionary<Unit, (OrderType, (Vector3, Unit))> LastOrders = new Dictionary<Unit, (OrderType, (Vector3, Unit))> { };

        public static void Move(Unit Unit, Vector3 Position, bool Queued)
        {
            if (LastOrders.TryGetValue(Unit, out var Value) && Value.Item1 == OrderType.MovePosition && Value.Item2.Item1 == Position)
            {
                //Console.WriteLine("return");
                return;
            }

            Unit.Move(Position, Queued);
            LastOrders[Unit] = (OrderType.MovePosition, (Position, null));
        }

        public static void Attack(Unit Unit, Unit Target, bool Queued)
        {
            if (LastOrders.TryGetValue(Unit, out var Value) && Value.Item1 == OrderType.AttackTarget && Value.Item2.Item2 == Target)
            {
                //Console.WriteLine("return");
                return;
            }

            Unit.Attack(Target, Queued);
            LastOrders[Unit] = (OrderType.AttackTarget, (Vector3.Zero, Target));
        }
    }
}
