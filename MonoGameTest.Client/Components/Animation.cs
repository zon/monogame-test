using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;

namespace MonoGameTest.Client {

	public struct Animation {
		public AnimatedSprite Sprite;
		public Color Color;
		public Vector2 Position;
		public float Rotation;
		public Vector2 Origin;
		public Vector2 Scale;
		public SpriteEffects Effects;
		public float Depth;
		public bool Enabled;

		public static Animation Create(AsepriteDocument document) {
			var sprite = new AnimatedSprite(document);
			return new Animation {
				Sprite = sprite,
				Color = Color.White,
				Scale = Vector2.One,
				Enabled = true
			};
		}

	}

}
