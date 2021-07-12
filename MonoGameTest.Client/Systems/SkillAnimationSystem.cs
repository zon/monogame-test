using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest.Client {

	public class SkillAnimationSystem : AEntitySetSystem<float> {
		readonly Context Context;

		const float FORWARD_STEP = 4;
		
		SpriteBatch Batch => Context.WorldBatch;
		Camera Camera => Context.WorldCamera;

		public SkillAnimationSystem(Context context) : base(context.World
			.GetEntities()
			.With<SkillAnimation>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var animation = ref entity.Get<SkillAnimation>();
			if (!animation.IsActive) return;
			animation.Update(dt);
			if (!animation.IsActive) return;

			ref var characterSprite = ref entity.Get<Sprite>();

			if (animation.Skill.IsMelee) {
				var offset = Vector2.Zero;
				var a = Vector2.Zero;
				var b = animation.Forward * FORWARD_STEP;
				if (animation.IsLeading) {
					offset = Vector2.Lerp(a, b, animation.LeadProgress);
				} else {
					offset = Vector2.Lerp(b, a, animation.FollowProgress);
				}
				characterSprite.Position += offset;
				animation.Attack.Position = characterSprite.Position + Context.HalfTileSize + animation.Forward * Context.HalfTileSize.X;

			} else {
				animation.Attack.Position = characterSprite.Position + Context.HalfTileSize;
			}

			animation.Attack.Rotation = animation.Rotation;
			var depth = Depths.Skill;
			if (animation.Rotation < 0) {
				depth = -Depths.Skill;
			}
			animation.Attack.LayerDepth = Camera.Depth(characterSprite.Position, depth);
			animation.Attack.Render(Batch);
		}

	}

}
