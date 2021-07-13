using ldtk;

namespace MonoGameTest.Common {

	public static class EntityDefinitionExtension {

		public static FieldDefinition GetFieldDefinition(this EntityDefinition entity, string identifier) {
			foreach (var field in entity.FieldDefs) {
				if (field.Identifier == identifier) return field;
			}
			return null;
		}

	}

}
