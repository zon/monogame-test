namespace MonoGameTest.Common {

	public static class Time {

		public const float FRAME = 0.08f;

		public static float Frames(int count) {
			return FRAME * count;
		}

	}

}
