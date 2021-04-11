using System;
using System.Collections;
using System.Collections.Generic;
using DefaultEcs;
using Priority_Queue;

namespace MonoGameTest.Common {

	public class Search : IEnumerator<Entity> {
		public readonly Grid Grid;
		public readonly EntityMap<Position> Positions;
		public readonly Node Start;
		public readonly Func<Entity, bool> Criteria;
		public readonly SimplePriorityQueue<Node, float> Frontier;
		public readonly Dictionary<int, float> Costs;

		public Entity Current { get; private set; }

		public Search(Grid grid, EntityMap<Position> positions, Node start, Func<Entity, bool> criteria) {
			Grid = grid;
			Positions = positions;
			Start = start;
			Criteria = criteria;
			Frontier = new SimplePriorityQueue<Node, float>();
			Costs = new Dictionary<int, float>();
			Reset();
		}

		public Search(
			Grid grid, EntityMap<Position> positions, Coord start, Func<Entity, bool> criteria
		) : this(
			grid, positions, grid.Get(start), criteria
		) {}

		public void Reset() {
			Frontier.Clear();
			Costs.Clear();
			Current = default;

			if (Start == null) return;

			Frontier.EnqueueWithoutDuplicates(Start, 0);
			Costs.Add(Grid.Index(Start), 0);
		}

		public bool MoveNext() {
			Node node;
			if (!Frontier.TryDequeue(out node)) return false;

			Entity entity;
			var hasEntity = Positions.TryGetEntity(node.Position, out entity);

			if (hasEntity && Criteria(entity)) {
				Current = entity;
				return true;
			}

			var x = node.X;
			var y = node.Y;
			var neighbors = new Neighbors(
				x, y, Grid, Positions,
				(n, e) => !n.Solid && (!e.HasValue || (n.X == x || n.Y == y))
			);
			var baseCost = Costs[Grid.Index(node)];
			foreach (var next in neighbors) {
				var i = Grid.Index(next);
				var dx = next.X - x;
				var dy = next.Y - y;
				var nextCost = baseCost + Movement.COST;
				if (dx != 0 && dy != 0) {
					nextCost = baseCost + Movement.DIAGONAL_COST;
				}
				var prevCost = 0f;
				var hasPrevCost = Costs.TryGetValue(i, out prevCost);
				if (!hasPrevCost || nextCost < prevCost) {
					Frontier.EnqueueWithoutDuplicates(next, nextCost);
					Costs.Add(i, nextCost);
				}
			}
			return true;
		}

		public void Dispose() {}

		object IEnumerator.Current => Current;
		
	}

}
