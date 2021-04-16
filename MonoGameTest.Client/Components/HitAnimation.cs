using Microsoft.Xna.Framework;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;

namespace MonoGameTest.Client {

	public struct HitAnimation {

		public AnimatedSprite Sprite;

		public bool IsActive => Sprite.Animating;

		public HitAnimation(AsepriteDocument document) {
			Sprite = new AnimatedSprite(document);
			Sprite.Origin = new Vector2(Sprite.Width, Sprite.Height) / 2;
			Sprite.Stop();
			Sprite.OnAnimationLoop = OnEnd;
		}

		public void Start() {
			Sprite.Play("hit-0");
		}

		void OnEnd() {
			Sprite.Stop();
		}

	}

}
