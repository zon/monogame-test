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

			Batch = new SpriteBatch(GraphicsDevice);

			IsMouseVisible = true;
		}

		protected override void Initialize() {
			base.Initialize();

			Client = new Client();
			Client.PeerDisconnectedEvent += OnDisconnected;
			Client.TilemapEvent += OnTilemap;
			
			World = new World();

			Context = new Context(GraphicsDevice, Resources, World, Client);

			Behavior = new SequentialSystem<float>(
				new CooldownSystem(World),
				new MovementSystem(World, Context),
				new MovementInputSystem(World, Context),
				new ClientNetworkSystem(Context)
			);
			
			Rendering = new SequentialSystem<float>(
				new CharacterViewSystem(World, Context),
				new TilemapDrawSystem(Context),
				new SpriteDrawSystem(World, Batch, Context),
				new MovementDebugSystem(World, Batch, Context)
			);
			
			Client.Connect();
		}

		protected override void Update(GameTime gameTime) {
			if (Behavior != null) Behavior
				.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
			Client.Poll();
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);
			if (Rendering == null) return;
			Rendering.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
		}
		
		void OnDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
			ClearLevel();
		}

		void OnTilemap(TilemapPacket packet) {
			LoadLevel(packet.Name);
		}

		void LoadLevel(string name) {
			Context.Load(this, name);
			Context.Camera.SetWindowSize(Graphics);
		}

		void ClearLevel() {
			Context.Unload();
		}

	}
	
}
