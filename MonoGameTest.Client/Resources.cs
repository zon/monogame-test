using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoGame.Aseprite.Documents;

namespace MonoGameTest.Client {

	public class Resources {
		public readonly AsepriteDocument Characters;
		public readonly AsepriteDocument Attacks;
		public readonly AsepriteDocument Hits;
		public readonly SoundEffect MoveSound;
		public readonly SoundEffect HitSound;

		Resources(ContentManager content) {
			Characters = content.Load<AsepriteDocument>("entities");
			Attacks = content.Load<AsepriteDocument>("attacks");
			Hits = content.Load<AsepriteDocument>("hits");
			MoveSound = content.Load<SoundEffect>("bump-strike-0");
			HitSound = content.Load<SoundEffect>("bump-strike-1");
		}

		public static Resources Load(ContentManager content) {
			return new Resources(content);
		}

	}

}
