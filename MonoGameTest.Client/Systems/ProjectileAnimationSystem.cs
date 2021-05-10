using System;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class ProjectileAnimationSystem : AEntitySetSystem<float> {
		readonly Context Context;
		
		SpriteBatch Batch => Context.Foreground;
		Camera Camera => Context.Camera;

		public ProjectileAnimationSystem(Context context) : base(context.World
			.GetEntities()
			.With<Projectile>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var projectile = ref entity.Get<Projectile>();
			ref var view = ref entity.Get<ProjectileView>();

			if (projectile.Target.IsAlive) {
				ref var targetSprite = ref projectile.Target.Get<Sprite>();
				view.TargetPosition = targetSprite.Position;
			}

			var a = Context.CoordToMidVector(projectile.Origin);
			var b = view.TargetPosition + Context.HalfTileSize;

			view.Sprite.Position = Vector2.Lerp(b, a, projectile.Timeout / projectile.Lifetime);
			view.Sprite.Rotation = View.ToRadians(b - a);
			view.Sprite.LayerDepth = Camera.Depth(view.Sprite.Position);
			view.Sprite.Render(Batch);
			view.Sprite.Update(dt);

			projectile.Timeout = Math.Max(projectile.Timeout - dt, 0);
			if (projectile.Timeout <= 0) {
				Context.Recorder.Record(entity).Dispose();
			}
		}

	}

}
