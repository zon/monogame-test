using System;
using DefaultEcs;
using DefaultEcs.System;

namespace MonoGameTest.Common {

	public class MovementSystem : AEntitySetSystem<float> {
		Grid Grid;
		EntityMap<Position> Positions;

		public MovementSystem(World world, Grid grid, EntityMap<Position> positions) : base(world
			.GetEntities()
			.With<Movement>()
			.With<Cooldown>()
			.With<Position>()
			.AsSet()
		) {
			Grid = grid;
			Positions = positions;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var movement = ref entity.Get<Movement>();
			ref var cooldown = ref entity.Get<Cooldown>();
			ref var position = ref entity.Get<Position>();

			if (!cooldown.IsCool() || movement.IsIdle()) return;

			Node node = null;
			movement.Path = movement.Path.Pop(out node);

			Console.WriteLine("Move: {0}", node);

			// reset if something is in the path
			Entity other;
			if (
				Positions.TryGetEntity(node.Position, out other) &&
				other != entity
			) {
				movement.Path = movement.Path.Clear();
				return;
			}

			position.Coord = node.Coord;
			cooldown.action = Movement.ACTION_DURATION;
			cooldown.pause = Movement.PAUSE_DURATION;
		}

	}

}
