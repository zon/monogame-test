using System;
using DefaultEcs;
using DefaultEcs.System;

namespace MonoGameTest.Common {

	public class MovementSystem : AEntitySetSystem<float> {
		readonly EntityMap<Position> Positions;
		readonly IContext Context;

		public MovementSystem(World world, IContext context) : base(world
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

			Node node = null;
			movement.Path = movement.Path.Pop(out node);

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

		public override void Dispose() {
			Positions.Dispose();
			base.Dispose();
		}

	}

}
