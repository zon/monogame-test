namespace MonoGameTest.Common {

	public class Grid {
		public readonly int Width;
		public readonly int Height;
		public readonly Node[] Nodes;

		public Grid(int width, int height, Node[] nodes) {
			this.Width = width;
			this.Height = height;
			this.Nodes = nodes;
		}

		public bool InBounds(int x, int y) {
			return x >= 0 && x < Width && y >= 0 && y < Height;
		}

		public Node Get(int x, int y) {
			if (!InBounds(x, y)) return null;
			return Nodes[Index(x, y)];
		}

		public Node Get(Coord coord) {
			return Get(coord.X, coord.Y);
		}

		public bool IsSolid(int x, int y) {
			var node = Get(x, y);
			return node != null ? node.Solid : true;
		}

		// https://stackoverflow.com/a/3706260
		public Node GetOpenNearby(int x, int y) {
			var vx = 1;
			var vy = 0;
			var len = 1;
			var ox = 0;
			var oy = 0;
			var p = 0;
			for (var _ = 0; _ < 64; _++) {

				var node = Get(x + ox, y + oy);
				if (node != null && !node.Solid) return node;

				ox += vx;
				oy += vy;
				p += 1;
				if (p >= len) {
					p = 0;
					var f = vx;
					vx = -vy;
					vy = f;
					if (vy == 0) {
						len += 1;
					}
				}
			}
			return null;
		}

		public int Index(int x, int y) {
			return y * Width + x;
		}

		public int Index(Coord coord) {
			return Index(coord.X, coord.Y);
		}

		public int Index(Node node) {
			return Index(node.Position.Coord);
		}

	}

}
