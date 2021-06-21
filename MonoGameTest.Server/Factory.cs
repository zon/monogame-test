using System.Linq;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public static class Factory {

		public static Entity SpawnPlayer(Context context, Session session) {
			var role = Role.Get(1);
			var spawn = context.Grid.Spawns.First(s => s.Group == Group.Player);
			var node = context.Grid.GetOpenNearby(context.Positions, spawn.Coord);
			var sprite = 5;
			var c = SpawnCharacter(context.World, role, Group.Player, sprite);
			c.Set(new Player(session.Id));
			c.Set(new Position { Coord = node.Coord });
			return c;
		}

		public static Entity SpawnMob(World world, Group group, int sprite, Coord coord) {
			var role = Role.Get(2);
			var c = SpawnCharacter(world, role, group, sprite);
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

		public static Entity SpawnCharacter(
			World world,
			Role role,
			Coord coord,
			Group group = Group.Red,
			int sprite = 4
		) {
			var c = SpawnCharacter(world, role, group, sprite);
			c.Set(new Position { Coord = coord });
			return c;
		}

		public static Entity SpawnProjectile(Context context, Coord origin, Entity target, Skill skill) {
			var e = context.World.CreateEntity();
			e.Set(new Projectile(origin, target, skill));
			return e;
		}

		static Entity SpawnCharacter(
			World world,
			Role role,
			Group group = Group.Red,
			int sprite = 4
		) {
			var c = world.CreateEntity();
			c.Set(group);
			c.Set(new Attributes(sprite));
			c.Set(new Health(100));
			c.Set(new Energy(10));
			c.Set(new Character(role));
			c.Set(CharacterId.Create());
			return c;
		}

	}

}
