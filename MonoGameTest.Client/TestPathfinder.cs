using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class TestPathfinder : Game {
		GraphicsDeviceManager Graphics;
		Resources Resources;
		SpriteFont Font;
		Context Context;
		SpriteBatch Batch;
		World World;
		ISystem<float> Systems;

		public TestPathfinder() {
			IsMouseVisible = true;

			Graphics = new GraphicsDeviceManager(this);
		}

		protected override void Initialize() {
			base.Initialize();

			Graphics.PreferredBackBufferWidth = 1024;
			Graphics.PreferredBackBufferHeight = 1024;
			Graphics.ApplyChanges();
			
			Content.RootDirectory = "Content";
			Resources = Resources.Load(Content);

			Font = Content.Load<SpriteFont>("default");

			Batch = new SpriteBatch(GraphicsDevice);

			World = new World();

			Context = new Context(GraphicsDevice, Resources, World, null);
			Context.Load(Content, Window, "first");

			Systems = new SequentialSystem<float>(
				new TilemapDrawSystem(Context),
				new PathfinderDebugSystem(Batch, Font, Context)
			);
		}

		protected override void Update(GameTime gameTime) {
			GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			GraphicsDevice.Clear(Color.Black);
			Systems.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
		}

	}
	
}
