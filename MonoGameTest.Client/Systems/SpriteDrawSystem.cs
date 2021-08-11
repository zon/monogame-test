using System;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest.Client {

	public class SpriteDrawSystem : AComponentSystem<float, Sprite> {
		readonly Context Context;

		SpriteBatch Batch => Context.WorldBatch;
		CameraView Camera => Context.WorldCameraView;

		public SpriteDrawSystem(Context context) : base(context.World) {
			Context = context;
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
				layerDepth: Camera.Depth(sprite.Position)
			);
		}

	}

}
