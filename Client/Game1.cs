using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Aseprite.Documents;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Primitives;
using Roy_T.AStar.Paths;

namespace MonoGameTest
{
	public class Game1 : Game
	{
		TiledMap tiledMap;
		Grid grid;
		PathFinder pathfinder;

		GridPosition start;
		GridPosition end;
		Path path;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		AsepriteDocument entityViews;
		TiledMapRenderer tiledMapRenderer;
		EntitySprite startView;
		EntitySprite endView;
		EntitySprite pathView;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			this.Window.Title = "MonoGame Test";

			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
			graphics.ApplyChanges();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			tiledMap = Content.Load<TiledMap>("first");
			tiledMap.GetLayer("zones").IsVisible = false;

			var gridSize = new GridSize(tiledMap.Width, tiledMap.Height);
			var cellSize = new Size(Distance.FromMeters(1), Distance.FromMeters(1));
			var velocity = Velocity.FromMetersPerSecond(1.43f);
			grid = Grid.CreateGridWithLateralAndDiagonalConnections(gridSize, cellSize, velocity);
			pathfinder = new PathFinder();

			var walls = tiledMap.GetLayer<TiledMapTileLayer>("walls");
			for (var y = 0; y < tiledMap.Height; y++) {
				for (var x = 0; x < tiledMap.Width; x++) {
					var mapTile = walls.GetTile(Convert.ToUInt16(x), Convert.ToUInt16(y));
					var solid = GetProperty(mapTile, "solid") == "true";
					if (solid) {
						var pos = new GridPosition(x, y);
						grid.DisconnectNode(pos);
						grid.RemoveDiagonalConnectionsIntersectingWithNode(pos);
					}
				}
			}

			tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, tiledMap);

			entityViews = Content.Load<AsepriteDocument>("entities");
			
			startView = EntitySprite.Load(entityViews, 1);
			startView.color = Color.Blue;
			
			endView = EntitySprite.Load(entityViews, 1);
			endView.color = Color.Red;

			pathView = EntitySprite.Load(entityViews, 0);
			pathView.color = Color.Blue;
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			tiledMapRenderer.Update(gameTime);

			var mouse = Mouse.GetState();
			var dirty = false;
			if (mouse.LeftButton == ButtonState.Pressed) {
				var x = ScreenToMap(mouse.X);
				var y = ScreenToMap(mouse.Y);
				start = new GridPosition(x, y);
				dirty = true;

			} else if (mouse.RightButton == ButtonState.Pressed) {
				var x = ScreenToMap(mouse.X);
				var y = ScreenToMap(mouse.Y);
				end = new GridPosition(x, y);
				dirty = true;
			}

			if (dirty && start != null && end != null) {
				try {
					path = pathfinder.FindPath(start, end, grid);
				} catch (Exception e) {
					Console.WriteLine(e);
				}
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			tiledMapRenderer.Draw();

			spriteBatch.Begin();

			if (start != null) {
				startView.x = start.X * tiledMap.TileWidth;
				startView.y = start.Y * tiledMap.TileHeight;
				startView.Render(spriteBatch);
			}

			if (end != null) {
				endView.x = end.X * tiledMap.TileWidth;
				endView.y = end.Y * tiledMap.TileHeight;
				endView.Render(spriteBatch);
			}

			if (path != null) {
				foreach (var edge in path.Edges) {
					pathView.x = edge.End.Position.X * tiledMap.TileWidth;
					pathView.y = edge.End.Position.Y * tiledMap.TileHeight;
					pathView.Render(spriteBatch);
				}
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}

		int ScreenToMap(float v) {
			return Convert.ToInt32(MathF.Floor(v / tiledMap.TileWidth));
		}

		string GetProperty(TiledMapTile mapTile, string name) {
			var tileset = tiledMap.GetTilesetByTileGlobalIdentifier(mapTile.GlobalIdentifier);
			var firstGid = tiledMap.GetTilesetFirstGlobalIdentifier(tileset);
			var id = mapTile.GlobalIdentifier - firstGid;
			if (id >= tileset.Tiles.Count) return "";
			var tilesetTile = tileset.Tiles[id];
			var value = "";
			tilesetTile.Properties.TryGetValue(name, out value);
			return value;
		}

	}
}
