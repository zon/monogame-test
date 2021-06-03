namespace MonoGameTest.Common {

	public struct SpriteLocation {
		public readonly string Tag;
		public readonly SpriteFile File;

		public SpriteLocation(string tag, SpriteFile file = SpriteFile.Effects) {
			Tag = tag;
			File = file;
		}

	}

}
