using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoGame.Aseprite.Documents;
using MonoGame.Extended.BitmapFonts;

namespace MonoGameTest.Client {

	public class Resources {
		public readonly AsepriteDocument Characters;
		public readonly AsepriteDocument Attacks;
		public readonly AsepriteDocument Hits;
		public readonly SoundEffect MoveSound;
		public readonly SoundEffect HitSound;
		public readonly BitmapFont Font;

		Resources(ContentManager content) {
			Characters = content.Load<AsepriteDocument>("entities");
			Attacks = content.Load<AsepriteDocument>("attacks");
			Hits = content.Load<AsepriteDocument>("hits");
			MoveSound = content.Load<SoundEffect>("bump-strike-0");
			HitSound = content.Load<SoundEffect>("bump-strike-1");
			Font = content.Load<BitmapFont>("munro");
		}

		public static Resources Load(ContentManager content) {
			return new Resources(content);
		}

	}

}
