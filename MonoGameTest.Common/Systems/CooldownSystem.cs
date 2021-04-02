using System;
using DefaultEcs;
using DefaultEcs.System;

namespace MonoGameTest.Common {

	public class CooldownSystem : AEntitySetSystem<float> {

		public CooldownSystem(World world) : base(world
			.GetEntities()
			.With<Cooldown>()
			.AsSet()
		) {}

		protected override void Update(float dt, in Entity entity)
		{
			ref var cooldown = ref entity.Get<Cooldown>();

			if (cooldown.action > 0) {
				cooldown.action = Math.Max(cooldown.action - dt, 0);
			} else {
				cooldown.pause = Math.Max(cooldown.pause - dt, 0);
			}
		}

	}

}