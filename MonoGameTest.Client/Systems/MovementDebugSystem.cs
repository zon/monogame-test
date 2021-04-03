using System.Collections.Immutable;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class MovementDebugSystem : AComponentSystem<float, Movement> {
		readonly TiledMap TiledMap;
		readonly SpriteBatch Batch;

		public MovementDebugSystem(SpriteBatch batch, TiledMap tiledMap,  World world) : base(world) {
			TiledMap = tiledMap;
			Batch = batch;
		}

		protected override void PreUpdate(float dt) {
			Batch.Begin();
		}

		protected override void Update(float dt, ref Movement movement) {
			if (movement.Path == null) return;
			PathfinderDebugSystem.DrawPath(Batch, TiledMap, movement.Path);
		}

		protected override void PostUpdate(float dt) {
			Batch.End();
		}

	}

}
