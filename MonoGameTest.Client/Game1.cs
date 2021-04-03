﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Aseprite.Documents;
using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGameTest.Client {

	public class Game1 : Game {
		TiledMap TiledMap;
		Grid Grid;
		AsepriteDocument CharacterSprites;
		GraphicsDeviceManager Graphics;
		SpriteBatch Batch;
		OrthographicCamera Camera;
		World World;
		ISystem<float> Behavior;
		ISystem<float> Rendering;

		public Game1() {
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			Graphics = new GraphicsDeviceManager(this);
		}

		protected override void Initialize() {
			base.Initialize();

			var d = 16 * 16 * 2;
			Graphics.PreferredBackBufferWidth = d;
			Graphics.PreferredBackBufferHeight = d;
			Graphics.ApplyChanges();

			TiledMap = Content.Load<TiledMap>("first");
			TiledMap.GetLayer("zones").IsVisible = false;

			var nodes = new Node[TiledMap.Width * TiledMap.Height];
			var walls = TiledMap.GetLayer<TiledMapTileLayer>("walls");
			for (var y = 0; y < TiledMap.Height; y++) {
				for (var x = 0; x < TiledMap.Width; x++) {
					var i = TiledMap.Width * y + x;
					var mapTile = walls.GetTile(Convert.ToUInt16(x), Convert.ToUInt16(y));
					var solid = GetProperty(mapTile, "solid") == "true";
					nodes[i] = new Node(x, y, solid);
				}
			}
			Grid = new Grid(TiledMap.Width, TiledMap.Height, nodes);

			CharacterSprites = Content.Load<AsepriteDocument>("entities");

			Batch = new SpriteBatch(GraphicsDevice);

			var viewport = new BoxingViewportAdapter(
				Window,
				GraphicsDevice,
				TiledMap.WidthInPixels,
				TiledMap.HeightInPixels
			);
			Camera = new OrthographicCamera(viewport);

			World = new World();
			var characters = World.GetEntities().AsMap<Character>();
			var players = World.GetEntities().AsMap<Player>();
			var positions = World.GetEntities().With<Character>().AsMap<Position>();

			Behavior = new SequentialSystem<float>(
				new CooldownSystem(World),
				new MovementSystem(World, Grid, positions),
				new MovementInputSystem(World, Grid, TiledMap, positions, Camera)
			);
			Rendering = new SequentialSystem<float>(
				new CharacterViewSystem(World, TiledMap),
				new TilemapDrawSystem(GraphicsDevice, TiledMap, Camera),
				new SpriteDrawSystem(Batch, Camera, CharacterSprites.Texture, World),
				new MovementDebugSystem(Batch, TiledMap, Camera, World)
			);

			ClientEntity.CreatePlayer(World, new Coord(7, 7), Sprite.Create(CharacterSprites, 1));
		}

		protected override void Update(GameTime gameTime) {
			Behavior.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);
			Rendering.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
		}

		string GetProperty(TiledMapTile mapTile, string name) {
			var tileset = TiledMap.GetTilesetByTileGlobalIdentifier(mapTile.GlobalIdentifier);
			var firstGid = TiledMap.GetTilesetFirstGlobalIdentifier(tileset);
			var id = mapTile.GlobalIdentifier - firstGid;
			if (id >= tileset.Tiles.Count) return "";
			var tilesetTile = tileset.Tiles[id];
			var value = "";
			tilesetTile.Properties.TryGetValue(name, out value);
			return value;
		}

	}
	
}
