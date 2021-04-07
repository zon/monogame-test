using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class CharacterViewSystem : AEntitySetSystem<float> {
		readonly Context Context;

		public CharacterViewSystem(World world, Context context) : base(world
			.GetEntities()
			.With<Position>()
			.With<Sprite>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var position = ref entity.Get<Position>();
			ref var sprite = ref entity.Get<Sprite>();
			sprite.Position = Context.CoordToVector(position.Coord);
		}

	}

}
