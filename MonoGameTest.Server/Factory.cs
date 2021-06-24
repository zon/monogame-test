using System.Linq;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public static class Factory {

		public static Entity SpawnPlayer(Context context, Session session) {
			var role = Role.Get(1);
			var spawn = context.Grid.Spawns.First(s => s.Group == Group.Player);
			var node = context.Grid.GetOpenNearby(context.Positions, spawn.Coord);
			var c = SpawnCharacter(context.World, role, Group.Player);
			c.Set(new Player(session.Id));
			c.Set(new Position { Coord = node.Coord });
			return c;
		}

		public static Entity SpawnMob(World world, int roleId, Group group, Coord coord) {
			var role = Role.Get(roleId);
			var c = SpawnCharacter(world, role, group);
			c.Set(new Mob());
			c.Set(new Position { Coord = coord });
			return c;
		}

		public static Entity SpawnMob(World world, Spawn spawn) {
			return SpawnMob(world, spawn.RoleId.Value, spawn.Group, spawn.Coord);
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
			Group group = Group.Red
		) {
			var c = SpawnCharacter(world, role, group);
			c.Set(new Position { Coord = coord });
			return c;
		}

		public static Entity SpawnProjectile(Context context, in Entity origin, Skill skill, in Entity target) {
			var e = context.World.CreateEntity();
			ref var position = ref origin.Get<Position>();
			ref var attributes = ref origin.Get<Attributes>();
			e.Set(new Projectile(position.Coord, target, skill, attributes));
			return e;
		}

		static Entity SpawnCharacter(
			World world,
			Role role,
			Group group = Group.Red
		) {
			var c = world.CreateEntity();
			c.Set(group);
			c.Set(role.Attributes);
			c.Set(new Health(role.Attributes));
			c.Set(new Energy(role.Attributes));
			c.Set(new Character(role));
			c.Set(CharacterId.Create());
			return c;
		}

	}

}
