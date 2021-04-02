using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public static class ClientEntity {

		public static Entity CreatePlayer(World world, Coord coord, Sprite sprite) {
			var player = world.CreateEntity();
			player.Set(Character.Create());
			player.Set(Player.Create());
			player.Set(new Position { Coord = coord });
			player.Set(new Movement());
			player.Set(new Cooldown());
			player.Set(sprite);
			player.Set(new MovementInput());
			return player;
		}

	}

}
