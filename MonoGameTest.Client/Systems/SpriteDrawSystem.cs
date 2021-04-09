using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest.Client {

	public class SpriteDrawSystem : AComponentSystem<float, Sprite> {
		readonly Context Context;
		readonly SpriteBatch Batch;

		public SpriteDrawSystem(Context context, SpriteBatch batch) : base(context.World) {
			Context = context;
			Batch = batch;
		}

		protected override void PreUpdate(float dt) {
			Batch.Begin(transformMatrix: Context.Camera.GetMatrix(), samplerState: SamplerState.PointClamp);
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
