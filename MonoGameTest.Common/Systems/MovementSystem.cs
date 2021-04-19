using System;
using System.Collections.Immutable;
using DefaultEcs;
using DefaultEcs.System;

namespace MonoGameTest.Common {

	public class MovementSystem : AEntitySetSystem<float> {
		readonly EntityMap<Position> Positions;
		readonly IContext Context;

		Grid Grid => Context.Grid;

		public MovementSystem(IContext context) : base(context.World
			.GetEntities()
			.With<Movement>()
			.AsSet()
		) {
			Positions = World.GetEntities().With<Character>().AsMap<Position>();
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var movement = ref entity.Get<Movement>();
			ref var cooldown = ref entity.Get<Cooldown>();
			ref var position = ref entity.Get<Position>();

			if (!cooldown.IsCool() || movement.IsIdle) return;

			// determine path
			ImmutableStack<Node> path;
			if (movement.Path == null) {
				var pathfinder = new Pathfinder(Context.Grid, Positions);
				
				// correct unreachable goals
				var start = Grid.Get(position.Coord);
				var goal = Grid.Get(movement.Goal.Value);
				if (goal != null && goal.Solid) {
					goal = pathfinder.OptimalMoveTo(start, goal).Node;
					movement.Goal = goal?.Coord;
				}

				path = pathfinder.MoveTo(start, goal).Path;

			// use preloaded movement path
			} else {
				path = movement.Path;
				movement.Path = null;
			} 
			
			// reset if path is empty
			if (path.IsEmpty) {
				movement.Goal = null;
				return;
			}

			// get first step
			Node node = null;
			path.Pop(out node);

			// only move if position is empty
			if (Positions.ContainsKey(node.Position)) return;

			// move
			position.Coord = node.Coord;
			cooldown.action = Movement.ACTION_DURATION;
			cooldown.pause = Movement.PAUSE_DURATION;

			// clear at goal
			if (node.Coord == movement.Goal) {
				movement.Goal = null;
			}

			entity.NotifyChanged<Position>();
		}

		public override void Dispose() {
			Positions.Dispose();
			base.Dispose();
		}

	}

}
