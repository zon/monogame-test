using System;

namespace MonoGameTest.Common {

	public struct Character : IEquatable<Character> {
		public readonly int Id;

		static int AutoId = 0;

		Character(int id) {
			Id = id;
		}

		public override int GetHashCode() => Id;

		public override bool Equals(object obj) => obj is Character other && Equals(other);

		public bool Equals(Character other) => Id == other.Id;

		public override string ToString() => $"Character: {Id}";

		public static bool operator ==(Character left, Character right) => left.Equals(right);

		public static bool operator !=(Character left, Character right) => !left.Equals(right);

		public static Character Create() {
			return new Character(++AutoId);
		}

	}

}
