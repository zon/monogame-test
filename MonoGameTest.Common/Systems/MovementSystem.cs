using System;
using DefaultEcs;
using DefaultEcs.System;

namespace MonoGameTest.Common {

	public class MovementSystem : AEntitySetSystem<float> {
		readonly EntityMap<Position> Positions;
		readonly IContext Context;

		public MovementSystem(IContext context) : base(context.World
			.GetEntities()
			.With<Movement>()
			.With<Cooldown>()
			.With<Position>()
			.AsSet()
		) {
			Positions = World.GetEntities().With<Character>().AsMap<Position>();
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var movement = ref entity.Get<Movement>();
			ref var cooldown = ref entity.Get<Cooldown>();
			ref var position = ref entity.Get<Position>();

			if (!cooldown.IsCool() || movement.IsIdle()) return;

			var path = Pathfinder.Pathfind(
				Context.Grid,
				Positions,
				position.Coord,
				movement.Goal.Value
			);
			
			// reset if path isn't possible
			if (path.IsEmpty) {
				movement.Goal = null;
				return;
			}

			// move to first step in path
			Node node = null;
			path.Pop(out node);
			position.Coord = node.Coord;

			// clear at goal
			if (node.Coord == movement.Goal) {
				movement.Goal = null;
			}

			cooldown.action = Movement.ACTION_DURATION;
			cooldown.pause = Movement.PAUSE_DURATION;

			entity.NotifyChanged<Position>();
		}

		public override void Dispose() {
			Positions.Dispose();
			base.Dispose();
		}

	}

}
