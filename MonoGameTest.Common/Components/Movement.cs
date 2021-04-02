using System.Collections.Immutable;

namespace MonoGameTest.Common {

	public struct Movement {
		public ImmutableStack<Node> Path;

		public const int COST = 10;
		public const int EXTRA_DIAGONAL_COST = 4;

		public const float ACTION_DURATION = 0.25f;
		public const float PAUSE_DURATION = 1;

		public bool IsIdle() {
			return Path == null || Path.IsEmpty;
		}

	}

}
