using System;

namespace MonoGameTest.Common {

	public struct BuffEffectId : IEquatable<BuffEffectId> {
		public readonly int Id;

		static int AutoId = 0;

		public BuffEffectId(int id) {
			Id = id;
		}

		public override int GetHashCode() => Id;

		public override bool Equals(object obj) => obj is BuffEffectId other && Equals(other);

		public bool Equals(BuffEffectId other) => Id == other.Id;

		public override string ToString() => $"BuffEffectId: {Id}";

		public static bool operator ==(BuffEffectId left, BuffEffectId right) => left.Equals(right);

		public static bool operator !=(BuffEffectId left, BuffEffectId right) => !left.Equals(right);

		public static BuffEffectId Create() {
			return new BuffEffectId(++AutoId);
		}

	}

}
