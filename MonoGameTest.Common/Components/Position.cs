using System;

namespace MonoGameTest.Common {

	public struct Position : IEquatable<Position> {
		public Coord Coord;

		public override int GetHashCode() => Coord.GetHashCode();

		public override bool Equals(object obj) => obj is Position other && Equals(other);

		public bool Equals(Position other) => Coord == other.Coord;

		public override string ToString() => $"Position: {Coord.X}, {Coord.Y}";

		public static bool operator ==(Position left, Position right) => left.Equals(right);

		public static bool operator !=(Position left, Position right) => !left.Equals(right);

	}

}
