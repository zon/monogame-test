using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Tiled;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class Level {
		public readonly TiledMap TiledMap;
		public readonly Grid Grid;

		Level(TiledMap tiledMap, Grid grid) {
			TiledMap = tiledMap;
			Grid = grid;
		}

		public static Level Load(ContentManager content, string name) {
			var tiledMap = Tiled.LoadMap(content, name);
			var grid = Tiled.LoadGrid(tiledMap);
			return new Level(tiledMap, grid);
		}

	}

}
