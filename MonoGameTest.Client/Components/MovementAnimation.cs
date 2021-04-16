using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct MovementAnimation {
		public Coord Previous;
		public Coord Facing;
		public float Progress;
		public float Duration;

		public void Start(Coord from, Coord to, float duration) {
			Previous = from;
			Facing = Coord.Facing(from, to);
			Progress = 0;
			Duration = duration;
		}

		public void Update(float dt) {
			Progress = Calc.Progress(Progress, Duration, dt);
		}

	}

}
