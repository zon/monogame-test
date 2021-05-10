using DefaultEcs;
using DefaultEcs.Command;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class Context : IContext {
		public readonly GraphicsDevice GraphicsDevice;
		public readonly Resources Resources;
		public readonly Client Client;
		public World World { get; private set; }
		public EntityMap<CharacterId> Characters { get; private set; }
		public EntityMap<Position> Positions { get; private set; }
		public readonly EntityCommandRecorder Recorder;
		public int PeerId { get; private set; }
		public TiledMap TiledMap { get; private set; }
		public Vector2 TileSize { get; private set; }
		public Vector2 HalfTileSize { get; private set; }
		public Grid Grid { get; private set; }
		public readonly SpriteBatch Foreground;
		public readonly Camera Camera;
		public bool IsReady { get; private set; }

		public Context(
			GraphicsDevice graphicsDevice,
			Resources resources,
			World world,
			Client client,
			SpriteBatch foreground,
			Camera camera
		) {
			GraphicsDevice = graphicsDevice;
			Resources = resources;
			World = world;
			Characters = world.GetEntities().AsMap<CharacterId>();
			Positions = world.GetEntities().With<CharacterId>().AsMap<Position>();
			Recorder = new EntityCommandRecorder();
			Client = client;
			Foreground = foreground;
			Camera = camera;
		}

		public void Load(
			ContentManager content,
			GameWindow window,
			SessionPacket session
		) {
			PeerId = session.PeerId;
			Load(content, window, session.TileMapName);
		}

		public void Load(
			ContentManager content,
			GameWindow window,
			string tileMapName
		) {
			TiledMap = Tiled.LoadMap(content, tileMapName);
			TileSize = new Vector2(TiledMap.TileWidth, TiledMap.TileHeight);
			HalfTileSize = TileSize / 2;
			Grid = Tiled.LoadGrid(TiledMap);
			IsReady = true;
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

		public Node GetNode(float x, float y) {
			var p = Camera.ScreenToWorld(x, y);
			return Grid.Get(VectorToCoord(p));
		}

		public bool GetEntityByCharacterId(int characterId, out Entity entity) {
			var character = new CharacterId(characterId);
			return Characters.TryGetEntity(character, out entity);
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

		public void Unload() {
			PeerId = 0;
			TiledMap = null;
			Grid = null;
			IsReady = false;
		}
		

	}

}
