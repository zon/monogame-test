using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace MonoGameTest.Client {

	public class TestRenderTarget : Microsoft.Xna.Framework.Game {
		GraphicsDeviceManager Graphics;
		TiledMap TiledMap;
		TiledMapRenderer TiledMapRenderer;
		SpriteBatch Batch;
		RenderTarget2D RenderTarget;

		public TestRenderTarget() {
			Graphics = new GraphicsDeviceManager(this);
		}

		protected override void LoadContent() {
			Content.RootDirectory = "Content";
			TiledMap = Tiled.LoadMap(Content, "entry");
		}

		protected override void Initialize() {
			base.Initialize();
			
			TiledMapRenderer = new TiledMapRenderer(GraphicsDevice, TiledMap);

			Batch = new SpriteBatch(GraphicsDevice);

			RenderTarget = new RenderTarget2D(
				GraphicsDevice,
				TiledMap.WidthInPixels,
				TiledMap.HeightInPixels,
				false,
				SurfaceFormat.Color,
				DepthFormat.Depth24
			);

			Graphics.PreferredBackBufferWidth = TiledMap.WidthInPixels * View.SCALE;
			Graphics.PreferredBackBufferHeight = TiledMap.WidthInPixels * View.SCALE;
			Graphics.ApplyChanges();

			IsMouseVisible = true;
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.SetRenderTarget(RenderTarget);
			GraphicsDevice.Clear(Color.Black);
			TiledMapRenderer.Draw();
			GraphicsDevice.SetRenderTarget(null);

			GraphicsDevice.Clear(Color.Black);
			Batch.Begin(samplerState: SamplerState.PointClamp);
			Batch.Draw(
				texture: RenderTarget,
				position: Vector2.Zero,
				sourceRectangle: null,
				color: Color.White,
				rotation: 0,
				origin: Vector2.Zero,
				scale: Vector2.One * View.SCALE,
				effects: SpriteEffects.None,
				layerDepth: 0
			);
			Batch.End();
		}

	}

}
