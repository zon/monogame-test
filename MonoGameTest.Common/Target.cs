using System.Collections.Immutable;
using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Target {
		public readonly Entity? Entity;
		public readonly Coord? Coord;
		
		public bool IsMobile => Entity != null;
		public bool IsFixed => Coord != null;
		public bool IsValid => Entity?.IsAlive ?? true;

		public Target(Entity entity) {
			Entity = entity;
			Coord = null;
		}

		public Target(Coord coord) {
			Entity = null;
			Coord = coord;
		}

		public Coord GetPosition() {
			if (IsMobile) {
				return Entity.Value.Get<Position>().Coord;

			} else {
				return Coord.Value;
			}
		}

	}

}
