using System;
using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class CharacterSystem : AEntitySetSystem<float> {
		readonly Context Context;

		public CharacterSystem(Context context) : base(context.World
			.GetEntities()
			.With<Character>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var character = ref entity.Get<Character>();
			if (character.IsIdle) return;

			character.Timeout = Math.Max(character.Timeout - dt, 0);
			if (character.Timeout > 0) return;

			switch (character.State) {

				case CharacterState.Cooldown:
					character.State = CharacterState.Idle;
					break;
				
				case CharacterState.SkillFollow:
					character.State = CharacterState.Cooldown;
					character.Timeout = character.Skill.Cooldown;
					break;
				
				case CharacterState.SkillLead:
					if (character.Target.IsAlive) {
						if (character.Skill.IsMelee) {
							ref var health = ref character.Target.Get<Health>();
							var damage = character.Skill.Damage;
							health.Amount = Calc.Max(health.Amount - damage, 0);
							character.Target.NotifyChanged<Health>();
						} else {
							ref var position = ref entity.Get<Position>();
							Factory.SpawnProjectile(Context, position.Coord, character.Target, character.Skill);
						}
					}
					character.State = CharacterState.SkillFollow;
					character.Timeout = character.Skill.Follow;
					break;

			}
		}

	}

}
