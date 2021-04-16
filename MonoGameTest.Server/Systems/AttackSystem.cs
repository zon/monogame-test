using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class AttackSystem : AEntitySetSystem<float> {
		readonly Context Context;
		
		public AttackSystem(Context context) : base(context.World
			.GetEntities()
			.With<Attack>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var cooldown = ref entity.Get<Cooldown>();
			if (!cooldown.IsCool()) return;

			ref var target = ref entity.Get<Target>();
			if (!target.HasEntity) return;
			var targetEntity = target.Entity.Value;

			ref var position = ref entity.Get<Position>();
			ref var targetPosition = ref targetEntity.Get<Position>();
			var d = Coord.ChebyshevDistance(targetPosition.Coord, position.Coord);
			if (d > 1) return;

			ref var attack = ref entity.Get<Attack>();
			ref var health = ref targetEntity.Get<Health>();
			health.Amount = Calc.Max(health.Amount - attack.Damage, 0);
			cooldown.action = Attack.ACTION_DURATION;
			cooldown.pause = Attack.PAUSE_DURATION;

			ref var character = ref entity.Get<Character>();
			Context.Server.SendToAll(new AttackPacket {
				CharacterId = character.Id,
				TargetX = targetPosition.Coord.X,
				TargetY = targetPosition.Coord.Y,
				Damage = attack.Damage,
				Duration = Attack.ACTION_DURATION
			});

			targetEntity.NotifyChanged<Health>();
		}

	}

}
