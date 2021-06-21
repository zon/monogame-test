using System;
using DefaultEcs;
using DefaultEcs.System;

namespace MonoGameTest.Common {

	public class EnergySystem : AEntitySetSystem<float> {
		readonly IContext Context;
		
		public EnergySystem(IContext context) : base(context.World
			.GetEntities()
			.With<Energy>()
			.AsSet()	
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var energy = ref entity.Get<Energy>();
			energy.Amount = Math.Min(energy.Amount + dt * Energy.REGEN, (float) energy.Maximum);
		}

	}

}
