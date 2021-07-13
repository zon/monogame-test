using System.Collections.Generic;
using System.Collections.Immutable;
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

		public Level GetLevel(string identifier) {
			foreach(var level in Json.Levels) {
				if (level.Identifier == identifier) return level;
			}
			return null;
		}

		public LayerDefinition GetLayerDefinition(string identifier) {
			foreach (var layer in Json.Defs.Layers) {
				if (layer.Identifier == identifier) return layer;
			}
			return null;
		}

		public EntityDefinition GetEntityDefinition(string identifier) {
			foreach (var entity in Json.Defs.Entities) {
				if (entity.Identifier == identifier) return entity;
			}
			return null;
		}

		public TilesetDefinition GetTileset(long uid) {
			foreach(var tileset in Json.Defs.Tilesets) {
				if (tileset.Uid == uid) return tileset;
			}
			return null;
		}

		public ImmutableDictionary<Coord, Node> GetNodes() {
			var builder = ImmutableDictionary.CreateBuilder<Coord, Node>();
			var collisions = GetLayerDefinition("Collisions");
			var solid = collisions.GetIntDefinition("Solid").Value;
			foreach (var level in Json.Levels) {
				foreach (var layer in level.LayerInstances) {
					if (layer.LayerDefUid != collisions.Uid) continue;
					var offset = layer.GetOffset(level, collisions);
					for (var y = 0; y < layer.CHei; y++) {
						for (var x = 0; x < layer.CWid; x++) {
							var i = layer.CWid * y + x;
							var value = layer.IntGridCsv[i];
							var coord = offset + new Coord(x, y);
							var isSolid = value == solid;
							builder.Add(coord, new Node(coord, isSolid));
						}
					}
				}
			}
			return builder.ToImmutable();
		}

		public Spawn[] GetSpawns() {
			var spawns = new List<Spawn>();
			var entities = GetLayerDefinition("Entities");
			var start = GetEntityDefinition("Start");
			var mob = GetEntityDefinition("Mob");
			foreach (var level in Json.Levels) {
				var layer = level.GetLayer(entities);
				var offset = layer.GetOffset(level, entities);
				foreach (var entity in layer.EntityInstances) {
					var coord = offset + new Coord(entity.Grid);
					if (entity.DefUid == mob.Uid) {
						spawns.Add(Spawn.Mob(coord, Group.Red, 2));
					} else if (entity.DefUid == start.Uid) {
						spawns.Add(Spawn.Player(coord));
					}
				}
			}
			return spawns.ToArray();
		}

	}

}
