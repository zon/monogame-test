using System;
using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class BuffEffectSystem : AEntitySetSystem<float> {
		readonly Context Context;

		public BuffEffectSystem(Context context) : base(context.World
			.GetEntities()
			.With<BuffEffect>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			var characterId = entity.Get<CharacterId>();
			Entity target;
			if (!Context.CharacterIds.TryGetEntity(characterId, out target) || !target.IsAlive) {
				Context.Recorder.Record(entity).Dispose();
				return;
			}

			ref var effect = ref entity.Get<BuffEffect>();
			var a = Calc.Floor(effect.Timeout / Buff.INTERVAL);
			effect.Timeout = Math.Max(effect.Timeout - dt, 0);
			var b = Calc.Floor(effect.Timeout / Buff.INTERVAL);

			if (a != b) {
				ref var health = ref target.Get<Health>();
				health.Amount = Math.Max(
					health.Amount - effect.Buff.HealthPerSecond * (a - b) * Buff.INTERVAL,
					0
				);
				target.NotifyChanged<Health>();
			}

			if (effect.Timeout <= 0) {
				Context.Recorder.Record(entity).Dispose();
			}
		}

	}

}
