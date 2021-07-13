using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using DefaultEcs;
using DefaultEcs.Command;
using MonoGameTest.Common;
using TiledCS;

namespace MonoGameTest.Server {
 	public class Context : IContext {
		public readonly Server Server;
		public LdtkWorld Level { get; private set; }
		public World World { get; private set; }
		public EntityCommandRecorder Recorder { get; private set; }
		public EntitySet Characters { get; private set; }
		public EntityMap<CharacterId> CharacterIds { get; private set; }
		public EntityMultiMap<CharacterId> CharacterBuffs { get; private set; }
		public EntityMap<Position> Positions { get; private set; }
		public EntityMap<Player> PlayerIds { get; private set; }
		public Grid Grid { get; private set; }
		public bool IsReady { get; private set; }

		public Context(Server server, World world) {
			Server = server;
			World = world;
			Recorder = new EntityCommandRecorder();
			Characters = World.GetEntities().With<Character>().AsSet();
			CharacterIds = World.GetEntities().With<Character>().AsMap<CharacterId>();
			CharacterBuffs = World.GetEntities().With<BuffEffect>().AsMultiMap<CharacterId>();
			Positions = World.GetEntities().With<Character>().With<CharacterId>().AsMap<Position>();
			PlayerIds = World.GetEntities().AsMap<Player>();
		}

		public void Dispose() {
			Recorder.Dispose();
			Characters.Dispose();
			CharacterIds.Dispose();
			CharacterBuffs.Dispose();
			Positions.Dispose();
			PlayerIds.Dispose();
		}

		public void Load(string name) {
			Level = new LdtkWorld(name);
			var nodes = Level.GetNodes();
			var spawns = Level.GetSpawns();
			Grid = new Grid(nodes, spawns);
			IsReady = true;
		}

		public void Unload() {
			Grid = null;
			IsReady = false;
		}

		public Pathfinder CreatePathfinder(bool debug = false) {
			return new Pathfinder(Grid, Positions, debug);
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
