using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class CharacterSystem : AEntitySetSystem<float> {
		readonly Context Context;

		public CharacterSystem(Context context) : base(context.World
			.GetEntities()
			.With<Character>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var character = ref entity.Get<Character>();
			character.Update(dt);
		}

	}

}
