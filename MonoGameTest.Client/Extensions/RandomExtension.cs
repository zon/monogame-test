using System;

namespace MonoGameTest.Client {

	public static class RandomExtension {

		public static float NextFloat(this Random random) {
			return (float) random.NextDouble();
		}

	}

}
