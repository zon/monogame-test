using System;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace MonoGameTest.Client {

	public class BangSystem : AEntitySetSystem<float> {
		Context Context;

		const float DURATION = 1;

		SpriteBatch Batch => Context.Foreground;
		Camera Camera => Context.Camera;

		public BangSystem(Context context) : base(context.World
			.GetEntities()
			.With<Bang>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var bang = ref entity.Get<Bang>();
			if (bang.IsDone) return;

			ref var sprite = ref entity.Get<Sprite>();
			var font = Context.Resources.Font;
			var p = sprite.Position + new Vector2(sprite.Rectangle.Width / 2, 0);
			var a = p + new Vector2(0, 9);
			var b = p + new Vector2(0, 1);
			var position = Vector2.Lerp(a, b, bang.Progress);
			var text = Math.Abs(bang.Delta).ToString();
			var size = font.MeasureString(text);
			var origin = new Vector2(size.Width / 2, size.Height);

			Batch.DrawString(
				font,
				text,
				position + new Vector2(1, 1),
				Color.Black,
				0,
				origin,
				1,
				SpriteEffects.None,
				Camera.Depth(sprite.Position, Depths.Bang)
			);
			Batch.DrawString(
				font,
				text,
				position,
				Color.WhiteSmoke,
				0,
				origin,
				1,
				SpriteEffects.None,
				Camera.Depth(sprite.Position, Depths.Bang)
			);

			bang.Update(dt, DURATION);
		}

	}

}
