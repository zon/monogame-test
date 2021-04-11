using System;
using System.Collections;
using System.Collections.Generic;
using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Closest : IEnumerator<Entity> {
		public readonly Coord Origin;
		public readonly EntitySet Positions;
		public readonly Func<Entity, bool> Critera;

		public Entity Current { get; private set; }
		public float Distance { get; private set; }

		public Closest(Coord origin, EntitySet positions, Func<Entity, bool> criteria) {
			Origin = origin;
			Positions = positions;
			Critera = criteria;
			Current = default;
			Distance = -1;
		}
		
		public Closest GetEnumerator() => new Closest(Origin, Positions, Critera);

		public bool MoveNext() {
			var found = false;
			var best = float.MaxValue;
			foreach (var entity in Positions.GetEntities()) {
				if (!Critera(entity)) continue;
				var position = entity.Get<Position>();
				var d = Coord.ChebyshevDistance(Origin, position.Coord);
				if (d > Distance && d < best) {
					Current = entity;
					best = d;
					found = true;
				}
			}
			Distance = best;
			return found;
		}

		public void Reset() {
			Distance = -1;
		}

		public void Dispose() {}

		object IEnumerator.Current => throw new System.NotImplementedException();

	}

}
