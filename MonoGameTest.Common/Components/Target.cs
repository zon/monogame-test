using System;
using System.Collections.Immutable;
using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Target {
		
		public Nullable<Entity> Entity;
		public ImmutableStack<Node> Path;

	}

}
