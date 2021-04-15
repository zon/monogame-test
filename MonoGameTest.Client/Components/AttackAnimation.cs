using DefaultEcs;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct AttackAnimation {
		public AnimatedSprite Sprite;
		public Coord Facing;
		public float Amount;
		public float Duration;

		public bool IsActive => Sprite.Animating;

		public AttackAnimation(AsepriteDocument document) {
			Sprite = new AnimatedSprite(document);
			Facing = Coord.Zero;
			Amount = 0;
			Duration = 0;
			Sprite.Origin = new Vector2(Sprite.Width, Sprite.Height) / 2;
			Sprite.Stop();
			Sprite.OnAnimationLoop = OnEnd;
		}
		
		public void Start(
			Context context,
			in Entity entity,
			Coord target,
			string animationName
		) {
			ref var position = ref entity.Get<Position>();
			Facing = Coord.Facing(position.Coord, target);
			if (Facing.X == 0) {
				Sprite.Play($"{animationName}-down");
			} else {
				Sprite.Play($"{animationName}-right");
			}
			Amount = 0;
			Duration = Sprite.GetCurrentDuration();
			context.Resources.HitSound.Play();
		}

		public void Update(float dt) {
			Amount = MathHelper.Clamp(Amount + dt / Duration, 0, 1);
			Sprite.Update(dt);
		}

		void OnEnd() {
			Sprite.Stop();
		}

	}

}
