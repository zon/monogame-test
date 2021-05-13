using System;
using Microsoft.Xna.Framework;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public static class View {
		public const int WIDTH = 256;
		public const int HEIGHT = 256;
		public const int SCALE = 3;
		public const int TILE = 16;
		public const int SKILL_BAR_HEIGHT = 24; 
		public const int SCREEN_WIDTH = WIDTH;
		public const int SCREEN_HEIGHT = HEIGHT + SKILL_BAR_HEIGHT;

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

	}

}
