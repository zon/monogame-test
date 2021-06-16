namespace MonoGameTest.Common {

	public class Buff {
		public readonly int Id;
		public readonly Skill Skill;
		public readonly int Duration;
		public readonly int HealthPerSecond;

		public const int INTERVAL = 1;

		static int AutoId = 0;

		public Buff(int duration, int healthPerSecond) {
			Id = 0;
			Skill = null;
			Duration = duration;
			HealthPerSecond = healthPerSecond;
		}

		public Buff(Skill skill, int duration, int healthPerSecond) {
			Id = ++AutoId;
			Skill = skill;
			Duration = duration;
			HealthPerSecond = healthPerSecond;
		}

		public Buff Complete(Skill skill) {
			return new Buff(skill, Duration, HealthPerSecond);
		}

	}

}
