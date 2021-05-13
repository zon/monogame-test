using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Aseprite.Documents;

namespace MonoGameTest.Client {

	public class ButtonResource {
		public readonly AsepriteDocument Document;
		public readonly Rectangle Idle;
		public readonly Rectangle Hover;
		public readonly Rectangle Pressed;
		public readonly Rectangle Down;
		public readonly Rectangle Selected;

		public Point Size => Idle.Size;

		public ButtonResource(ContentManager content) {
			Document = content.Load<AsepriteDocument>("button");
			Idle = Document.Frames[0].ToRectangle();
			Hover = Document.Frames[1].ToRectangle();
			Pressed = Document.Frames[2].ToRectangle();
			Down = Document.Frames[3].ToRectangle();
			Selected = Document.Frames[4].ToRectangle();
		}

	}

}
