using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Character {
		public CharacterState State;
		public float Timeout;
		public Attack PrimaryAttack;
		public Attack Attack;
		public Entity Target;

		public bool IsIdle => State == CharacterState.Idle;

		public void StartCooldown(float timeout) {
			State = CharacterState.Cooldown;
			Timeout = timeout;
		}

		public void StartAttack(Attack attack, Entity target) {
			State = CharacterState.AttackLead;
			Timeout = attack.Lead;
			Attack = attack;
			Target = target;
		}

	}

}
