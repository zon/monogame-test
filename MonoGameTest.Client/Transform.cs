using Microsoft.Xna.Framework;

namespace MonoGameTest.Client {

	public struct Transform {
		public readonly Matrix Matrix;
		public readonly Matrix InverseMatrix;

		public Transform(Vector2 position, float scale) {
			Matrix = Matrix.CreateTranslation(position.X, position.Y, 0) * Matrix.CreateScale(scale);
			InverseMatrix = Matrix.Invert(Matrix);
		}

	}

}
