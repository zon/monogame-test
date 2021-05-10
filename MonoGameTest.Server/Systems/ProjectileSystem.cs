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
			
			if (projectile.Timeout > 0) return;

			if (projectile.Target.IsAlive) {
				ref var health = ref projectile.Target.Get<Health>();
				var damage = projectile.Attack.Damage;
				health.Amount = Calc.Max(health.Amount - damage, 0);
				projectile.Target.NotifyChanged<Health>();
			}

			Context.Recorder.Record(entity).Dispose();
		}

	}

}
 