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
		ButtonListener ButtonListener;
		ISystem<float> Behavior;
		ISystem<float> BackgroundRendering;
		ISystem<float> ForegroundRendering;
		ISystem<float> UIRendering;
		Resources Resources;
		GraphicsDeviceManager Graphics;
		CameraView WorldCamera;
		SpriteBatch WorldBatch;
		CameraView UICamera;
		SpriteBatch UIBatch;
		SpriteBatch Result;

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

			GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };
			
			Graphics.PreferredBackBufferWidth = View.SCREEN_WIDTH;
			Graphics.PreferredBackBufferHeight = View.SCREEN_HEIGHT;
			Graphics.ApplyChanges();

			WorldCamera = new CameraView(Window, GraphicsDevice);
			WorldBatch = new SpriteBatch(GraphicsDevice);
			UICamera = new CameraView(Window, GraphicsDevice);
			UIBatch = new SpriteBatch(GraphicsDevice);
			Result = new SpriteBatch(GraphicsDevice);

			Context = new Context(
				GraphicsDevice,
				Resources,
				World,
				Client,
				WorldCamera,
				WorldBatch,
				UICamera,
				UIBatch
			);

			PacketListener = new PacketListener(Context);
			ButtonListener = new ButtonListener(Context);

			Behavior = new SequentialSystem<float>(
				new LocalPlayerCameraSystem(Context),
				new CameraSystem(Context),
				new EnergySystem(Context),
				new CharacterSystem(Context),
				new LocalInputSystem(Context),
				new SkillInputSystem(Context)
			);

			BackgroundRendering = new LdtkDrawSystem(Context);
			ForegroundRendering = new SequentialSystem<float>(
				new SkillTargetingSystem(Context),
				new MovementAnimationSystem(Context),
				new HealthBarSystem(Context),
				new BangSystem(Context),
				new SkillAnimationSystem(Context),
				new HitAnimationSystem(Context),
				new ProjectileAnimationSystem(Context),
				new TargetDrawSystem(Context),
				new SpriteDrawSystem(Context),
				new EffectSystem(Context)
			);
			UIRendering = new SequentialSystem<float>(
				new EnergyBarSystem(Context),
				new ButtonSystem(Context)
			);

			Client.Connect();
		}

		protected override void Update(GameTime gameTime) {
			var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
			if (Context.IsReady) {
				Context.Update();
				Behavior.Update(dt);
			}
			Context.Recorder.Execute();
			Client.Poll();
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);
			if (!Context.IsReady) return;

			var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

			GraphicsDevice.SetRenderTarget(WorldCamera.RenderTarget);
			GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			GraphicsDevice.Clear(Color.Black);

			BackgroundRendering.Update(dt);

			var matrix = Context.WorldCameraView.GetMatrix();
			WorldBatch.Begin(
				transformMatrix: Context.WorldCameraView.GetMatrix(),
				samplerState: SamplerState.PointClamp,
				sortMode: SpriteSortMode.FrontToBack
			);
			ForegroundRendering.Update(dt);
			WorldBatch.End();

			GraphicsDevice.SetRenderTarget(UICamera.RenderTarget);
			GraphicsDevice.Clear(new Color(0, 0, 0, 0));

			matrix = Context.UICameraView.GetMatrix();
			UIBatch.Begin(
				transformMatrix: matrix,
				samplerState: SamplerState.PointClamp,
				sortMode: SpriteSortMode.FrontToBack
			);
			UIRendering.Update(dt);
			UIBatch.End();

			GraphicsDevice.SetRenderTarget(null);

			Result.Begin(samplerState: SamplerState.PointClamp);
			Result.Draw(
				texture: WorldCamera.RenderTarget,
				position: Vector2.Zero,
				sourceRectangle: null,
				color: Color.White,
				rotation: 0,
				origin: Vector2.Zero,
				scale: Vector2.One,
				effects: SpriteEffects.None,
				layerDepth: 0
			);
			Result.Draw(
				texture: UICamera.RenderTarget,
				position: Vector2.Zero,
				sourceRectangle: null,
				color: Color.White,
				rotation: 0,
				origin: Vector2.Zero,
				scale: Vector2.One,
				effects: SpriteEffects.None,
				layerDepth: 0
			);
			Result.End();
		}
		
		void OnDisconnected(DisconnectInfo disconnectInfo) {
			ClearLevel();
		}

		void OnSession(SessionPacket session) {
			Console.WriteLine("Session: {0}", session.Id);
			Context.Load(Content, session);
		}

		void ClearLevel() {
			Context.Unload();
		}

	}
	
}
