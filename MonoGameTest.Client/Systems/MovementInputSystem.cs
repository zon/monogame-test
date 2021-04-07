using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Input;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class MovementInputSystem : AEntitySetSystem<float> {
		readonly EntityMap<Position> Positions;
		readonly Context Context;

		public MovementInputSystem(World world, Context context) : base(world
			.GetEntities()
			.With<Movement>()
			.With<Position>()
			.AsSet()
		) {
			Positions = World.GetEntities().With<Character>().AsMap<Position>();
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var movement = ref entity.Get<Movement>();
			ref var position = ref entity.Get<Position>();

			var mouse = Mouse.GetState();
			if (
				mouse.LeftButton != ButtonState.Pressed &&
				mouse.RightButton != ButtonState.Pressed
			) return;

			var goal = Context.GetNode(mouse.X, mouse.Y);
			if (goal == null) return;

			movement.Path = Pathfinder.OptimalPathfind(Context.Grid, Positions, position.Coord, goal.Coord);
		}

		public override void Dispose() {
			Positions.Dispose();
			base.Dispose();
		}

	}

}
