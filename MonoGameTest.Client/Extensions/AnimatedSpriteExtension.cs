using MonoGame.Aseprite.Graphics;

namespace MonoGameTest.Client {

	public static class AnimatedSpriteExtension {

		public static float GetDuration(this AnimatedSprite sprite, MonoGame.Aseprite.Graphics.Animation animation) {
			var total = 0f;
			for (var f = animation.From; f <= animation.To; f++) {
				total += sprite.Frames[f].Duration;
			}
			return total;
		}

		public static float GetCurrentDuration(this AnimatedSprite sprite) {
			return GetDuration(sprite, sprite.CurrentAnimation);
		}

	}

}
