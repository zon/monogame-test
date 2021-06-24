namespace MonoGameTest.Common {

	public class Spawn {
		public readonly int TileId;
		public readonly Coord Coord;
		public readonly Group Group;
		public readonly int? RoleId;

		public Spawn(int tileId, Coord coord, Group group, int? roleId) {
			TileId = tileId;
			Coord = coord;
			Group = group;
			RoleId = roleId;
		}

		public static Spawn Player(int tileId, Coord coord) {
			return new Spawn(tileId, coord, Group.Player, null);
		}

		public static Spawn Mob(int tileId, Coord coord, Group group, int roleId) {
			return new Spawn(tileId, coord, group, roleId);
		}

	}

}
