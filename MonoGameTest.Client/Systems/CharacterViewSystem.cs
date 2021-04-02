using DefaultEcs;
using DefaultEcs.System;
using MonoGame.Extended.Tiled;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class CharacterViewSystem : AEntitySetSystem<float> {
		readonly TiledMap TiledMap;

		public CharacterViewSystem(World world, TiledMap tiledMap) : base(world
			.GetEntities()
			.With<Position>()
			.With<Sprite>()
			.AsSet()
		) {
			TiledMap = tiledMap;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var position = ref entity.Get<Position>();
			ref var sprite = ref entity.Get<Sprite>();
			sprite.Position = Tiled.CoordToVector(TiledMap, position.Coord);
		}

	}

}
