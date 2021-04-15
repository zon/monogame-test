using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Documents;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct Sprite {
		public AsepriteDocument Document;
		public Rectangle Rectangle;
		public Color Color;
		public Vector2 Position;
		public float Rotation;
		public Vector2 Origin;
		public Vector2 Scale;
		public SpriteEffects Effects;

		public void LookForward(Coord direction) {
			if (direction.X < 0) {
				Effects = SpriteEffects.FlipHorizontally;
			} else {
				Effects = SpriteEffects.None;
			}
		}

		public static Sprite Create(AsepriteDocument document, int frame, Vector2 position) {
			var f = document.Frames[frame];
			return new Sprite {
				Document = document,
				Rectangle = new Rectangle(f.X, f.Y, f.Width, f.Height),
				Color = Color.White,
				Position = position,
				Scale = Vector2.One
			};
		}

	}

}
