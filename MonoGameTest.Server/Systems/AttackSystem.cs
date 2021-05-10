using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class AttackSystem : AEntitySetSystem<float> {
		readonly Context Context;

		public AttackSystem(Context context) : base(context.World
			.GetEntities()
			.With<Attack>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var character = ref entity.Get<Character>();
			if (!character.IsIdle) return;

			ref var target = ref entity.Get<Target>();
			if (!target.HasEntity) return;
			var targetEntity = target.Entity.Value;

			ref var attack = ref entity.Get<Attack>();
			ref var position = ref entity.Get<Position>();
			ref var targetPosition = ref targetEntity.Get<Position>();

			if (attack.IsMelee) {
				var d = Coord.ChebyshevDistance(targetPosition.Coord, position.Coord);
				if (d > 1) return;

			} else {
				var d = Coord.DistanceSquared(position.Coord, targetPosition.Coord);
				if (d > attack.Range * attack.Range) return;
				var pathfinder = new Pathfinder(Context.Grid, Context.Positions);
				if (!pathfinder.HasSight(position.Coord, targetPosition.Coord)) return;
			}

			character.StartAttack(attack, targetEntity);

			ref var characterId = ref entity.Get<CharacterId>();
			ref var targetCharacterId = ref targetEntity.Get<CharacterId>();
			Context.Server.SendToAll(new AttackPacket {
				AttackId = attack.Id,
				OriginCharacterId = characterId.Id,
				TargetCharacterId = targetCharacterId.Id
			});
		}

	}

}
