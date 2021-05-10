using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Projectile {
		public Coord Origin;
		public Entity Target;
		public Attack Attack;
		public float Lifetime;
		public float Timeout;
	}

}
