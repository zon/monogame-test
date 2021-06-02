using System.Collections.Immutable;

namespace MonoGameTest.Common {

	public class Role {
		public readonly int Id;
		public readonly float MoveCooldown;
		public readonly Skill PrimarySkill;
		public readonly ImmutableArray<Skill> Skills;

		static int AutoId;

		static readonly ImmutableArray<Role> Collection = ImmutableArray.Create(
			new Role(
				1,
				Skill.Get(1),
				Skill.Get(3),
				Skill.Get(4)
			),
			new Role(
				1,
				Skill.Get(2)
			)
		);

		public static Role Get(int id) {
			var index = id - 1;
			if (index < 0 || index > Collection.Length) return null;
			return Collection[index];
		}

		public Role(int moveCooldown, Skill primarySkill, params Skill[] skills) {
			Id = ++AutoId;
			MoveCooldown = moveCooldown;
			PrimarySkill = primarySkill;
			Skills = ImmutableArray.Create(skills);
		}

		public Skill GetSkill(int id) {
			foreach (var skill in Skills) {
				if (skill.Id == id) return skill;
			}
			return null;
		}

	}

}
