using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class MobTargetSystem : AEntitySetSystem<float> {
		readonly Context Context;
		readonly EntityMap<Position> Positions;

		public MobTargetSystem(Context context) : base(context.World
			.GetEntities()
			.With<Character>()
			.With<Cooldown>()
			.With<Position>()
			.With<Target>()
			.AsSet()
		) {
			Context = context;
			Positions = World.GetEntities().With<Character>().AsMap<Position>();
		}

		protected override void Update(float state, in Entity entity) {
		
			ref var cooldown = ref entity.Get<Cooldown>();

			if (!cooldown.IsCool()) return;

			ref var character = ref entity.Get<Character>();
			ref var position = ref entity.Get<Position>();
			ref var target = ref entity.Get<Target>();

			/*
			TODO:
				* filter positions into possible targets
				* search grid for nearest target
				* set target entity
			*/
		}


	}

}
