using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class HealthBarSystem : AEntitySetSystem<float> {
		Context Context;

		const float MARGIN = 1;
		const float HEIGHT = 2;

		SpriteBatch Batch => Context.Foreground;
		Camera Camera => Context.Camera;
		
		public HealthBarSystem(Context context) : base(context.World
			.GetEntities()
			.With<Health>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var health = ref entity.Get<Health>();
			if (health.IsFull) return;

			ref var sprite = ref entity.Get<Sprite>();
			var req = new RectangleF(
				x: sprite.Position.X + MARGIN,
				y: sprite.Position.Y + sprite.Rectangle.Height,
				width: sprite.Rectangle.Width - MARGIN * 2,
				height: HEIGHT
			);
			var depth = Camera.Depth(sprite.Position.Y, Depths.Character);
			Batch.FillRectangle(req, Color.DarkGreen, depth);

			req.Width *= health.Percentage;
			Batch.FillRectangle(req, Color.YellowGreen, depth + 0.01f);
		}

	}

}
