using System.Collections.Immutable;

namespace MonoGameTest.Common {

	public class Role {
		public readonly int Id;
		public readonly Attack PrimaryAttack;
		public readonly ImmutableArray<Attack> Skills;

		static int AutoId;

		static readonly ImmutableArray<Role> Collection = ImmutableArray.Create(
			new Role(
				Attack.Get(1),
				Attack.Get(2)
			),
			new Role(
				Attack.Get(2)
			)
		);

		public static Role Get(int id) {
			var index = id - 1;
			if (index < 0 || index > Collection.Length) return null;
			return Collection[index];
		}

		public Role(Attack primaryAttack, params Attack[] skills) {
			Id = ++AutoId;
			PrimaryAttack = primaryAttack;
			Skills = ImmutableArray.Create(skills);
		}

	}

}
