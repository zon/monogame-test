using Microsoft.Xna.Framework;
using MonoGame.Aseprite.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct ProjectileView {
		public AnimatedSprite Sprite;
		public Vector2 TargetPosition;

		public ProjectileView(Context context, Projectile projectile) {
			Sprite = new AnimatedSprite(context.Resources.Attacks);
			Sprite.Origin = new Vector2(Sprite.Width, Sprite.Height) / 2;
			Sprite.Play(projectile.Attack.Projectile);
			TargetPosition = default;
		}

	}

}
