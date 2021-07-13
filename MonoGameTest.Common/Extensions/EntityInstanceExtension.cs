using ldtk;

namespace MonoGameTest.Common {

	public static class EntityInstanceExtension {

		public static T GetFieldValue<T>(this EntityInstance entity, FieldDefinition field) {
			foreach(var f in entity.FieldInstances) {
				if (f.DefUid == field.Uid) return f.Value;
			}
			return default;
		}

	}

}
