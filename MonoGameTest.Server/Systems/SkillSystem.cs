using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class SkillSystem : AEntitySetSystem<float> {
		readonly Context Context;

		public SkillSystem(Context context) : base(context.World
			.GetEntities()
			.With<Character>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var character = ref entity.Get<Character>();
			var skill = character.Role.PrimarySkill;
			if (!character.IsIdle) return;

			ref var target = ref entity.Get<Target>();
			if (!target.HasEntity) return;
			var targetEntity = target.Entity.Value;

			ref var position = ref entity.Get<Position>();
			ref var targetPosition = ref targetEntity.Get<Position>();

			if (!skill.InRange(position, targetPosition)) return;

			if (!skill.IsMelee) {
				var pathfinder = Context.CreatePathfinder();
				if (!pathfinder.HasSight(position.Coord, targetPosition.Coord)) return;
			}

			character.StartSkill(skill, targetEntity);

			ref var characterId = ref entity.Get<CharacterId>();
			ref var targetCharacterId = ref targetEntity.Get<CharacterId>();
			Context.Server.SendToAll(new SkillPacket {
				SkillId = skill.Id,
				OriginCharacterId = characterId.Id,
				TargetCharacterId = targetCharacterId.Id
			});
		}

	}

}
