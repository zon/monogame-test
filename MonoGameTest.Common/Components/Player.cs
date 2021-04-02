using System;

namespace MonoGameTest.Common {

	public struct Player : IEquatable<Player> {
		public readonly int Id;

		static int AutoId = 0;

		Player(int id) {
			Id = id;
		}

		public override int GetHashCode() => Id;

		public override bool Equals(object obj) => obj is Player other && Equals(other);

		public bool Equals(Player other) => Id == other.Id;

		public override string ToString() => $"Player: {Id}";

		public static bool operator ==(Player left, Player right) => left.Equals(right);

		public static bool operator !=(Player left, Player right) => !left.Equals(right);

		public static Player Create() {
			return new Player(++AutoId);
		}

	}

}

