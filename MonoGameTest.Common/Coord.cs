using System;

namespace MonoGameTest.Common {

	public struct Coord : IEquatable<Coord> {
		public readonly int X;
		public readonly int Y;

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

	}

}
