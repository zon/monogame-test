using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class MovementDebugSystem : AComponentSystem<float, Movement> {
		readonly Context Context;
		readonly SpriteBatch Batch;

		public MovementDebugSystem(Context context, SpriteBatch batch) : base(context.World) {
			Context = context;
			Batch = batch;
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
