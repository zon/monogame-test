using ldtk;

namespace MonoGameTest.Common {

	public static class LayerDefinitionExtension {

		public static IntGridValueDefinition GetIntDefinition(this LayerDefinition layer, string identifier) {
			foreach (var gv in layer.IntGridValues) {
				if (gv.Identifier == identifier) return gv;
			}
			return null;
		}

	}

}
