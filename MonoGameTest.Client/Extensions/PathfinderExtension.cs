using Microsoft.Xna.Framework;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public static class PathfinderExtension {

		public static Node Cast(this Pathfinder pathfinder, Vector2 a, Vector2 b) {
			return pathfinder.Cast(a.X, a.Y, b.X, b.Y);
		}

	}

}
