namespace MonoGameTest.Common {

	public class Buff {
		public readonly int Id;
		public readonly Skill Skill;
		public readonly int Duration;
		public readonly Attributes? Attributes;
		public readonly int HealthPerSecond;

		public const int INTERVAL = 1;

		static int AutoId = 0;

		public Buff(int duration, Attributes? attributes = null, int healthPerSecond = 0) {
			Id = 0;
			Skill = null;
			Duration = duration;
			Attributes = attributes;
			HealthPerSecond = healthPerSecond;
		}

		public Buff(Skill skill, int duration, Attributes? attributes, int healthPerSecond) {
			Id = ++AutoId;
			Skill = skill;
			Duration = duration;
			Attributes = attributes;
			HealthPerSecond = healthPerSecond;
		}

		public Buff Complete(Skill skill) {
			return new Buff(skill, Duration, Attributes, HealthPerSecond);
		}

		public static Buff DamageOverTime(int duration, int damagePerSecond) {
			return new Buff(duration, healthPerSecond: damagePerSecond);
		}

	}

}
