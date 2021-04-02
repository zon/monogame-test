using System;
using System.Collections;
using System.Collections.Generic;
using DefaultEcs;

namespace MonoGameTest.Common {

	public struct Neighbors : IEnumerator<Node> {
		int X;
		int Y;
		Grid Grid;
		EntityMap<Position> Characters;
		Func<Node, Nullable<Entity>, bool> Check;
		int Step;
		bool Left;
		bool Right;
		bool Up;
		bool Down;

		public Node Current { get; private set; }

		public Neighbors(
			int x,
			int y,
			Grid grid,
			EntityMap<Position> characters,
			Func<Node, Nullable<Entity>, bool> check
		) {
			X = x;
			Y = y;
			Grid = grid;
			Characters = characters;
			Check = check;
			Step = 0;
			Left = false;
			Right = false;
			Up = false;
			Down = false;
			Current = default;
		}
		
		public Neighbors GetEnumerator() => new Neighbors(X, Y, Grid, Characters, Check);

		public bool MoveNext() {
			while (Step < 8) {
				switch (Step++) {
					case 0:
						Left = CheckNode(X - 1, Y);
						if (Left) return true;
						break;
					case 1:
						Right = CheckNode(X + 1, Y);
						if (Right) return true;
						break;
					case 2:
						Up = CheckNode(X, Y - 1);
						if (Up) return true;
						break;
					case 3:
						Down = CheckNode(X, Y + 1);
						if (Down) return true;
						break;
					case 4:
						if (Left && Up && CheckNode(X - 1, X - 1)) return true;
						break;
					case 5:
						if (Right && Up && CheckNode(X + 1, X - 1)) return true;
						break;
					case 6:
						if (Right && Down && CheckNode(X + 1, X + 1)) return true;
						break;
					case 7:
						if (Left && Down && CheckNode(X - 1, X + 1)) return true;
						break;
				}
			}
			return false;
		}

		public void Reset() {
			Step = 0;
		}

		public void Dispose() {}

		bool CheckNode(int x, int y) {
			var node = Grid.Get(x, y);
			if (node == null) return false;
			Entity entity;
			var okay = false;
			if (Characters.TryGetEntity(node.Position, out entity)) {
				okay = Check(node, entity);
			} else {
				okay = Check(node, null);
			}
			if (okay) {
				Current = node;
			}
			return okay;
		}

		object IEnumerator.Current => Current;

	}

}
