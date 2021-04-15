using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest.Client {

	public class TestDepth : Microsoft.Xna.Framework.Game {
		
		GraphicsDeviceManager Graphics;
		Resources Resources;
		SpriteBatch Batch;

		Sprite A;
		Sprite B;

		public TestDepth() {
			Graphics = new GraphicsDeviceManager(this);
		}

		protected override void LoadContent() {
			Content.RootDirectory = "Content";
			Resources = Resources.Load(Content);
		}

		protected override void Initialize() {
			base.Initialize();

			Batch = new SpriteBatch(GraphicsDevice);

			Graphics.PreferredBackBufferWidth = 1024;
			Graphics.PreferredBackBufferHeight = 1024;
			Graphics.ApplyChanges();

			IsMouseVisible = true;

			A = Sprite.Create(Resources.Characters, 3, new Vector2(8, 8));
			B = Sprite.Create(Resources.Characters, 4, new Vector2(16, 16));
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);
			Batch.Begin(sortMode: SpriteSortMode.FrontToBack);

			var scale = 8;

			Batch.Draw(
				texture: A.Document.Texture,
				position: A.Position * scale,
				sourceRectangle: A.Rectangle,
				color: A.Color,
				rotation: A.Rotation,
				origin: A.Origin,
				scale: scale,
				effects: A.Effects,
				layerDepth: 0f
			);

			Batch.Draw(
				texture: B.Document.Texture,
				position: B.Position * scale,
				sourceRectangle: B.Rectangle,
				color: B.Color,
				rotation: B.Rotation,
				origin: B.Origin,
				scale: scale,
				effects: B.Effects,
				layerDepth: 0.1f
			);

			Batch.End();
			
		}

	}

}
