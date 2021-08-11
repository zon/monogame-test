using Microsoft.Xna.Framework;

namespace MonoGameTest.Client {

	public static class Vector {
		
		public static Vector2 Move(Vector2 position, Vector2 target, float maxSpeed) {
			if (position == target) return target;
			var delta = target - position;
			if (delta.LengthSquared() > maxSpeed * maxSpeed) {
				delta.Normalize();
				delta *= maxSpeed;
			}
			return position + delta;
		}

	}

}
