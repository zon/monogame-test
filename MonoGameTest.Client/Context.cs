using DefaultEcs;
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
		public int PeerId { get; private set; }
		public TiledMap TiledMap { get; private set; }
		public Grid Grid { get; private set; }
		public SpriteBatch Foreground { get; private set; }
		public Camera Camera { get; private set; }
		public bool IsReady { get; private set; }

		public Context(
			GraphicsDevice graphicsDevice,
			Resources resources,
			World world,
			Client client,
			SpriteBatch foreground
		) {
			GraphicsDevice = graphicsDevice;
			Resources = resources;
			World = world;
			Client = client;
			Foreground = foreground;
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
			Grid = Tiled.LoadGrid(TiledMap);
			Camera = new Camera(window, GraphicsDevice, TiledMap);
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

		public Vector2 CoordToVector(Coord coord) {
			return CoordToVector(coord.X, coord.Y);
		}

		public Vector2 Half() {
			return new Vector2(TiledMap.TileWidth, TiledMap.TileHeight) / 2;
		}

		public Node GetNode(float x, float y) {
			var p = Camera.ScreenToWorld(x, y);
			return Grid.Get(VectorToCoord(p));
		}

		public void Unload() {
			PeerId = 0;
			TiledMap = null;
			Grid = null;
			Camera = null;
			IsReady = false;
		}
		

	}

}
