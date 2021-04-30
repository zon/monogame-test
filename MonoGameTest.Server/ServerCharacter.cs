using System.Linq;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public static class ServerCharacter {

		public static Entity SpawnPlayer(Context context, int peerId) {
			var spawn = context.Grid.Spawns.First(s => s.Group == Group.Player);
			var node = context.Grid.GetOpenNearby(context.Positions, spawn.Coord);
			var c = context.World.CreateEntity();
			c.Set(Group.Player);
			c.Set(new Attributes(5));
			c.Set(new Health(100));
			c.Set(new Movement());
			c.Set(new Attack { Damage = 5 });
			c.Set(new Cooldown());
			c.Set(new Target());
			c.Set(Character.Create());
			c.Set(new Player(peerId));
			c.Set(new Position { Coord = node.Coord });
			return c;
		}

		public static Entity SpawnMob(World world, Group group, int sprite, Coord coord) {
			var c = world.CreateEntity();
			c.Set(group);
			c.Set(new Attributes(sprite));
			c.Set(new Health(100));
			c.Set(new Attack { Damage = 5 });
			c.Set(new Cooldown());
			c.Set(new Movement());
			c.Set(new Target());
			c.Set(Character.Create());
			c.Set(new Mob());
			c.Set(new Position { Coord = coord });
			return c;
		}

		public static Entity SpawnMob(World world, Spawn spawn) {
			return SpawnMob(world, spawn.Group, spawn.Sprite, spawn.Coord);
		}

		public static void SpawnMobs(Grid grid, World world) {
			for (var s = 0; s < grid.Spawns.Length; s++) {
				var spawn = grid.Spawns[s];
				if (spawn.Group == Group.Player) continue;
				SpawnMob(world, spawn);
			}
		}

	}

}
