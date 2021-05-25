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
			.With<Character>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var character = ref entity.Get<Character>();
			ref var position = ref entity.Get<Position>();

			if (!character.HasCommand || character.State != CharacterState.Standby) return;

			var command = character.GetCurrentCommand().Value;
			if (!command.HasTarget) return;
			var target = command.Target.Value;
			var pathfinder = Context.CreatePathfinder();
			var start = Grid.Get(position.Coord);
			var goal = Grid.Get(target.GetPosition());

			// determine path
			ImmutableStack<Node> path;
			if (command.Skill?.IsRanged == true) {
				path = pathfinder.MoveWithinRange(start, goal, command.Skill.Range).Path;
			} else if (target.IsMobile) {
				path = pathfinder.MoveAdjacent(start, goal).Path;
			} else {
				path = pathfinder.MoveTo(start, goal).Path;
			}
			
			// cancel command if path is empty
			if (path.IsEmpty) {
				character.CancelCommand();
				return;
			}

			// get first step
			Node node = null;
			path.Pop(out node);

			// only move if position is empty
			if (Positions.ContainsKey(node.Position)) return;
 
			// move
			position.Coord = node.Coord;
			character.BeginCooldown(Movement.ACTION_DURATION + character.Role.MoveCooldown);

			// clear at goal
			if (command.IsMove && node == goal) {
				character.CancelCommand();
			}

			entity.NotifyChanged<Position>();
		}

	}

}
