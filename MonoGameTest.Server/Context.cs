using System;
using System.Collections.Generic;
using System.Linq;
using DefaultEcs;
using MonoGameTest.Common;
using TiledCS;

namespace MonoGameTest.Server {

	public class Context : IContext {
		const string CONTENT_PATH = "../MonoGameTest.Client/Content";
		
		public readonly Server Server;
		public World World { get; private set; }
		public EntityMap<Position> Positions { get; private set; }
		public Grid Grid { get; private set; }
		public bool IsReady { get; private set; }

		public Context(Server server, World world) {
			Server = server;
			World = world;
			Positions = World.GetEntities().AsMap<Position>();
		}

		public void Load(string name) {
			var path = $"{CONTENT_PATH}/{name}.tmx";
			var map = new TiledMap(path);
			var tilesets = map.GetTiledTilesets(path);

			var nodes = new Node[map.Width * map.Height];
			var walls = map.Layers.First(l => l.name == "walls");
			for (var y = 0; y < map.Height; y++) {
				for (var x = 0; x < map.Width; x++) {
					var i = map.Width * y + x;
					var gid = walls.data[i];
					var tileset = map.GetTiledMapTileset(gid);
					var tile = map.GetTiledTile(tileset, tilesets[tileset.firstgid], gid);
					var solid = getTileBool(tile, "solid");
					nodes[i] = new Node(x, y, solid);
				}
			}

			var spawns = new List<Spawn>();
			var characters = map.Layers.First(l => l.name == "characters");
			for (var i = 0; i < characters.data.Length; i++) {
				var gid = characters.data[i];
				var tileset = map.GetTiledMapTileset(gid);
				var tile = map.GetTiledTile(tileset, tilesets[tileset.firstgid], gid);

				var x = i % map.Width;
				var y = Calc.Floor(i / map.Width);
				var coord = new Coord(x, y);
				
				var sprite = gid - tileset.firstgid;

				var group = getTileInt(tile, "group");
				if (group != 0) {
					spawns.Add(new Spawn(coord, (Group) group, sprite));

				} else if (getTileBool(tile, "spawn")) {
					spawns.Add(new Spawn(coord, Group.Player, sprite));
				}
			}

			Grid = new Grid(map.Width, map.Height, nodes, spawns.ToArray());
			IsReady = true;
		}

		public void Unload() {
			Grid = null;
			IsReady = false;
		}

		TiledProperty GetTileProperty(TiledTile tile, string name) {
			if (tile == null) return null;
			return tile.properties.FirstOrDefault(p => p.name == name);
		}

		bool getTileBool(TiledTile tile, string name) {
			return GetTileProperty(tile, name)?.value == "true";
		}

		int getTileInt(TiledTile tile, string name) {
			var p = GetTileProperty(tile, name);
			if (p != null) {
				return Convert.ToInt32(p.value);
			} else {
				return 0;
			}
		}

	}

}
