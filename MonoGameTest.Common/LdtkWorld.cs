using System.IO;
using System.Reflection;
using ldtk;

namespace MonoGameTest.Common {

	public class LdtkWorld {
		public readonly LdtkJson Json;

		public LdtkWorld(string name) {
			var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var path = Path.Combine(location, "Resources", name +".ldtk");
			var text = File.ReadAllText(path);
			Json = LdtkJson.FromJson(text);
		}

		public Level GetLevel(long uid) {
			foreach(var level in Json.Levels) {
				if (level.Uid == uid) return level;
			}
			return null;
		}

		public Level GetLevel(string name) {
			foreach(var level in Json.Levels) {
				if (level.Identifier == name) return level;
			}
			return null;
		}

		public LayerInstance GetLayer(long levelUid, string layerName) {
			var level = GetLevel(levelUid);
			foreach(var layer in level.LayerInstances) {
				if (layer.Identifier == layerName) return layer;
			}
			return null;
		}

		public TilesetDefinition GetTileset(long uid) {
			foreach(var tileset in Json.Defs.Tilesets) {
				if (tileset.Uid == uid) return tileset;
			}
			return null;
		}

	}

}
