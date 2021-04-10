using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct CharacterView {
		public CharacterViewState State;
		public Coord Previous;
		public float MoveAmount;
		public float MoveDuration;

		public static CharacterView Create(float moveDuration = 0.2f) {
			return new CharacterView { MoveDuration = moveDuration };
		}

	}

	public enum CharacterViewState {
		Spawned,
		Idle,
		Moving
	}

}
