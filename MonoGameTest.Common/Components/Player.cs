using System;

namespace MonoGameTest.Common {

	public struct Player : IEquatable<Player> {
		public readonly int SessionId;

		public Player(int sessionId) {
			SessionId = sessionId;
		}

		public override int GetHashCode() => SessionId;

		public override bool Equals(object obj) => obj is Player other && Equals(other);

		public bool Equals(Player other) => SessionId == other.SessionId;

		public override string ToString() => $"Player: {SessionId}";

		public static bool operator ==(Player left, Player right) => left.Equals(right);

		public static bool operator !=(Player left, Player right) => !left.Equals(right);

	}

}

