using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ldtk;
using MonoGameTest.Common;
using Microsoft.Xna.Framework.Input;

namespace MonoGameTest.Client {

	public class TestLevelRender : Microsoft.Xna.Framework.Game {
		Resources Resources;
		GraphicsDeviceManager Graphics;
		SpriteBatch Batch;
		Camera Camera;

		public TestLevelRender() {
			Graphics = new GraphicsDeviceManager(this);
		}

		protected override void LoadContent() {
			Content.RootDirectory = "Content";
			Resources = Resources.Load(Content);
		}

		protected override void Initialize() {
			base.Initialize();

			Graphics.PreferredBackBufferWidth = View.SCREEN_WIDTH;
			Graphics.PreferredBackBufferHeight = View.SCREEN_HEIGHT;
			Graphics.ApplyChanges();

			Batch = new SpriteBatch(GraphicsDevice);

			Camera = new Camera(Window, GraphicsDevice, View.WIDTH, View.HEIGHT);

			IsMouseVisible = true;
		}

		Vector2 GetMovementDirection() {
			var movementDirection = Vector2.Zero;
			var state = Keyboard.GetState();
			if (state.IsKeyDown(Keys.Down)) {
				movementDirection += Vector2.UnitY;
			}
			if (state.IsKeyDown(Keys.Up)) {
				movementDirection -= Vector2.UnitY;
			}
			if (state.IsKeyDown(Keys.Left)) {
				movementDirection -= Vector2.UnitX;
			}
			if (state.IsKeyDown(Keys.Right)) {
				movementDirection += Vector2.UnitX;
			}
			return movementDirection;
		}

		protected override void Update(GameTime gameTime) {
			const float movementSpeed = 16;
			var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
			Camera.Move(GetMovementDirection() * movementSpeed * dt);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.SetRenderTarget(Camera.RenderTarget);
			GraphicsDevice.Clear(Color.Black);

			var matrix = Camera.GetMatrix();
			Batch.Begin(transformMatrix: matrix, samplerState: SamplerState.PointClamp);
			foreach (var level in Resources.World.Json.Levels) {
				var offset = new Point((int) level.WorldX, (int) level.WorldY);
				var layer = Resources.World.GetLayer(level.Uid, "Collisions");
				var tileset = Resources.World.GetTileset(layer.TilesetDefUid.Value);
				var tileSize = new Point((int) tileset.TileGridSize, (int) tileset.TileGridSize);
				var texture = Resources.GetTilesetSprite(tileset).Texture;
				foreach (var tile in layer.AutoLayerTiles) {
					Batch.Draw(
						texture: texture,
						position: (View.ToPoint(tile.Px) + offset).ToVector2(),
						sourceRectangle: new Rectangle(View.ToPoint(tile.Src), tileSize),
						color: Color.White,
						rotation: 0,
						origin: Vector2.Zero,
						scale: Vector2.One,
						effects: SpriteEffects.None,
						layerDepth: 0
					);
				}
			}
			Batch.End();

			GraphicsDevice.SetRenderTarget(null);

			GraphicsDevice.Clear(Color.Black);
			Batch.Begin(samplerState: SamplerState.PointClamp);
			Batch.Draw(
				texture: Camera.RenderTarget,
				position: Vector2.Zero,
				sourceRectangle: null,
				color: Color.White,
				rotation: 0,
				origin: Vector2.Zero,
				scale: Vector2.One,
				effects: SpriteEffects.None,
				layerDepth: 0
			);
			Batch.End();
		}

	}

}
