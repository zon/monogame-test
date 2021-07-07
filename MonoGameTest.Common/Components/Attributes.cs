using System;
using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Attributes {
		public string Sprite;
		public int Power;
		public int MoveCoolown;
		public int Health;
		public int Energy;
		public float EnergyGen;
		public int Defense;

		public static Attributes Zero = new Attributes();

		public Attributes(
			string sprite = "mob",
			int power = 5,
			int moveCooldown = 1,
			int health = 100,
			int energy = 100,
			float energyGen = 5,
			int defense = 0
		) {
			Sprite = sprite;
			Power = power;
			MoveCoolown = moveCooldown;
			Health = health;
			Energy = energy;
			EnergyGen = energyGen;
			Defense = defense;
		}

		public bool Equals(Attributes other) => (
			Sprite == other.Sprite &&
			Power == other.Power &&
			MoveCoolown == other.MoveCoolown &&
			Health == other.Health &&
			Energy == other.Energy &&
			EnergyGen == other.EnergyGen &&
			Defense == other.Defense
		);

		public override bool Equals(object obj) => obj is Attributes other && Equals(other);

		public override int GetHashCode() => (
			Sprite.GetHashCode() * 10007 +
			Power * 10427 +
			MoveCoolown * 10631 +
			Health * 10883 +
			Energy * 11213 +
			Calc.Floor(EnergyGen * 102181) +
			Defense * 11923
		);

		public void Total(IContext context, CharacterId characterId) {
			var total = Attributes.Zero;

			Entity entity;
			if (context.CharacterIds.TryGetEntity(characterId, out entity)) {
				ref var character = ref entity.Get<Character>();
				total += character.Role.Attributes;
			}

			ReadOnlySpan<Entity> entities;
			if (context.CharacterBuffs.TryGetEntities(characterId, out entities)) {
				foreach (var e in entities) {
					ref var buff = ref e.Get<Buff>();
					if (buff.Attributes == null) continue;
					total += buff.Attributes.Value;
				}
			}

			Set(total);
		}

		public void Set(Attributes other) {
			Sprite = other.Sprite;
			Power = other.Power;
			MoveCoolown = other.MoveCoolown;
			Health = other.Health;
			Energy = other.Energy;
			EnergyGen = other.EnergyGen;
			Defense = other.Defense;
		}

		public int Attack(Attributes target) {
			return Math.Max(Power - target.Defense, 0);
		}

		// TODO
		// public Buff DamageOverTime(Attributes target) {
			
		// }

		public static bool operator ==(Attributes a, Attributes b) => a.Equals(b);
		public static bool operator !=(Attributes a, Attributes b) => !a.Equals(b);

		public static Attributes operator +(Attributes a, Attributes b) => new Attributes {
			Sprite = b.Sprite,
			Power = a.Power + b.Power,
			MoveCoolown = a.MoveCoolown + b.MoveCoolown,
			Health = a.Health + b.Health,
			Energy = a.Energy + b.Energy,
			EnergyGen = a.EnergyGen + b.EnergyGen,
			Defense = a.Defense + b.Defense
		};

		public static Attributes operator -(Attributes a, Attributes b) => new Attributes {
			Sprite = a.Sprite,
			Power = a.Power - b.Power,
			MoveCoolown = a.MoveCoolown - b.MoveCoolown,
			Health = a.Health - b.Health,
			Energy = a.Energy - b.Energy,
			EnergyGen = a.EnergyGen - b.EnergyGen,
			Defense = a.Defense - b.Defense
		};

	}

}
