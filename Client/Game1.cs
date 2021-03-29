using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using RogueSharp;
using MonoGame.Aseprite.Documents;

namespace MonoGameTest
{
	public class Game1 : Game
	{
		TiledMap tiledMap;
		IMap map;
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		TiledMapRenderer tiledMapRenderer;
		EntitySprite entity;

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

			map = new Map(tiledMap.Width, tiledMap.Height);
			var walls = tiledMap.GetLayer<TiledMapTileLayer>("walls");
			for (var y = 0; y < tiledMap.Height; y++) {
				for (var x = 0; x < tiledMap.Width; x++) {
					var cell = map.GetCell(x, y);
					var mapTile = walls.GetTile(Convert.ToUInt16(x), Convert.ToUInt16(y));
					var solid = getProperty(mapTile, "solid") == "true";
					cell.IsTransparent = !solid;
					cell.IsWalkable = !solid;
				}
			}

			tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, tiledMap);

			var aseprite = Content.Load<AsepriteDocument>("entities");
			entity = EntitySprite.Load(aseprite, 1);
			entity.x = 16;
			entity.y = 16;
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			tiledMapRenderer.Update(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			tiledMapRenderer.Draw();

			spriteBatch.Begin();
			entity.Render(spriteBatch);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		string getProperty(TiledMapTile mapTile, string name) {
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
