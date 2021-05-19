using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Character {
		public Role Role;
		public CharacterState State;
		public float Timeout;
		public Skill Skill;
		public Entity Target;

		public bool IsIdle => State == CharacterState.Idle;

		public void StartCooldown(float timeout) {
			State = CharacterState.Cooldown;
			Timeout = timeout;
		}

		public void StartSkill(Skill skill, Entity target) {
			State = CharacterState.SkillLead;
			Timeout = skill.Lead;
			Skill = skill;
			Target = target;
		}

	}

}
