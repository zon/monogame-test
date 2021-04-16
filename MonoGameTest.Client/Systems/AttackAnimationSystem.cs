using System;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class AttackAnimationSystem : AEntitySetSystem<float> {
		readonly Context Context;

		const float FORWARD_STEP = 4;
		
		SpriteBatch Batch => Context.Foreground;
		Camera Camera => Context.Camera;

		public AttackAnimationSystem(Context context) : base(context.World
			.GetEntities()
			.With<AttackAnimation>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var attack = ref entity.Get<AttackAnimation>();
			if (!attack.IsActive) return;
			attack.Update(dt);
			if (!attack.IsActive) return;

			ref var characterSprite = ref entity.Get<Sprite>();
			var offset = Vector2.Zero;
			var a = Vector2.Zero;
			var b = new Vector2(attack.Facing.X, attack.Facing.Y) * FORWARD_STEP;
			var half = 0.5f;
			if (attack.Progress < half) {
				offset = Vector2.Lerp(a, b, attack.Progress / half);
			} else {
				offset = Vector2.Lerp(b, a, (attack.Progress - half) / half);
			}
			characterSprite.Position += offset;

			var width = new Vector2(characterSprite.Rectangle.Width, 0);
			var height = new Vector2(0, characterSprite.Rectangle.Height);
			var halfWidth = new Vector2(characterSprite.Rectangle.Width / 2, 0);
			var halfHeight = new Vector2(0, characterSprite.Rectangle.Height / 2);
			var depth = Depths.Attack;
			if (attack.Facing.X == 0) {
				if (attack.Facing.Y < 0) {
					attack.Sprite.Position = characterSprite.Position + halfWidth;
					attack.Sprite.SpriteEffect = SpriteEffects.FlipVertically;
					depth = -Depths.Attack;
				} else {
					attack.Sprite.Position = characterSprite.Position + height + halfWidth;
					attack.Sprite.SpriteEffect = SpriteEffects.None;
				}
			} else {
				if (attack.Facing.X < 0) {
					attack.Sprite.Position = characterSprite.Position + halfHeight;
					attack.Sprite.SpriteEffect = SpriteEffects.FlipHorizontally;
				} else {
					attack.Sprite.Position = characterSprite.Position + width + halfHeight;
					attack.Sprite.SpriteEffect = SpriteEffects.None;
				}
				characterSprite.Effects = attack.Sprite.SpriteEffect;
			}
			attack.Sprite.LayerDepth = Camera.Depth(characterSprite.Position, depth);
			attack.Sprite.Render(Batch);
		}

	}

}
