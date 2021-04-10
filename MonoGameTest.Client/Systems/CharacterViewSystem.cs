using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class CharacterViewSystem : AEntitySetSystem<float> {
		readonly Context Context;

		public CharacterViewSystem(Context context) : base(context.World
			.GetEntities()
			.With<Position>()
			.With<Sprite>()
			.With<CharacterView>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var position = ref entity.Get<Position>();
			ref var sprite = ref entity.Get<Sprite>();
			ref var view = ref entity.Get<CharacterView>();
			switch (view.State) {

				case CharacterViewState.Spawned:
					view.State = CharacterViewState.Idle;
					view.Previous = position.Coord;
					sprite.Position = Context.CoordToVector(position.Coord);
					break;

				case CharacterViewState.Idle:
					if (position.Coord != view.Previous) {
						view.State = CharacterViewState.Moving;
						view.MoveAmount = 0;
						if (position.Coord.X < view.Previous.X) {
							sprite.Effects = SpriteEffects.FlipHorizontally;
						} else if (position.Coord.X > view.Previous.X) {
							sprite.Effects = SpriteEffects.None;
						}
					}
					break;
				
				case CharacterViewState.Moving:
					view.MoveAmount = MathHelper.Clamp(view.MoveAmount + dt / view.MoveDuration, 0, 1);
					sprite.Position = Vector2.Lerp(
						Context.CoordToVector(view.Previous),
						Context.CoordToVector(position.Coord),
						view.MoveAmount
					);
					if (view.MoveAmount >= 1) {
						view.State = CharacterViewState.Idle;
						view.Previous = position.Coord;
						Context.Resources.Move.Play();
					}
					break;
			}
		}

	}

}
