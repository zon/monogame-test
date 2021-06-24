using System;
using DefaultEcs;
using DefaultEcs.Command;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Tiled;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class Context : IContext {
		public readonly GraphicsDevice GraphicsDevice;
		public readonly Resources Resources;
		public readonly Client Client;
		public World World { get; private set; }
		public EntityMap<CharacterId> CharacterIds { get; private set; }
		public EntityMap<Position> Positions { get; private set; }
		public readonly EntityMap<BuffEffectId> Buffs;
		public EntityMultiMap<CharacterId> CharacterBuffs { get; private set; } 
		public EntitySet Buttons { get; private set; }
		public Entity? LocalPlayer { get; private set; }
		public EntityCommandRecorder Recorder { get; private set; }
		public int SessionId { get; private set; }
		public TiledMap TiledMap { get; private set; }
		public Vector2 TileSize { get; private set; }
		public Vector2 HalfTileSize { get; private set; }
		public Grid Grid { get; private set; }
		public readonly Camera Camera;
		public readonly SpriteBatch Foreground;
		public readonly SpriteBatch UI;
		public MouseStateExtended Mouse { get; private set; }
		public bool IsReady { get; private set; }

		IDisposable LocalPlayerAddedListener;
		IDisposable LocalPlayerRemovedListener;

		public Context(
			GraphicsDevice graphicsDevice,
			Resources resources,
			World world,
			Client client,
			Camera camera,
			SpriteBatch foreground,
			SpriteBatch ui
		) {
			GraphicsDevice = graphicsDevice;
			Resources = resources;
			World = world;
			CharacterIds = world.GetEntities().With<Character>().AsMap<CharacterId>();
			Positions = world.GetEntities().With<Character>().With<CharacterId>().AsMap<Position>();
			Buffs = world.GetEntities().With<BuffEffect>().AsMap<BuffEffectId>();
			CharacterBuffs = world.GetEntities().With<BuffEffect>().AsMultiMap<CharacterId>();
			Buttons = world.GetEntities().With<Button>().AsSet();
			LocalPlayerAddedListener = world.SubscribeComponentAdded<LocalPlayer>(OnLocalPlayerAdded);
			LocalPlayerRemovedListener = world.SubscribeComponentRemoved<LocalPlayer>(OnLocalPlayerRemoved);
			Recorder = new EntityCommandRecorder();
			Client = client;
			Camera = camera;
			Foreground = foreground;
			UI = ui;
		}

		public void Dispose() {
			CharacterIds.Dispose();
			Positions.Dispose();
			Buffs.Dispose();
			CharacterBuffs.Dispose();
			Buttons.Dispose();
			LocalPlayerAddedListener.Dispose();
			LocalPlayerRemovedListener.Dispose();
			Recorder.Dispose();
		}

		public void Load(
			ContentManager content,
			SessionPacket session
		) {
			SessionId = session.Id;
			Load(content, session.TileMapName);
		}

		public void Load(
			ContentManager content,
			string tileMapName
		) {
			TiledMap = Tiled.LoadMap(content, tileMapName);
			TileSize = new Vector2(TiledMap.TileWidth, TiledMap.TileHeight);
			HalfTileSize = TileSize / 2;
			Grid = Tiled.LoadGrid(TiledMap);
			IsReady = true;
		}

		public void Update() {
			Mouse = MouseExtended.GetState();
		}

		public Coord VectorToCoord(float x, float y) {
			return new Coord(
				Calc.Floor(x / TiledMap.TileWidth),
				Calc.Floor(y / TiledMap.TileHeight)
			);
		}

		public Coord VectorToCoord(Vector2 vector) {
			return VectorToCoord(vector.X, vector.Y);
		}

		public Vector2 CoordToVector(int x, int y) {
			return new Vector2(
				x * TiledMap.TileWidth,
				y * TiledMap.TileHeight
			);
		}

		public Vector2 CoordToMidVector(Position position) {
			return CoordToMidVector(position.Coord);
		}

		public Vector2 CoordToMidVector(Coord coord) {
			return CoordToMidVector(coord.X, coord.Y);
		}

		public Vector2 CoordToMidVector(int x, int y) {
			return CoordToVector(x, y) + HalfTileSize;
		}

		public Vector2 CoordToVector(Coord coord) {
			return CoordToVector(coord.X, coord.Y);
		}

		public Vector2 CoordToVector(Position position) {
			return CoordToVector(position.Coord);
		}

		public Coord ScreenToCoord(float x, float y) {
			return VectorToCoord(Camera.ScreenToWorld(x, y));
		}

		public Coord ScreenToCoord(Vector2 v) {
			return ScreenToCoord(v.X, v.Y);
		}

		public Node ScreenToNode(float x, float y) {
			return Grid.Get(ScreenToCoord(x, y));
		}

		public Node ScreenToNode(MouseStateExtended mouse) {
			return ScreenToNode(mouse.X, mouse.Y);
		}

		public bool GetEntityByCharacterId(int characterId, out Entity entity) {
			var character = new CharacterId(characterId);
			return CharacterIds.TryGetEntity(character, out entity);
		}

		public bool GetEntityByPosition(Position position, out Entity entity) {
			return Positions.TryGetEntity(position, out entity);
		}

		public bool GetEntityByPosition(Coord coord, out Entity entity) {
			return GetEntityByPosition(new Position { Coord = coord }, out entity);
		}

		public bool GetEntityByPosition(Node node, out Entity entity) {
			return GetEntityByPosition(node.Coord, out entity);
		}

		public Pathfinder CreatePathfinder(bool debug = false) {
			return new Pathfinder(Grid, Positions, debug);
		}

		public void Unload() {
			SessionId = 0;
			TiledMap = null;
			Grid = null;
			IsReady = false;
		}

		void OnLocalPlayerAdded(in Entity entity, in LocalPlayer value) {
			LocalPlayer = entity;
		}

		void OnLocalPlayerRemoved(in Entity entity, in LocalPlayer value) {
			LocalPlayer = null;
		}

	}

}
