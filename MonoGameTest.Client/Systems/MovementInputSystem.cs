using System;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class MovementInputSystem : AEntitySetSystem<float> {
		readonly Grid Grid;
		readonly TiledMap TiledMap;
		readonly EntityMap<Position> Positions;
		readonly OrthographicCamera Camera;

		public MovementInputSystem(
			World world,
			Grid grid,
			TiledMap tiledMap,
			EntityMap<Position> positions,
			OrthographicCamera camera
		) : base(world
			.GetEntities()
			.With<Movement>()
			.With<Position>()
			.AsSet()
		) {
			Grid = grid;
			TiledMap = tiledMap;
			Positions = positions;
			Camera = camera;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var movement = ref entity.Get<Movement>();
			ref var position = ref entity.Get<Position>();

			var mouse = Mouse.GetState();
			if (
				mouse.LeftButton != ButtonState.Pressed &&
				mouse.RightButton != ButtonState.Pressed
			) return;

			var goal = Tiled.GetNode(TiledMap, Grid, Camera, mouse.X, mouse.Y);
			if (goal == null) return;

			movement.Path = Pathfinder.OptimalPathfind(Grid, Positions, position.Coord, goal.Coord);
		}

	}

}
