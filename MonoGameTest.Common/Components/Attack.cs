using System.Collections.Immutable;

namespace MonoGameTest.Common {

	public struct Attack {
		public int Id;
		public int Range;
		public int Damage;
		public float Lead;
		public float Follow;
		public float Cooldown;
		public string Animation;
		public string Projectile;
		public float ProjectleSpeed;

		public bool IsMelee => Range <= 1;
		public float Duration => Lead + Follow;

		public static Attack Get(int id) {
			// return List[id + 1];
			return new Attack {
				Id = 1,
				Damage = 5,
				Lead = Time.Frames(3),
				Follow = Time.Frames(3),
				Cooldown = 1,
				Animation = "sword-right"
			};
		}

		// static readonly ImmutableArray<Attack> List = ImmutableArray.Create(
		// 	new Attack {
		// 		Id = 1,
		// 		Damage = 5,
		// 		Lead = Time.Frames(3),
		// 		Follow = Time.Frames(3),
		// 		Cooldown = 1,
		// 		Animation = "sword-right"
		// 	},
		// 	new Attack {
		// 		Id = 2,
		// 		Range = 3,
		// 		Damage = 5,
		// 		Lead = Time.Frames(6),
		// 		Follow = Time.Frames(3),
		// 		Cooldown = 1,
		// 		Animation = "bow",
		// 		Projectile = "arrow",
		// 		ProjectleSpeed = 1
		// 	}
		// );

	}

}
