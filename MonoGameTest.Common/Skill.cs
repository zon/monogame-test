using System;
using System.Collections.Immutable;

namespace MonoGameTest.Common {

	public class Skill : IEquatable<Skill> {
		public readonly int Id;
		public readonly Targeting Targeting;
		public readonly int Range;
		public readonly int Damage;
		public readonly float Lead;
		public readonly float Follow;
		public readonly float Timeout;
		public readonly float Cooldown;
		public readonly string Icon;
		public readonly string Animation;
		public readonly string Projectile;
		public readonly float ProjectleSpeed;
		public readonly bool Repeating;

		public bool IsMelee => Range <= 1;
		public bool IsRanged => Range > 0;
		public float Duration => Lead + Follow;

		static int AutoId = 0;

		static readonly ImmutableArray<Skill> List = ImmutableArray.Create(
			new Skill(
				damage: 5,
				lead: Time.Frames(3),
				follow: Time.Frames(3),
				icon: "sword",
				animation: "sword-right",
				repeating: true
			),
			new Skill(
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
			new Skill(
				damage: 10,
				lead: Time.Frames(3),
				follow: Time.Frames(3),
				icon: "poison",
				animation: "sword-right",
				cooldown: 10
			),
			new Skill(
				damage: 10,
				lead: Time.Frames(6),
				follow: Time.Frames(3),
				icon: "bow",
				animation: "bow",
				range: 4,
				projectile: "arrow",
				projectileSpeed: 12,
				cooldown: 15
			)
		);

		public static Skill Get(int id) {
			var index = id - 1;
			if (index < 0 || index > List.Length) return null;
			return List[index];
		}

		public Skill(
			int damage,
			float lead,
			float follow,
			string icon,
			string animation,
			Targeting targeting = Targeting.Character,
			int range = 1,
			string projectile = "",
			float projectileSpeed = 0,
			float timeout = 1,
			float cooldown = 0,
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
			Timeout = timeout;
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

		public bool IsValidMeleeTarget(Coord position, Coord target) {
			return (
				position != target &&
				InRange(position, target)
			);
		}

		public override int GetHashCode() => Id;

		public override bool Equals(object obj) => obj is Skill other && Equals(other);

		public bool Equals(Skill other) {
			return Equals(this, other);
		}

		public static bool Equals(Skill left, Skill right) => left?.Id == right?.Id;

		public static bool operator ==(Skill left, Skill right) => Equals(left, right);

		public static bool operator !=(Skill left, Skill right) => !Equals(left, right);

	}

}
