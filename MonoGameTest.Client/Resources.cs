using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoGame.Aseprite.Documents;

namespace MonoGameTest.Client {

	public class Resources {
		public readonly AsepriteDocument Characters;
		public readonly SoundEffect Move;

		Resources(ContentManager content) {
			Characters = content.Load<AsepriteDocument>("entities");
			Move = content.Load<SoundEffect>("bump-strike-0");
		}

		public static Resources Load(ContentManager content) {
			return new Resources(content);
		}

	}

}
