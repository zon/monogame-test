using ldtk;

namespace MonoGameTest.Common {

	public static class LayerInstanceExtension {

		public static Coord GetOffset(this LayerInstance layer, Level level, LayerDefinition definition) {
			return new Coord(
				(level.WorldX + definition.PxOffsetX + layer.PxOffsetX) / layer.GridSize,
				(level.WorldY + definition.PxOffsetY + layer.PxOffsetY) / layer.GridSize
			);
		}

	}

}
