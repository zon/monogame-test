using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Projectile {
		public Coord Origin;
		public Entity Target;
		public Skill Skill;
		public float Lifetime;
		public float Timeout;

		public Projectile(Coord origin, Entity target, Skill skill) {
			ref var targetPosition = ref target.Get<Position>();
			Origin = origin;
			Target = target;
			Skill = skill;
			Lifetime = Coord.Distance(origin, targetPosition.Coord) / skill.ProjectleSpeed;
			Timeout = Lifetime;
		}

	}

}
