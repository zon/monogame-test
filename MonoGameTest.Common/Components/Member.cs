using System;

namespace MonoGameTest.Common {

	public struct Member : IEquatable<Member> {
		public readonly Group Group;

		public Member(Group group) {
			Group = group;
		}

		public override int GetHashCode() => Group.GetHashCode();

		public override bool Equals(object obj) => obj is Position other && Equals(other);

		public bool Equals(Member other) => Group == other.Group;

		public override string ToString() => $"Member: {Group}";

		public static bool operator ==(Member left, Member right) => left.Equals(right);

		public static bool operator !=(Member left, Member right) => !left.Equals(right);

	}

}
