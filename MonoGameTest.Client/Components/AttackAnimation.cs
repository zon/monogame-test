using DefaultEcs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct AttackAnimation {
		public AnimatedSprite Sprite;

		public AttackAnimation(AsepriteDocument document) {
			Sprite = new AnimatedSprite(document);
			Sprite.Origin = new Vector2(Sprite.Width, Sprite.Height) / 2;
			Sprite.OnAnimationLoop = OnEnd;
			Sprite.Stop();
		}
		
		public void Start(
			Context context,
			in Entity entity,
			Coord target,
			string animationName
		) {
			ref var position = ref entity.Get<Position>();
			ref var characterSprite = ref entity.Get<Sprite>();
			Sprite.Position = context.CoordToVector(position.Coord);
			Sprite.LayerDepth = Depths.Attack;
			var facing = Coord.Facing(position.Coord, target);
			var width = new Vector2(characterSprite.Rectangle.Width, 0);
			var height = new Vector2(0, characterSprite.Rectangle.Height);
			var halfWidth = new Vector2(characterSprite.Rectangle.Width / 2, 0);
			var halfHeight = new Vector2(0, characterSprite.Rectangle.Height / 2);
			if (facing.X == 0) {
				if (facing.Y < 0) {
					Sprite.Position = characterSprite.Position + halfWidth;
					Sprite.SpriteEffect = SpriteEffects.FlipVertically;
				} else {
					Sprite.Position = characterSprite.Position + height + halfWidth;
					Sprite.SpriteEffect = SpriteEffects.None;
				}
				Sprite.Play($"{animationName}-down");
			} else {
				if (facing.X < 0) {
					Sprite.Position = characterSprite.Position + halfHeight;
					Sprite.SpriteEffect = SpriteEffects.FlipHorizontally;
				} else {
					Sprite.Position = characterSprite.Position + width + halfHeight;
					Sprite.SpriteEffect = SpriteEffects.None;
				}
				Sprite.Play($"{animationName}-right");
			}
			characterSprite.Effects = Sprite.SpriteEffect;
			context.Resources.HitSound.Play();
		}

		void OnEnd() {
			Sprite.Stop();
		}

	}

}
