using ldtk;

namespace MonoGameTest.Common {

	public static class LevelExtension {

		public static LayerInstance GetLayer(this Level level, long layerDefUid) {
			foreach(var layer in level.LayerInstances) {
				if (layer.LayerDefUid == layerDefUid) return layer;
			}
			return null;
		}

		public static LayerInstance GetLayer(this Level level, LayerDefinition layerDefinition) {
			return GetLayer(level, layerDefinition.Uid);
		}

	}

}
