using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class MobTargetSystem : AEntitySetSystem<float> {
		readonly Context Context;
		readonly EntitySet Others;

		const float NOTHING_PAUSE = 1;

		public MobTargetSystem(Context context) : base(context.World
			.GetEntities()
			.With<Mob>()
			.AsSet()
		) {
			Context = context;
			Others = context.World.GetEntities().With<CharacterId>().With<Position>().AsSet();
		}

		protected override void Update(float state, in Entity entity) {
			ref var character = ref entity.Get<Character>();
			if (!character.IsIdle) return;

			var position = entity.Get<Position>();
			var group = entity.Get<Group>();
			var skill = character.Role.PrimarySkill;

			var found = false;
			var closest = new Closest(position.Coord, Others, e => {
				var g = e.Get<Group>();
				return g != group;
			});
			var pathfinder = Context.CreatePathfinder();
			foreach (var other in closest) {
				var otherPosition = other.Get<Position>();
				var res = pathfinder.MoveToSkill(position.Coord, otherPosition.Coord, skill);
				if (!res.IsGoal) continue;
				character.EnqueueNext(entity, Command.Targeting(other, skill));
				found = true;
				break;
			}
			
			if (!found) {
				character.EnqueueNext(entity, Command.Pause());
			}
		}


	}

}
