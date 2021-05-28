using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Command {
		public readonly int Id;
		public readonly Target? Target;
		public readonly Skill Skill;

		static int AutoId = 0;

		public bool HasTarget => Target != null;
		public bool HasSkill => Skill != null;
		public bool IsMove => Skill == null;
		public bool IsPause => Skill == null && Target == null;
		public bool IsRepeating => IsMove || Skill?.Repeating == true;
		public bool IsValid => Target?.IsValid ?? true;

		public Command(Target? target = null, Skill skill = null) {
			Id = ++AutoId;
			Target = target;
			Skill = skill;
		}

		public static Command Targeting(Coord coord, Skill skill = null) {
			return new Command(new Target(coord), skill);
		}
		
		public static Command Targeting(Entity entity, Skill skill = null) {
			return new Command(new Target(entity), skill);
		}

		public static Command Pause() {
			return new Command();
		}

	}

}
