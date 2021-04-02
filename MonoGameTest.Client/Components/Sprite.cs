using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Documents;

namespace MonoGameTest.Client {

	public struct Sprite {
		public Rectangle Rectangle;
		public Color Color;
		public Vector2 Position;
		public float Rotation;
		public Vector2 Origin;
		public Vector2 Scale;
		public SpriteEffects Effects;
		public float Depth;

		public static Sprite Create(AsepriteDocument document, int frame) {
			var f = document.Frames[frame];
			return new Sprite {
				Rectangle = new Rectangle(f.X, f.Y, f.Width, f.Height),
				Color = Color.White,
				Scale = Vector2.One
			};
		}

	}

}
