using System.Collections.Immutable;

namespace MonoGameTest.Common {

	public class Role {
		public readonly int Id;
		public readonly Skill PrimarySkill;
		public readonly ImmutableArray<Skill> Skills;

		static int AutoId;

		static readonly ImmutableArray<Role> Collection = ImmutableArray.Create(
			new Role(
				Skill.Get(1),
				Skill.Get(2),
				Skill.Get(3)
			),
			new Role(
				Skill.Get(2)
			)
		);

		public static Role Get(int id) {
			var index = id - 1;
			if (index < 0 || index > Collection.Length) return null;
			return Collection[index];
		}

		public Role(Skill primarySkill, params Skill[] skills) {
			Id = ++AutoId;
			PrimarySkill = primarySkill;
			Skills = ImmutableArray.Create(skills);
		}

	}

}
