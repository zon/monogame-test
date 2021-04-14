using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;
using LiteNetLib;

namespace MonoGameTest.Client {

	public class Game : Microsoft.Xna.Framework.Game {
		Context Context;
		Client Client;
		World World;
		PacketListener PacketListener;
		ISystem<float> Behavior;
		ISystem<float> Rendering;
		Resources Resources;
		GraphicsDeviceManager Graphics;
		SpriteBatch Batch;

		public Game() {
			Graphics = new GraphicsDeviceManager(this);
		}

		protected override void LoadContent() {
			Content.RootDirectory = "Content";
			Resources = Resources.Load(Content);

			IsMouseVisible = true;
		}

		protected override void Initialize() {
			base.Initialize();

			Client = new Client();
			Client.DisconnectedEvent += OnDisconnected;
			Client.Processor.SubscribeReusable<SessionPacket>(OnSession);
			
			World = new World();

			Context = new Context(GraphicsDevice, Resources, World, Client);

			PacketListener = new PacketListener(Context);

			Behavior = new SequentialSystem<float>(
				new LocalPlayerSystem(Context)
			);
			
			Rendering = new SequentialSystem<float>(
				new TilemapDrawSystem(Context),
				new MovementAnimationSystem(Context),
				new SpriteDrawSystem(Context),
				new AttackAnimationSystem(Context)
			);
			
			Client.Connect();
		}

		protected override void Update(GameTime gameTime) {
			var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
			if (Context.IsReady) Behavior.Update(dt);
			Client.Poll();
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			GraphicsDevice.Clear(Color.Black);
			if (!Context.IsReady) return;
			var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
			Rendering.Update(dt);
		}
		
		void OnDisconnected(DisconnectInfo disconnectInfo) {
			ClearLevel();
		}

		void OnSession(SessionPacket session) {
			Context.Load(Content, Window, session);
			Context.Camera.SetWindowSize(Graphics);
		}

		void ClearLevel() {
			Context.Unload();
		}

	}
	
}
