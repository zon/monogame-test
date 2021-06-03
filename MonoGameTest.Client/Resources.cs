using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class Resources {
		public readonly AsepriteDocument Characters;
		public readonly AsepriteDocument Effects;
		public readonly AsepriteDocument EffectsLarge;
		public readonly AsepriteDocument Hits;
		public readonly ButtonResource Button;
		public readonly AsepriteDocument SkillIcons;
		public readonly AsepriteDocument Highlight;
		public readonly SoundEffect MoveSound;
		public readonly SoundEffect HitSound;
		public readonly SoundEffect MoveConfirmSound;
		public readonly BitmapFont Font;

		Resources(ContentManager content) {
			Characters = content.Load<AsepriteDocument>("entities");
			Effects = content.Load<AsepriteDocument>("attacks");
			EffectsLarge = content.Load<AsepriteDocument>("effects-3x3");
			Hits = content.Load<AsepriteDocument>("hits");
			Button = new ButtonResource(content);
			SkillIcons = content.Load<AsepriteDocument>("skills");
			Highlight = content.Load<AsepriteDocument>("highlight");
			MoveSound = content.Load<SoundEffect>("bump-strike-0");
			HitSound = content.Load<SoundEffect>("bump-strike-1");
			MoveConfirmSound = content.Load<SoundEffect>("bump-strike-8");
			Font = content.Load<BitmapFont>("munro");
		}

		public AsepriteDocument GetSprite(SpriteFile? file) {
			switch (file) {
				case SpriteFile.Effects:
					return Effects;
				case SpriteFile.EffectsLarge:
					return EffectsLarge;
			}
			return null;
		}

		public AnimatedSprite GetAnimatedSprite(SpriteFile? file) {
			var doc = GetSprite(file);
			if (doc == null) return null;
			return new AnimatedSprite(doc);
		}

		public static Resources Load(ContentManager content) {
			return new Resources(content);
		}

	}

}
