using System;

namespace MonoGameTest.Common {

	public struct CharacterId : IEquatable<CharacterId> {
		public readonly int Id;

		static int AutoId = 0;

		public CharacterId(int id) {
			Id = id;
		}

		public override int GetHashCode() => Id;

		public override bool Equals(object obj) => obj is CharacterId other && Equals(other);

		public bool Equals(CharacterId other) => Id == other.Id;

		public override string ToString() => $"CharacterId: {Id}";

		public static bool operator ==(CharacterId left, CharacterId right) => left.Equals(right);

		public static bool operator !=(CharacterId left, CharacterId right) => !left.Equals(right);

		public static CharacterId Create() {
			return new CharacterId(++AutoId);
		}

	}

}
