using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Projectile {
		public Coord Origin;
		public Entity Target;
		public Coord TargetCoord;
		public Skill Skill;
		public Attributes? Attributes;
		public float Lifetime;
		public float Timeout;

		public Projectile(Coord origin, Entity target, Skill skill, Attributes? attributes = null) {
			ref var targetPosition = ref target.Get<Position>();
			Origin = origin;
			Target = target;
			TargetCoord = targetPosition.Coord;
			Skill = skill;
			Attributes = attributes;
			Lifetime = Coord.Distance(origin, targetPosition.Coord) / skill.ProjectleSpeed;
			Timeout = Lifetime;
		}

	}

}
