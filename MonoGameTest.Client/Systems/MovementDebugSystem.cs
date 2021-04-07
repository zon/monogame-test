using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class MovementDebugSystem : AComponentSystem<float, Movement> {
		readonly SpriteBatch Batch;
		readonly Context Context;

		public MovementDebugSystem(World world, SpriteBatch batch, Context context) : base(world) {
			Batch = batch;
			Context = context;
		}

		protected override void PreUpdate(float dt) {
			Batch.Begin(transformMatrix: Context.Camera.GetMatrix());
		}

		protected override void Update(float dt, ref Movement movement) {
			if (movement.Path == null) return;
			PathfinderDebugSystem.DrawPath(Batch, Context, movement.Path);
		}

		protected override void PostUpdate(float dt) {
			Batch.End();
		}

	}

}
