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
		RenderTarget2D RenderTarget;
		Camera Camera;
		SpriteBatch Foreground;
		SpriteBatch UI;
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

			RenderTarget = new RenderTarget2D(
				graphicsDevice: GraphicsDevice,
				width: View.SCREEN_WIDTH,
				height: View.SCREEN_HEIGHT,
				mipMap: false,
				preferredFormat: GraphicsDevice.PresentationParameters.BackBufferFormat,
				preferredDepthFormat: DepthFormat.Depth24
			);
			GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true }; 

			Camera = new Camera(Window, GraphicsDevice);
			Foreground = new SpriteBatch(GraphicsDevice);
			UI = new SpriteBatch(GraphicsDevice);
			Result = new SpriteBatch(GraphicsDevice);

			Context = new Context(GraphicsDevice, Resources, World, Client, Camera, Foreground, UI);

			PacketListener = new PacketListener(Context);
			ButtonListener = new ButtonListener(Context);

			Behavior = new SequentialSystem<float>(
				new LocalInputSystem(Context),
				new SkillInputSystem(Context)
			);

			BackgroundRendering = new TilemapDrawSystem(Context);
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
				new ButtonSystem(Context)
			);
			
			Context.Camera.SetWindowSize(Graphics);

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

			GraphicsDevice.SetRenderTarget(RenderTarget);
			GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			GraphicsDevice.Clear(Color.Black);

			BackgroundRendering.Update(dt);

			Foreground.Begin(
				transformMatrix: Context.Camera.GetMatrix(),
				samplerState: SamplerState.PointClamp,
				sortMode: SpriteSortMode.FrontToBack
			);
			ForegroundRendering.Update(dt);
			Foreground.End();

			UI.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);
			UIRendering.Update(dt);
			UI.End();

			GraphicsDevice.SetRenderTarget(null);

			Result.Begin(samplerState: SamplerState.PointClamp);
			Result.Draw(
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
			Result.End();
		}
		
		void OnDisconnected(DisconnectInfo disconnectInfo) {
			ClearLevel();
		}

		void OnSession(SessionPacket session) {
			Context.Load(Content, session);
		}

		void ClearLevel() {
			Context.Unload();
		}

	}
	
}
