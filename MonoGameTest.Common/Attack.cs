using System;
using System.Collections.Immutable;

namespace MonoGameTest.Common {

	public class Attack : IEquatable<Attack> {
		public readonly int Id;
		public readonly Targeting Targeting;
		public readonly int Range;
		public readonly int Damage;
		public readonly float Lead;
		public readonly float Follow;
		public readonly float Cooldown;
		public readonly string Icon;
		public readonly string Animation;
		public readonly string Projectile;
		public readonly float ProjectleSpeed;
		public readonly bool Repeating;

		public bool IsMelee => Range <= 1;
		public float Duration => Lead + Follow;

		static int AutoId = 0;

		static readonly ImmutableArray<Attack> List = ImmutableArray.Create(
			new Attack(
				damage: 5,
				lead: Time.Frames(3),
				follow: Time.Frames(3),
				icon: "sword",
				animation: "sword-right",
				repeating: true
			),
			new Attack(
				damage: 5,
				lead: Time.Frames(6),
				follow: Time.Frames(3),
				icon: "bow",
				animation: "bow",
				range: 3,
				projectile: "arrow",
				projectileSpeed: 12,
				repeating: true
			),
			new Attack(
				damage: 10,
				lead: Time.Frames(3),
				follow: Time.Frames(3),
				icon: "poison",
				animation: "sword-right"
			),
			new Attack(
				damage: 10,
				lead: Time.Frames(6),
				follow: Time.Frames(3),
				icon: "bow",
				animation: "bow",
				range: 4,
				projectile: "arrow",
				projectileSpeed: 12
			)
		);

		public static Attack Get(int id) {
			var index = id - 1;
			if (index < 0 || index > List.Length) return null;
			return List[index];
		}

		public Attack(
			int damage,
			float lead,
			float follow,
			string icon,
			string animation,
			Targeting targeting = Targeting.Character,
			int range = 1,
			string projectile = "",
			float projectileSpeed = 0,
			float cooldown = 1,
			bool repeating = false
		) {
			Id = ++AutoId;
			Damage = damage;
			Lead = lead;
			Follow = follow;
			Icon = icon;
			Animation = animation;
			Targeting = targeting;
			Range = range;
			Projectile = projectile;
			ProjectleSpeed = projectileSpeed;
			Cooldown = cooldown;
			Repeating = repeating;
		}

		public bool InRange(Coord position, Coord target) {
			if (IsMelee) {
				var d = Coord.ChebyshevDistance(position, target);
				if (d > 1) return false;
			} else {
				var d = Coord.DistanceSquared(position, target);
				if (d > Range * Range) return false;
			}
			return true;
		}

		public bool InRange(Position position, Position target) {
			return InRange(position.Coord, target.Coord);
		}

		public bool IsValidTarget(Pathfinder pathfinder, Coord position, Coord target) {
			return (
				position != target &&
				InRange(position, target) &&
				pathfinder.HasSight(position, target) &&
				pathfinder.HasEntity(target)
			);
		}

		public override int GetHashCode() => Id;

		public override bool Equals(object obj) => obj is Attack other && Equals(other);

		public bool Equals(Attack other) {
			return Equals(this, other);
		}

		public static bool Equals(Attack left, Attack right) => left?.Id == right?.Id;

		public static bool operator ==(Attack left, Attack right) => Equals(left, right);

		public static bool operator !=(Attack left, Attack right) => !Equals(left, right);

	}

}
