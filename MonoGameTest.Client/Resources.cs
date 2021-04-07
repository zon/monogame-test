using Microsoft.Xna.Framework.Content;
using MonoGame.Aseprite.Documents;

namespace MonoGameTest.Client {

	public class Resources {
		public readonly AsepriteDocument Characters;

		Resources(AsepriteDocument characters) {
			Characters = characters;
		}

		public static Resources Load(ContentManager content) {
			return new Resources(content.Load<AsepriteDocument>("entities"));
		}

	}

}
