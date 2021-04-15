using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class MovementAnimationSystem : AEntitySetSystem<float> {
		readonly Context Context;

		public MovementAnimationSystem(Context context) : base(context.World
			.GetEntities()
			.With<MovementAnimation>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var movement = ref entity.Get<MovementAnimation>();
			ref var position = ref entity.Get<Position>();
			ref var sprite = ref entity.Get<Sprite>();

			if (movement.Progress >= 1 || movement.Duration <= 0) {
				sprite.Position = Context.CoordToVector(position.Coord);
				return;
			}

			movement.Progress = MathHelper.Clamp(movement.Progress + dt / movement.Duration, 0, 1);
			sprite.Position = Vector2.Lerp(
				Context.CoordToVector(movement.Previous),
				Context.CoordToVector(position.Coord),
				movement.Progress
			);
			sprite.LookForward(movement.Facing);

			if (movement.Progress >= 1) {
				Context.Resources.MoveSound.Play();
			}
		}

	}

}
