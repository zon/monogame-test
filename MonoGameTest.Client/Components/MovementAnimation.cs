using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct MovementAnimation {
		public Coord Previous;
		public Coord Facing;
		public float Amount;
		public float Duration;

		public void Start(Coord from, Coord to, float duration) {
			Previous = from;
			Facing = Coord.Facing(from, to);
			Amount = 0;
			Duration = duration;
		}

	}

}
