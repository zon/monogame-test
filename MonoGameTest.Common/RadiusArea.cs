using System.Collections;
using System.Collections.Generic;

namespace MonoGameTest.Common {

	public struct RadiusArea : IEnumerable {
		public readonly Coord Center;
		public readonly float Radius;

		public RadiusArea(Coord center, float radius) {
			Center = center;
			Radius = radius;
		}

		public IEnumerator<Coord> GetEnumerator() {
			var r = Calc.Ceiling(Radius);
			var rsqr = Radius * Radius;
			for (var y = -r; y <= r; y++) {
				for (var x = -r; x <= r; x++) {
					var c = Center + new Coord(x, y);
					if (Coord.DistanceSquared(Center, c) <= rsqr) yield return c;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	}

}
