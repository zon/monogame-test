using System;
using System.Collections.Immutable;

namespace MonoGameTest.Common {

	public class Skill : IEquatable<Skill> {
		public readonly int Id;
		public readonly Targeting Targeting;
		public readonly int Range;
		public readonly int Damage;
		public readonly float Area;
		public readonly float Charge;
		public readonly float Lead;
		public readonly float Follow;
		public readonly float Timeout;
		public readonly float Cooldown;
		public readonly string Icon;
		public readonly SpriteLocation? ChargeSprite;
		public readonly SpriteLocation CastSprite;
		public readonly SpriteLocation? ProjectileSprite;
		public readonly SpriteLocation? ImpactSprite;
		public readonly float ProjectleSpeed;
		public readonly Buff Buff;
		public readonly bool Repeating;

		public bool IsMelee => Range <= 1;
		public bool IsRanged => Range > 0;
		public float Duration => Charge + Lead + Follow;
		public bool HasAreaEffect => Area > 0;

		static int AutoId = 0;

		static readonly ImmutableArray<Skill> List = ImmutableArray.Create(
			new Skill(
				damage: 5,
				lead: Time.Frames(3),
				follow: Time.Frames(3),
				icon: "sword",
				castSprite: new SpriteLocation("sword-right"),
				repeating: true
			),
			new Skill(
				damage: 5,
				lead: Time.Frames(6),
				follow: Time.Frames(3),
				icon: "bow",
				castSprite: new SpriteLocation("bow"),
				range: 3,
				projectileSprite: new SpriteLocation("arrow"),
				projectileSpeed: 12,
				repeating: true
			),
			new Skill(
				damage: 5,
				buff: new Buff(10, 1),
				lead: Time.Frames(3),
				follow: Time.Frames(3),
				icon: "poison",
				castSprite: new SpriteLocation("sword-right"),
				cooldown: 10
			),
			new Skill(
				damage: 10,
				lead: Time.Frames(6),
				follow: Time.Frames(3),
				icon: "bow",
				castSprite: new SpriteLocation("bow"),
				range: 4,
				projectileSprite: new SpriteLocation("arrow"),
				projectileSpeed: 12,
				cooldown: 15
			),
			new Skill(
				icon: "fireball",
				range: 4,
				damage: 15,
				area: 1,
				charge: 2,
				lead: Time.Frames(6),
				follow: Time.Frames(5),
				chargeSprite: new SpriteLocation("fire-charge"),
				castSprite: new SpriteLocation("fire-cast"),
				projectileSprite: new SpriteLocation("fireball"),
				impactSprite: new SpriteLocation("fireball-impact", SpriteFile.Effects),
				projectileSpeed: 10,
				cooldown: 1
			)
		);

		public static Skill Get(int id) {
			var index = id - 1;
			if (index < 0 || index > List.Length) return null;
			return List[index];
		}

		public static Skill GetByIcon(string icon) {
			foreach (var skill in List) {
				if (skill.Icon == icon) return skill;
			}
			return null;
		}

		public Skill(
			int damage,
			float lead,
			float follow,
			string icon,
			SpriteLocation castSprite,
			SpriteLocation? chargeSprite = null,
			SpriteLocation? projectileSprite = null,
			SpriteLocation? impactSprite = null,
			Targeting targeting = Targeting.Character,
			int range = 1,
			float projectileSpeed = 0,
			float area = 0,
			float charge = 0,
			float timeout = 1,
			float cooldown = 0,
			bool repeating = false,
			Buff buff = null
		) {
			Id = ++AutoId;
			Damage = damage;
			Lead = lead;
			Follow = follow;
			Icon = icon;
			CastSprite = castSprite;
			ChargeSprite = chargeSprite;
			ProjectileSprite = projectileSprite;
			ImpactSprite = impactSprite;
			Targeting = targeting;
			Range = range;
			ProjectleSpeed = projectileSpeed;
			Area = area;
			Charge = charge;
			Timeout = timeout;
			Cooldown = cooldown;
			Repeating = repeating;
			Buff = buff?.Complete(this);
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
