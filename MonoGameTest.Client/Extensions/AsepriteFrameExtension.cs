using Microsoft.Xna.Framework;
using MonoGame.Aseprite.Documents;

namespace MonoGameTest.Client {

	public static class AsepriteFrameExtension {

		public static Rectangle ToRectangle(this AsepriteFrame frame) {
			return new Rectangle(frame.X, frame.Y, frame.Width, frame.Height);
		}

	}

}
