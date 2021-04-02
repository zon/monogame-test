using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public static class Tiled {

		public static Coord VectorToCoord(TiledMap map, float x, float y) {
			return new Coord(
				Calc.Floor(x / map.TileWidth),
				Calc.Floor(y / map.TileHeight)
			);
		}

		public static Coord VectorToCoord(TiledMap map, Vector2 vector) {
			return VectorToCoord(map, vector.X, vector.Y);
		}

		public static Vector2 CoordToVector(TiledMap map, int x, int y) {
			return new Vector2(
				x * map.TileWidth,
				y * map.TileHeight
			);
		}

		public static Vector2 CoordToVector(TiledMap map, Coord coord) {
			return CoordToVector(map, coord.X, coord.Y);
		}

	}

}
