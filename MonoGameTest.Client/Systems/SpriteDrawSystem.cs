using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest.Client {

	public class SpriteDrawSystem : AComponentSystem<float, Sprite> {
		readonly Context Context;

		SpriteBatch Batch => Context.Batch;
		Camera Camera => Context.Camera;

		public SpriteDrawSystem(Context context) : base(context.World) {
			Context = context;
		}

		protected override void PreUpdate(float dt) {
			Batch.Begin(transformMatrix: Camera.GetMatrix(), samplerState: SamplerState.PointClamp);
		}

		protected override void Update(float dt, ref Sprite sprite) {
			Batch.Draw(
				texture: sprite.Document.Texture,
				position: sprite.Position,
				sourceRectangle: sprite.Rectangle,
				color: sprite.Color,
				rotation: sprite.Rotation,
				origin: sprite.Origin,
				scale: sprite.Scale,
				effects: sprite.Effects,
				layerDepth: 0
			);
		}

		protected override void PostUpdate(float dt) {
			Batch.End();
		}

	}

}
