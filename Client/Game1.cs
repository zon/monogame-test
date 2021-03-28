using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace MonoGameTest
{
	public class Game1 : Game
	{
		TiledMap _tiledMap;
		TiledMapRenderer _tiledMapRenderer;
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			this.Window.Title = "MonoGame Test";

			_graphics.PreferredBackBufferWidth = 800;
			_graphics.PreferredBackBufferHeight = 600;
			_graphics.ApplyChanges();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_tiledMap = Content.Load<TiledMap>("first");
			_tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);
			
			_spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			_tiledMapRenderer.Update(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			_tiledMapRenderer.Draw();

			base.Draw(gameTime);
		}
	}
}
