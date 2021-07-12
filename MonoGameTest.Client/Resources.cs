using ldtk;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGameTest.Common;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace MonoGameTest.Client {

	public class Resources {
		public readonly LdtkWorld World;
		public readonly ImmutableDictionary<long, AsepriteDocument> TilesetSprites;
		public readonly AsepriteDocument Characters;
		public readonly AsepriteDocument Attacks;
		public readonly AsepriteDocument Effects;
		public readonly AsepriteDocument Hits;
		public readonly ButtonResource Button;
		public readonly AsepriteDocument SkillIcons;
		public readonly AsepriteDocument Highlight;
		public readonly SoundEffect MoveSound;
		public readonly SoundEffect HitSound;
		public readonly SoundEffect MoveConfirmSound;
		public readonly BitmapFont Font;

		Resources(ContentManager content) {
			World = new LdtkWorld("dungeon");

			var builder = ImmutableDictionary.CreateBuilder<long, AsepriteDocument>();
			foreach (var def in World.Json.Defs.Tilesets) {
				var doc = content.Load<AsepriteDocument>(Path.GetFileNameWithoutExtension(def.RelPath));
				builder.Add(def.Uid, doc);
			}
			TilesetSprites = builder.ToImmutable();

			Characters = content.Load<AsepriteDocument>("entities");
			Attacks = content.Load<AsepriteDocument>("attacks");
			Effects = content.Load<AsepriteDocument>("effects-3x3");
			Hits = content.Load<AsepriteDocument>("hits");
			Button = new ButtonResource(content);
			SkillIcons = content.Load<AsepriteDocument>("skills");
			Highlight = content.Load<AsepriteDocument>("highlight");
			MoveSound = content.Load<SoundEffect>("bump-strike-0");
			HitSound = content.Load<SoundEffect>("bump-strike-1");
			MoveConfirmSound = content.Load<SoundEffect>("bump-strike-8");
			Font = content.Load<BitmapFont>("munro");
		}

		public AsepriteDocument GetSprite(SpriteFile? file) {
			switch (file) {
				case SpriteFile.Attacks:
					return Attacks;
				case SpriteFile.Effects:
					return Effects;
				case SpriteFile.Hits:
					return Hits;
			}
			return null;
		}

		public AnimatedSprite GetAnimatedSprite(SpriteFile? file) {
			var doc = GetSprite(file);
			if (doc == null) return null;
			return new AnimatedSprite(doc);
		}

		public AnimatedSprite GetAnimatedSprite(SpriteLocation? location) {
			return GetAnimatedSprite(location?.File);
		}

		public AsepriteDocument GetTilesetSprite(TilesetDefinition tileset) {
			return TilesetSprites[tileset.Uid];
		}

		public static Resources Load(ContentManager content) {
			return new Resources(content);
		}

	}

}
