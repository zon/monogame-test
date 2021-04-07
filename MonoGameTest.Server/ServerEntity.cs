using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public static class ServerEntity {

		public static Entity CreatePlayer(World world, int peerId, Coord coord) {
			var player = world.CreateEntity();
			player.Set(Character.Create());
			player.Set(new Player(peerId));
			player.Set(new Position { Coord = coord });
			player.Set(new Movement());
			player.Set(new Cooldown());
			return player;
		}

	}

}
