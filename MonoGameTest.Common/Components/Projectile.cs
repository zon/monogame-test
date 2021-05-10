using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Projectile {
		public Coord Origin;
		public Entity Target;
		public Attack Attack;
		public float Lifetime;
		public float Timeout;

		public Projectile(Coord origin, Entity target, Attack attack) {
			ref var targetPosition = ref target.Get<Position>();
			Origin = origin;
			Target = target;
			Attack = attack;
			Lifetime = Coord.Distance(origin, targetPosition.Coord) / attack.ProjectleSpeed;
			Timeout = Lifetime;
		}

	}

}
