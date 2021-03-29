using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Documents;

namespace MonoGameTest {
	
	public class EntitySprite {
		public readonly Texture2D texture;
		public readonly Rectangle sourceRectangle;

		public Color color = Color.White;
		public Vector2 position = Vector2.Zero;
		public float rotation = 0;
		public Vector2 origin = Vector2.Zero;
		public Vector2 scale = Vector2.One;
		public SpriteEffects effects = SpriteEffects.None;
		public float depth = 0;

		public float x {
            get { return position.X; }
            set { position.X = value; }
        }

        public float y {
            get { return position.Y; }
            set { position.Y = value; }
        }

		public EntitySprite(Texture2D texture, AsepriteFrame frame) {
			this.texture = texture;
			sourceRectangle = new Rectangle(frame.X, frame.Y, frame.Width, frame.Height);
		}

		public void Render(SpriteBatch spriteBatch) {
			spriteBatch.Draw(
				texture: texture,
				position: position,
				sourceRectangle: sourceRectangle,
				color: color,
				rotation: rotation,
				origin: origin,
				scale: scale,
				effects: effects,
				layerDepth: depth
			);
		}

		public static EntitySprite Load(AsepriteDocument document, int index) {
			return new EntitySprite(document.Texture, document.Frames[index]);
		}

	}

}
