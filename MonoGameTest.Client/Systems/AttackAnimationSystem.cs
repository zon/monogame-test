using System;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
			ref var animation = ref entity.Get<AttackAnimation>();
			if (!animation.IsActive) return;
			animation.Update(dt);
			if (!animation.IsActive) return;

			ref var characterSprite = ref entity.Get<Sprite>();

			if (animation.Attack.IsMelee) {
				var offset = Vector2.Zero;
				var a = Vector2.Zero;
				var b = animation.Forward * FORWARD_STEP;
				if (animation.IsLeading) {
					offset = Vector2.Lerp(a, b, animation.LeadProgress);
				} else {
					offset = Vector2.Lerp(b, a, animation.FollowProgress);
				}
				characterSprite.Position += offset;
				animation.Sprite.Position = characterSprite.Position + Context.HalfTileSize + animation.Forward * Context.HalfTileSize.X;

			} else {
				animation.Sprite.Position = characterSprite.Position + Context.HalfTileSize;
			}

			animation.Sprite.Rotation = animation.Rotation;
			var depth = Depths.Attack;
			if (animation.Rotation < 0) {
				depth = -Depths.Attack;
			}
			animation.Sprite.LayerDepth = Camera.Depth(characterSprite.Position, depth);
			animation.Sprite.Render(Batch);
		}

	}

}
