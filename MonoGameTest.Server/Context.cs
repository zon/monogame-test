using System.Linq;
using DefaultEcs;
using MonoGameTest.Common;
using TiledCS;

namespace MonoGameTest.Server {

	public class Context : IContext {
		const string CONTENT_PATH = "../MonoGameTest.Client/Content";
		
		public readonly Server Server;
		public World World { get; private set; }
		public Grid Grid { get; private set; }
		public bool IsReady { get; private set; }

		public Context(Server server, World world) {
			Server = server;
			World = world;
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
					var solid = tile?.properties.First(p => p.name == "solid")?.value == "true";
					nodes[i] = new Node(x, y, solid);
				}
			}
			Grid = new Grid(map.Width, map.Height, nodes);
			IsReady = true;
		}

		public void Unload() {
			Grid = null;
			IsReady = false;
		}

	}

}
