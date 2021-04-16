using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct Bang {
		public int Delta;
		public float Progress;

		public bool IsDone => Progress >= 1;

		public void Start(int delta) {
			Delta = delta;
			Progress = 0;
		}

		public void Update(float dt, float duration) {
			Progress = Calc.Progress(Progress, duration, dt);
		}

		public static Bang Create() {
			return new Bang { Progress = 1 };
		}

	}

}
