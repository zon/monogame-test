using Microsoft.Xna.Framework;

namespace MonoGameTest.Client {

	public struct Camera {
		public CameraView View;
		public Vector2 Center;
		public Vector2 Offset;
		public Vector2 Target;
		public float Speed;

		public void LookAt(Vector2 position) {
			Center = position;
			Target = position;
			View.LookAt(Center + Offset);
		}

	}

}
