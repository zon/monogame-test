using ldtk;
using Microsoft.Xna.Framework.Content;
using MonoGame.Aseprite.Documents;
using MonoGameTest.Common;
using System.Collections.Immutable;
using System.IO;

namespace MonoGameTest.Client {

	public class LevelResources {
		public readonly LdtkWorld World;
		public readonly ImmutableDictionary<long, AsepriteDocument> TilesetSprites;

		LevelResources(ContentManager content, string world) {
			World = new LdtkWorld(world);

			var builder = ImmutableDictionary.CreateBuilder<long, AsepriteDocument>();
			foreach (var def in World.Json.Defs.Tilesets) {
				var doc = content.Load<AsepriteDocument>(Path.GetFileNameWithoutExtension(def.RelPath));
				builder.Add(def.Uid, doc);
			}
			TilesetSprites = builder.ToImmutable();
		}

		public AsepriteDocument GetTilesetSprite(TilesetDefinition tileset) {
			return TilesetSprites[tileset.Uid];
		}

		public static LevelResources Load(ContentManager content, string world) {
			return new LevelResources(content, world);
		}

	}

}
