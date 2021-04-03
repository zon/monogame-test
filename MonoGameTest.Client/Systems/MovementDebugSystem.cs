using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class MovementDebugSystem : AComponentSystem<float, Movement> {
		readonly TiledMap TiledMap;
		readonly OrthographicCamera Camera;
		readonly SpriteBatch Batch;

		public MovementDebugSystem(SpriteBatch batch, TiledMap tiledMap, OrthographicCamera camera, World world) : base(world) {
			TiledMap = tiledMap;
			Camera = camera;
			Batch = batch;
		}

		protected override void PreUpdate(float dt) {
			Batch.Begin(transformMatrix: Camera.GetViewMatrix());
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
