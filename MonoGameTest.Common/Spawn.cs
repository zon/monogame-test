namespace MonoGameTest.Common {

	public class Spawn {
		public readonly Coord Coord;
		public readonly Group Group;
		public readonly int? RoleId;

		public Spawn(Coord coord, Group group, int? roleId) {
			Coord = coord;
			Group = group;
			RoleId = roleId;
		}

		public static Spawn Player(Coord coord) {
			return new Spawn(coord, Group.Player, null);
		}

		public static Spawn Mob(Coord coord, Group group, int roleId) {
			return new Spawn(coord, group, roleId);
		}

	}

}
