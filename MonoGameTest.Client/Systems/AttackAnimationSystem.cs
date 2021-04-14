using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest.Client {

	public class AttackAnimationSystem : AComponentSystem<float, AttackAnimation> {
		readonly Context Context;
		
		SpriteBatch Batch => Context.Batch;
		Camera Camera => Context.Camera;

		public AttackAnimationSystem(Context context) : base(context.World) {
			Context = context;
		}

		protected override void PreUpdate(float dt) {
			Batch.Begin(transformMatrix: Camera.GetMatrix(), samplerState: SamplerState.PointClamp);
		}

		protected override void Update(float dt, ref AttackAnimation attack) {
			if (!attack.Sprite.Animating) return;
			attack.Sprite.Update(dt);
			if (!attack.Sprite.Animating) return;
			attack.Sprite.Render(Batch);
		}

		protected override void PostUpdate(float dt) {
			Batch.End();
		}

	}

}
