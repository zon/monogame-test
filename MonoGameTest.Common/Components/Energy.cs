using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Energy {
		public int Maximum;
		public float Amount;

		public const float REGEN = 0.5f;

		public float Percentage => Amount / Maximum;
		public bool IsFull => Amount >= Maximum;
		public bool IsEmpty => Amount <= 0;

		public Energy(int maximum, int amount) {
			Maximum = maximum;
			Amount = amount;
		}

		public Energy(int maximum) {
			Maximum = maximum;
			Amount = maximum;
		}

		public bool CanAfford(Command command) {
			if (!command.HasSkill) return true;
			return command.Skill.Energy <= Amount;
		}

		public void Spend(in Entity entity, Skill skill) {
			if (skill.Energy == 0) return;
			Amount -= skill.Energy;
			entity.NotifyChanged<Energy>();
		}

	}

}
