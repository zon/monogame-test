using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class DeathSystem : AEntitySetSystem<float> {
		readonly Context Context;
		
		public DeathSystem(Context context) : base(context.World
			.GetEntities()
			.With<Health>()
			.AsSet()	
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var health = ref entity.Get<Health>();
			if (health.Amount <= 0) {
				Context.Recorder.Record(entity).Dispose();
			}
		}

	}

}
