using System;

namespace MonoGameTest.Common {

	public struct Coord : IEquatable<Coord> {
		public readonly int X;
		public readonly int Y;

		public readonly static Coord Zero = new Coord(0, 0);
		public readonly static Coord One = new Coord(1, 1);
		public readonly static Coord Left = new Coord(-1, 0);
		public readonly static Coord Right = new Coord(1, 0);
		public readonly static Coord Up = new Coord(0, -1);
		public readonly static Coord Down = new Coord(0, 1);

		public Coord(int x, int y) {
			this.X = x;
			this.Y = y;
		}

		public override int GetHashCode() => X * 6983 + Y * 4003;

		public override bool Equals(object obj) => obj is Coord other && Equals(other);

		public bool Equals(Coord other) => X == other.X && Y == other.Y;

		public override string ToString() => $"Coord: {X}, {Y}";

		public static bool operator ==(Coord left, Coord right) => left.Equals(right);

		public static bool operator !=(Coord left, Coord right) => !left.Equals(right);

		public static Coord operator +(Coord left, Coord right) => new Coord(left.X + right.X, left.Y + right.Y);
		public static Coord operator -(Coord left, Coord right) => new Coord(left.X - right.X, left.Y - right.Y);

		public static Coord operator *(Coord coord, int value) => new Coord(coord.X * value, coord.Y * value);

		public static float ManhattanDistance(Coord a, Coord b) {
			return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) * Movement.COST;
		}

		public static float ChebyshevDistance(Coord a, Coord b) {
			var dx = Math.Abs(a.X - b.X);
			var dy = Math.Abs(a.Y - b.Y);
			var c = Movement.COST;
			var g = Movement.DIAGONAL_COST;
			if (dx > dy) {
				return (dx - dy) * c + dy * g;
			} else {
				return (dy - dx) * c + dx * g;
			}
		}

		public static float Distance(Coord a, Coord b) {
			return MathF.Sqrt(DistanceSquared(a, b));
		}

		public static float DistanceSquared(Coord a, Coord b) {
			var x = a.X - b.X;
			var y = a.Y - b.Y;
			return x * x + y * y;
		}

		public static Coord Facing(Coord from, Coord to) {
			var d = to - from;
			var x = Math.Abs(d.X);
			var y = Math.Abs(d.Y);
			if (y > x) {
				return new Coord(0, d.Y / y);
			} else {
				return new Coord(d.X / x, 0);
			}
		}

	}

}
