using System.Collections.Immutable;
using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class MovementSystem : AEntitySetSystem<float> {
		readonly Context Context;

		Grid Grid => Context.Grid;
		EntityMap<Position> Positions => Context.Positions;

		public MovementSystem(Context context) : base(context.World
			.GetEntities()
			.With<Movement>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var character = ref entity.Get<Character>();
			ref var movement = ref entity.Get<Movement>();
			ref var position = ref entity.Get<Position>();

			if (!character.IsIdle || movement.IsIdle) return;

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
			character.StartCooldown(Movement.ACTION_DURATION + Movement.PAUSE_DURATION);

			// clear at goal
			if (node.Coord == movement.Goal) {
				movement.Goal = null;
			}

			entity.NotifyChanged<Position>();
		}

	}

}
