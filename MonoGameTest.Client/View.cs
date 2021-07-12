using System;
using Microsoft.Xna.Framework;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public static class View {
		public const int WIDTH = SCREEN_WIDTH / SCALE;
		public const int HEIGHT = SCREEN_HEIGHT / SCALE;
		public const int SCALE = 4;
		public const int TILE = 16;
		public const int ENERGY_BAR_HEIGHT = 6;
		public const int SKILL_BAR_HEIGHT = 24;
		public const int UI_HEIGHT = ENERGY_BAR_HEIGHT + SKILL_BAR_HEIGHT;
		public const int SCREEN_WIDTH = 1280;
		public const int SCREEN_HEIGHT = 720;

		public const float RADIAN = MathF.PI * 2;

		public static Vector2 ToVector(float radians) {
			return new Vector2(MathF.Cos(radians), MathF.Sin(radians));
		}

		public static float ToRadians(Vector2 vector) {
			return MathF.Atan2(vector.Y, vector.X);
		}

		public static float ToRadians(Coord coord) {
			return MathF.Atan2(coord.Y, coord.X);
		}

		public static Point ToPoint(long[] pair) {
			return new Point((int) pair[0], (int) pair[1]);
		}

	}

}
