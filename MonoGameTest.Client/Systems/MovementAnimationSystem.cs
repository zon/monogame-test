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
			if (movement.Amount >= 1 || movement.Duration <= 0) return;

			ref var position = ref entity.Get<Position>();
			ref var sprite = ref entity.Get<Sprite>();

			movement.Amount = MathHelper.Clamp(movement.Amount + dt / movement.Duration, 0, 1);
			sprite.Position = Vector2.Lerp(
				Context.CoordToVector(movement.Previous),
				Context.CoordToVector(position.Coord),
				movement.Amount
			);
			sprite.LookForward(movement.Facing);

			if (movement.Amount >= 1) {
				Context.Resources.MoveSound.Play();
			}
		}

	}

}
