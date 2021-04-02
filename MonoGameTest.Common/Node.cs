namespace MonoGameTest.Common {

	public class Node {
		public readonly Position Position;
		public readonly bool Solid;

		public Coord Coord {
			get {
				return Position.Coord;
			}
		}

		public int X {
			get {
				return Position.Coord.X;
			}
		}

		public int Y {
			get {
				return Position.Coord.Y;
			}
		}

		public Node(int x, int y, bool solid) {
			Position = new Position { Coord = new Coord(x, y) };
			Solid = solid;
		}

		public override string ToString() => $"Node {Position.Coord.X}, {Position.Coord.Y}";

	}

}
