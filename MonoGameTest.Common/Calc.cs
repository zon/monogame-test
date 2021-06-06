using System;

namespace MonoGameTest.Common {

	public static class Calc {

		public static int Max(int a, int b) {
			return a > b ? a : b;
		}

		public static int Floor(float v) {
			return Convert.ToInt32(Math.Floor(v));
		}

		public static int Ceiling(float v) {
			return Convert.ToInt32(Math.Ceiling(v));
		}

		public static float Progress(float progress, float duration, float dt) {
			return Math.Clamp(progress + dt / duration, 0, 1);
		}
		
	}

}
