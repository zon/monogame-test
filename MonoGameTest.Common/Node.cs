namespace MonoGameTest.Common {

	public class Node {
		public readonly Position Position;
		public readonly bool Solid;

		public Coord Coord {
			get {
				return Position.Coord;
			}
		}

		public long X {
			get {
				return Position.Coord.X;
			}
		}

		public long Y {
			get {
				return Position.Coord.Y;
			}
		}

		public Node(Coord coord, bool solid) {
			Position = new Position { Coord = coord };
			Solid = solid;
		}

		public override string ToString() => $"Node {Position.Coord.X}, {Position.Coord.Y}";

	}

}
