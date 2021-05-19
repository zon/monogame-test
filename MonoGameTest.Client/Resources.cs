using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoGame.Aseprite.Documents;
using MonoGame.Extended.BitmapFonts;

namespace MonoGameTest.Client {

	public class Resources {
		public readonly AsepriteDocument Characters;
		public readonly AsepriteDocument Attacks;
		public readonly AsepriteDocument Hits;
		public readonly ButtonResource Button;
		public readonly AsepriteDocument Skills;
		public readonly AsepriteDocument Highlight;
		public readonly SoundEffect MoveSound;
		public readonly SoundEffect HitSound;
		public readonly SoundEffect MoveConfirmSound;
		public readonly BitmapFont Font;

		Resources(ContentManager content) {
			Characters = content.Load<AsepriteDocument>("entities");
			Attacks = content.Load<AsepriteDocument>("attacks");
			Hits = content.Load<AsepriteDocument>("hits");
			Button = new ButtonResource(content);
			Skills = content.Load<AsepriteDocument>("skills");
			Highlight = content.Load<AsepriteDocument>("highlight");
			MoveSound = content.Load<SoundEffect>("bump-strike-0");
			HitSound = content.Load<SoundEffect>("bump-strike-1");
			MoveConfirmSound = content.Load<SoundEffect>("bump-strike-8");
			Font = content.Load<BitmapFont>("munro");
		}

		public static Resources Load(ContentManager content) {
			return new Resources(content);
		}

	}

}
