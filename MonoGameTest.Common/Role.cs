using System.Collections.Immutable;

namespace MonoGameTest.Common {

	public class Role {
		public readonly int Id;
		public readonly Attributes Attributes;
		public readonly Skill PrimarySkill;
		public readonly ImmutableArray<Skill> Skills;

		static int AutoId;

		static readonly ImmutableArray<Role> Collection = ImmutableArray.Create(
			new Role(
				new Attributes {
					Sprite = 5,
					Power = 5,
					MoveCoolown = 1,
					Health = 100,
					Energy = 100,
					EnergyGen = 5
				},
				Skill.Get(1),
				Skill.Get(3),
				Skill.Get(4),
				Skill.Get(5)
			),
			new Role(
				new Attributes {
					Sprite = 4,
					Power = 4,
					MoveCoolown = 1,
					Health = 50,
					Energy = 100,
					EnergyGen = 5
				},
				Skill.Get(2)
			)
		);

		public static Role Get(int id) {
			var index = id - 1;
			if (index < 0 || index > Collection.Length) return null;
			return Collection[index];
		}

		public Role(Attributes attributes, Skill primarySkill, params Skill[] skills) {
			Id = ++AutoId;
			Attributes = attributes;
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
