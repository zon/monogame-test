using System;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest.Client {

	public class HitAnimationSystem : AEntitySetSystem<float> {
		readonly Context Context;
		readonly Random Random;

		const float MAGNITUDE = 1;

		SpriteBatch Batch => Context.Foreground;
		Camera Camera => Context.Camera;

		public HitAnimationSystem(Context context) : base(context.World
			.GetEntities()
			.With<HitAnimation>()
			.AsSet()
		) {
			Context = context;
			Random = new Random();
		}

		protected override void Update(float dt, in Entity entity) {
			ref var hit = ref entity.Get<HitAnimation>();
			if (!hit.IsActive) return;
			hit.Sprite.Update(dt);
			if (!hit.IsActive) return;
			ref var sprite = ref entity.Get<Sprite>();
			var offset = new Vector2(
				Random.NextFloat() * 2 - 1,
				Random.NextFloat() * 2 - 1
			) * MAGNITUDE;
			sprite.Position += offset;
			hit.Sprite.Position = sprite.Position + sprite.Rectangle.Size.ToVector2() / 2;
			hit.Sprite.LayerDepth = Camera.Depth(sprite.Position, Depths.Hit);
			hit.Sprite.Render(Batch);
		}

	}

}
