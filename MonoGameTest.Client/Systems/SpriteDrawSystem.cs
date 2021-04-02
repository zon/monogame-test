using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest.Client {

	public class SpriteDrawSystem : AComponentSystem<float, Sprite> {
		readonly SpriteBatch Batch;
		readonly Texture2D Texture;

		public SpriteDrawSystem(SpriteBatch batch, Texture2D texture, World world) : base(world) {
			Batch = batch;
			Texture = texture;
		}

		protected override void PreUpdate(float dt) {
			Batch.Begin();
		}

		protected override void Update(float dt, ref Sprite sprite) {
			Batch.Draw(
				Texture,
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
