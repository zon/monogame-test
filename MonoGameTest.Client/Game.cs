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
		ISystem<float> BackgroundRendering;
		ISystem<float> ForegroundRendering;
		Resources Resources;
		GraphicsDeviceManager Graphics;
		RenderTarget2D RenderTarget;
		SpriteBatch Foreground;
		Camera Camera;
		SpriteBatch Target;

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
				width: View.WIDTH,
				height: View.HEIGHT,
				mipMap: false,
				preferredFormat: GraphicsDevice.PresentationParameters.BackBufferFormat,
				preferredDepthFormat: DepthFormat.Depth24
			);
			GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true }; 

			Foreground = new SpriteBatch(GraphicsDevice);
			Camera = new Camera(Window, GraphicsDevice);
			Target = new SpriteBatch(GraphicsDevice);

			Context = new Context(GraphicsDevice, Resources, World, Client, Foreground, Camera);

			PacketListener = new PacketListener(Context);

			Behavior = new SequentialSystem<float>(
				new LocalInputSystem(Context)
			);

			BackgroundRendering = new TilemapDrawSystem(Context);
			ForegroundRendering = new SequentialSystem<float>(
				new MovementAnimationSystem(Context),
				new HealthBarSystem(Context),
				new BangSystem(Context),
				new AttackAnimationSystem(Context),
				new HitAnimationSystem(Context),
				new TargetDrawSystem(Context),
				new SpriteDrawSystem(Context),
				new EffectSystem(Context)
			);
			
			Context.Camera.SetWindowSize(Graphics);

			Client.Connect();
		}

		protected override void Update(GameTime gameTime) {
			var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
			if (Context.IsReady) Behavior.Update(dt);
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
			GraphicsDevice.SetRenderTarget(null);

			Target.Begin(samplerState: SamplerState.PointClamp);
			Target.Draw(
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
			Target.End();
		}
		
		void OnDisconnected(DisconnectInfo disconnectInfo) {
			ClearLevel();
		}

		void OnSession(SessionPacket session) {
			Context.Load(Content, Window, session);
		}

		void ClearLevel() {
			Context.Unload();
		}

	}
	
}
