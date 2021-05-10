using System.Collections.Immutable;

namespace MonoGameTest.Common {

	public class Attack {
		public readonly int Id;
		public readonly int Range;
		public readonly int Damage;
		public readonly float Lead;
		public readonly float Follow;
		public readonly float Cooldown;
		public readonly string Animation;
		public readonly string Projectile;
		public readonly float ProjectleSpeed;

		public bool IsMelee => Range <= 1;
		public float Duration => Lead + Follow;

		static int AutoId = 0;

		static readonly ImmutableArray<Attack> List = ImmutableArray.Create(
			new Attack(
				damage: 5,
				lead: Time.Frames(3),
				follow: Time.Frames(3),
				animation: "sword-right"
			),
			new Attack(
				damage: 5,
				lead: Time.Frames(6),
				follow: Time.Frames(3),
				animation: "bow",
				range: 3,
				projectile: "arrow",
				projectileSpeed: 1
			)
		);

		public static Attack Get(int id) {
			return List[id - 1];
		}

		public Attack(
			int damage,
			float lead,
			float follow,
			string animation,
			int range = 1,
			string projectile = "",
			float projectileSpeed = 0,
			float cooldown = 1
		) {
			Id = ++AutoId;
			Damage = damage;
			Lead = lead;
			Follow = follow;
			Animation = animation;
			Range = range;
			Projectile = projectile;
			ProjectleSpeed = projectileSpeed;
			Cooldown = cooldown;
		}

	}

}
