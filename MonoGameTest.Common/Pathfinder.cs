using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DefaultEcs;
using Priority_Queue;

namespace MonoGameTest.Common {

	public static class Pathfinder {
		
		public static ImmutableStack<Node> Empty = ImmutableStack.Create<Node>();

		public static ImmutableStack<Node> Pathfind(
			Grid grid,
			EntityMap<Position> characters,
			Node start,
			Node goal,
			Action<SimplePriorityQueue<Node>, Dictionary<int, Node>, Dictionary<int, int>> debug = null
		) {
			if (goal.Solid) return Empty;
			return Query(
				grid,
				characters,
				start,
				(n, e) => !n.Solid && e.HasValue,
				n => n.Coord == goal.Coord,
				n => Coord.ManhattanDistance(n.Coord, goal.Coord) * Movement.COST,
				debug
			);
		}

		public static ImmutableStack<Node> OptimalPathfind(
			Grid grid,
			EntityMap<Position> characters,
			Node start,
			Node goal,
			Action<SimplePriorityQueue<Node>, Dictionary<int, Node>, Dictionary<int, int>> debug = null
		) {
			if (goal.Solid) return Empty;
			return Query(
				grid,
				characters,
				start,
				(n, _) => !n.Solid,
				n => n.Coord == goal.Coord,
				n => Coord.ManhattanDistance(n.Coord, goal.Coord) * Movement.COST,
				debug
			);
		}
		
		public static ImmutableStack<Node> OptimalPathfind(
			Grid grid, EntityMap<Position> characters,
			Coord start,
			Coord goal,
			Action<SimplePriorityQueue<Node>, Dictionary<int, Node>, Dictionary<int, int>> debug = null
		) {
			var a = grid.Get(start);
			var b = grid.Get(goal);
			if (a == null || b == null) return Empty;
			return OptimalPathfind(grid, characters, a, b, debug);
		}

		static ImmutableStack<Node> Query(
			Grid grid,
			EntityMap<Position> characters,
			Node start,
			Func<Node, Nullable<Entity>, bool> check,
			Func<Node, bool> isGoal,
			Func<Node, float> heuristic,
			Action<SimplePriorityQueue<Node>, Dictionary<int, Node>, Dictionary<int, int>> debug = null
		) {
			var frontier = new SimplePriorityQueue<Node>();
			var origins = new Dictionary<int, Node>();
			var costs = new Dictionary<int, int>();

			frontier.Enqueue(start, heuristic(start));
			costs[grid.Index(start)] = 0;

			Node current = null;
			for (var _ = 0; _ < 1000; _++) {
				current = frontier.Dequeue();
				if (current == null) {
					return Empty;
				}
				
				if (isGoal(current)) {
					if (debug != null) {
						debug(frontier, origins, costs);
					}
					return CreatePath(grid, origins, current);
				}

				var x = current.X;
				var y = current.Y;
				var neighbors = new Neighbors(x, y, grid, characters, check);
				var baseCost = costs[grid.Index(current)];
				foreach (var next in neighbors) {
					var dx = next.X - x;
					var dy = next.Y - y;
					var nextCost = baseCost + Movement.COST;
					if (dx != 0 && dy != 0) {
						nextCost += Movement.EXTRA_DIAGONAL_COST;
					}
					var prevCost = 0;
					var hasPrevCost = costs.TryGetValue(grid.Index(next), out prevCost);
					if (!hasPrevCost || nextCost < prevCost) {
						var h = nextCost + heuristic(next);
						if (hasPrevCost) {
							frontier.UpdatePriority(next, h);
						} else {
							frontier.Enqueue(next, h);
						}
						origins[grid.Index(next)] = current;
						costs[grid.Index(next)] = nextCost;
					}
				}
			}

			return Empty;
		}

		static ImmutableStack<Node> CreatePath(Grid grid, Dictionary<int, Node> origins, Node last) {
			var path = new Stack<Node>();
			path.Push(last);
			var current = last;
			while (current != null) {
				if (origins.TryGetValue(grid.Index(current), out current)) {
					path.Push(current);
				}
			}
			path.Pop();
			return ImmutableStack.Create(path.Reverse().ToArray());
		}

	}

}
