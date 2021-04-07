using System;

namespace MonoGameTest.Common {

	public struct Player : IEquatable<Player> {
		public readonly int PeerId;

		public Player(int peerId) {
			PeerId = peerId;
		}

		public override int GetHashCode() => PeerId;

		public override bool Equals(object obj) => obj is Player other && Equals(other);

		public bool Equals(Player other) => PeerId == other.PeerId;

		public override string ToString() => $"Player: {PeerId}";

		public static bool operator ==(Player left, Player right) => left.Equals(right);

		public static bool operator !=(Player left, Player right) => !left.Equals(right);

	}

}

