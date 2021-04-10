using System;

namespace MonoGameTest.Common {

	public struct Movement {
		public Nullable<Coord> Goal;

		public const float COST = 1;
		public const float DIAGONAL_COST = 1.41f;

		public const float ACTION_DURATION = 0.25f;
		public const float PAUSE_DURATION = 1;

		public bool IsIdle() {
			return Goal == null;
		}

	}

}
