using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameTest.Client {

	public class SpriteDrawSystem : AComponentSystem<float, Sprite> {
		readonly SpriteBatch Batch;
		readonly OrthographicCamera Camera;
		readonly Texture2D Texture;

		public SpriteDrawSystem(
			SpriteBatch batch,
			OrthographicCamera camera,
			Texture2D texture,
			World world
		) : base(world) {
			Batch = batch;
			Camera = camera;
			Texture = texture;
		}

		protected override void PreUpdate(float dt) {
			Batch.Begin(transformMatrix: Camera.GetViewMatrix());
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
