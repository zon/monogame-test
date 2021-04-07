using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest.Client {

	public class SpriteDrawSystem : AComponentSystem<float, Sprite> {
		readonly SpriteBatch Batch;
		readonly Context Context;

		public SpriteDrawSystem(
			World world,
			SpriteBatch batch,
			Context context
		) : base(world) {
			Batch = batch;
			Context = context;
		}

		protected override void PreUpdate(float dt) {
			Batch.Begin(transformMatrix: Context.Camera.GetMatrix());
		}

		protected override void Update(float dt, ref Sprite sprite) {
			Batch.Draw(
				Context.Resources.Characters.Texture,
				sprite.Position,
				sprite.Rectangle,
				sprite.Color,
				sprite.Rotation,
				sprite.Origin,
				sprite.Scale,
				sprite.Effects,
				sprite.Depth
			);
		}

		protected override void PostUpdate(float dt) {
			Batch.End();
		}

	}

}
