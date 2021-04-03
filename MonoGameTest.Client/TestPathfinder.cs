using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;

namespace MonoGameTest.Client {

	public class TestPathfinder : Game {
		GraphicsDeviceManager Graphics;
		SpriteFont Font;
		TiledMap TiledMap;
		Grid Grid;
		OrthographicCamera Camera;
		SpriteBatch Batch;
		World World;
		ISystem<float> Systems;

		public TestPathfinder() {
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			Graphics = new GraphicsDeviceManager(this);
		}

		protected override void Initialize() {
			base.Initialize();

			Graphics.PreferredBackBufferWidth = 1024;
			Graphics.PreferredBackBufferHeight = 1024;
			Graphics.ApplyChanges();

			Font = Content.Load<SpriteFont>("default");

			TiledMap = Tiled.LoadMap(Content, "first");
			Grid = Tiled.LoadGrid(TiledMap);

			var viewport = new BoxingViewportAdapter(
				Window,
				GraphicsDevice,
				TiledMap.WidthInPixels,
				TiledMap.HeightInPixels
			);
			Camera = new OrthographicCamera(viewport);

			Batch = new SpriteBatch(GraphicsDevice);

			World = new World();
			var positions = World.GetEntities().With<Character>().AsMap<Position>();

			Systems = new SequentialSystem<float>(
				new TilemapDrawSystem(GraphicsDevice, TiledMap, Camera),
				new PathfinderDebugSystem(TiledMap, Grid, positions, Batch, Font, Camera)
			);
		}

		protected override void Update(GameTime gameTime) {
			GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			GraphicsDevice.Clear(Color.Black);
			Systems.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
		}

	}
	
}
