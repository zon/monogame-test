using System;
using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class ProjectileSystem : AEntitySetSystem<float> {
		readonly Context Context;
		
		public ProjectileSystem(Context context) : base(context.World
			.GetEntities()
			.With<Projectile>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var projectile = ref entity.Get<Projectile>();
			projectile.Timeout = MathF.Max(projectile.Timeout - dt, 0);

			if (projectile.Target.IsAlive) {
				projectile.TargetCoord = projectile.Target.Get<Position>().Coord;
			}
			
			if (projectile.Timeout > 0) return;

			if (projectile.Attributes != null) {
				var attributes = projectile.Attributes.Value;
				var skill = projectile.Skill;
				if (projectile.Skill.HasAreaEffect) {
					var area = new RadiusArea(projectile.TargetCoord, projectile.Skill.Area);
					foreach (var coord in area) {
						Entity other;
						if (!Context.Positions.TryGetEntity(new Position { Coord = coord }, out other)) continue;
						CharacterSystem.Impact(Context, attributes, skill, other);
					}

				} else {
					CharacterSystem.Impact(Context, attributes, skill, projectile.Target);
				}
			}

			

			Context.Recorder.Record(entity).Dispose();
		}

	}

}
 