using Microsoft.Xna.Framework;
using MonoGame.Aseprite.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct ProjectileView {
		public AnimatedSprite Sprite;
		public Vector2 TargetPosition;

		public ProjectileView(Context context, Projectile projectile) {
			Sprite = context.Resources.GetAnimatedSprite(projectile.Skill.ProjectileSprite);
			Sprite.Origin = new Vector2(Sprite.Width, Sprite.Height) / 2;
			Sprite.Play(projectile.Skill.ProjectileSprite.Value.Tag);
			TargetPosition = default;
		}

	}

}
