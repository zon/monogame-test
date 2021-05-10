using System.Numerics;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public static class CoordExtension {

		public static Vector2 ToVector(this Coord coord) {
			return new Vector2(coord.X, coord.Y);
		}

	}

}
