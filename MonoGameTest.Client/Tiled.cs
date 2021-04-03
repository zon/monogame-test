using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public static class Tiled {

		public static TiledMap LoadMap(ContentManager content, string name) {
			var map = content.Load<TiledMap>("first");
			map.GetLayer("zones").IsVisible = false;
			return map;
		}

		public static Grid LoadGrid(TiledMap map) {
			var nodes = new Node[map.Width * map.Height];
			var walls = map.GetLayer<TiledMapTileLayer>("walls");
			for (var y = 0; y < map.Height; y++) {
				for (var x = 0; x < map.Width; x++) {
					var i = map.Width * y + x;
					var tile = walls.GetTile(Convert.ToUInt16(x), Convert.ToUInt16(y));
					var solid = GetTileProperty(map, tile, "solid") == "true";
					nodes[i] = new Node(x, y, solid);
				}
			}
			return new Grid(map.Width, map.Height, nodes);
		}

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

		public static Node GetNode(TiledMap map, Grid grid, OrthographicCamera camera, float x, float y) {
			var p = camera.ScreenToWorld(x, y);
			return grid.Get(VectorToCoord(map, p));
		}

		static string GetTileProperty(TiledMap map, TiledMapTile tile, string name) {
			var tileset = map.GetTilesetByTileGlobalIdentifier(tile.GlobalIdentifier);
			var firstGid = map.GetTilesetFirstGlobalIdentifier(tileset);
			var id = tile.GlobalIdentifier - firstGid;
			if (id >= tileset.Tiles.Count) return "";
			var tilesetTile = tileset.Tiles[id];
			var value = "";
			tilesetTile.Properties.TryGetValue(name, out value);
			return value;
		}

	}

}
