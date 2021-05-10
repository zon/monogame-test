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
				
				case CharacterState.AttackFollow:
					character.State = CharacterState.Cooldown;
					character.Timeout = character.Attack.Cooldown;
					break;
				
				case CharacterState.AttackLead:
					if (character.Target.IsAlive) {
						if (character.Attack.IsMelee) {
							ref var health = ref character.Target.Get<Health>();
							var damage = character.Attack.Damage;
							health.Amount = Calc.Max(health.Amount - damage, 0);
							character.Target.NotifyChanged<Health>();
						} else {
							ref var position = ref entity.Get<Position>();
							Factory.SpawnProjectile(Context, position.Coord, character.Target, character.Attack);
						}
					}
					character.State = CharacterState.AttackFollow;
					character.Timeout = character.Attack.Follow;
					break;

			}
		}

	}

}
