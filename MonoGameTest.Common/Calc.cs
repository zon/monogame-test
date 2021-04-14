using System;

namespace MonoGameTest.Common {

	public static class Calc {

		public static int Max(int a, int b) {
			return a > b ? a : b;
		}

		public static int Floor(float v) {
			return Convert.ToInt32(Math.Floor(v));
		}
		
	}

}
