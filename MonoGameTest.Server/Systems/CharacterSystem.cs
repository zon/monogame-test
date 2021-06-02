using System;
using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class CharacterSystem : AEntitySetSystem<float> {
		readonly Context Context;

		Server Server => Context.Server;

		public CharacterSystem(Context context) : base(context.World
			.GetEntities()
			.With<Character>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var character = ref entity.Get<Character>();
			character.Update(dt);
			if (character.IsIdle) return;

			character.Timeout = Math.Max(character.Timeout - dt, 0);
			if (character.Timeout > 0) return;

			// let movement system handle move commands
			if (!character.HasCommand) return;
			var command = character.GetCurrentCommand().Value;
			if (command.IsMove && character.State == CharacterState.Standby) return;
			var skill = command.Skill;

			// only leave standby if there is a valid target
			if (command.HasSkill && character.State == CharacterState.Standby) {
				var characterId = entity.Get<CharacterId>().Id;
				if (command.HasTarget) {
					var position = entity.Get<Position>().Coord;
					var target = command.Target.Value;
					var targetPosition = target.GetPosition();

					if (skill.IsMelee) {
						if (!skill.IsValidMeleeTarget(position, targetPosition)) return;
					} else {
						var pathfinder = Context.CreatePathfinder();
						if (!skill.IsValidTarget(pathfinder, position, targetPosition)) return;
					}
					
					// target is valid
					// broadcast skill start packet
					if (target.IsMobile) {
						var targetCharacterId = target.Entity.Value.Get<CharacterId>().Id;
						Server.SendToAll(new SkillStartMobilePacket {
							SkillId = skill.Id,
							OriginCharacterId = characterId,
							TargetCharacterId = targetCharacterId
						});

					} else {
						var coord = target.Coord.Value;
						Server.SendToAll(new SkillStartFixedPacket {
							SkillId = skill.Id,
							OriginCharacterId = characterId,
							TargetX = coord.X,
							TargetY = coord.Y
						});

					}

				} else {
					Server.SendToAll(new SkillStartPacket {
						SkillId = skill.Id,
						OriginCharacterId = characterId
					});
				}
			}

			// advance state
			character.NextState(entity, skill);

			// perform generic skill behavior
			if (command.HasSkill && character.State == CharacterState.Active) {

				// only entity target bahavior is supported so far
				if (command.Target?.Entity != null) {
					var targetEntity = command.Target.Value.Entity.Value;
					if (targetEntity.IsAlive) {

						if (skill.IsMelee) {
							ref var health = ref targetEntity.Get<Health>();
							var damage = skill.Damage;
							health.Amount = Calc.Max(health.Amount - damage, 0);
							targetEntity.NotifyChanged<Health>();

						} else {
							ref var position = ref entity.Get<Position>();
							Factory.SpawnProjectile(Context, position.Coord, targetEntity, skill);
						}
					}
				}
				
				// immediately advance state
				character.NextState(entity, skill);
			}
		}

	}

}
