namespace MonoGameTest.Common {

	public struct Cooldown {
		public float action;
		public float pause;

		public bool IsCool() {
			return action <= 0 && pause <= 0;
		}

		public bool IsActing() {
			return action > 0;
		}

	}

}
