using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DefaultEcs;
using Priority_Queue;

namespace MonoGameTest.Common {

	public class Pathfinder {
		public readonly Grid Grid;
		public readonly EntityMap<Position> Positions;
		public readonly SimplePriorityQueue<Node, float> Frontier;
		public readonly Dictionary<int, Node> Origins;
		public readonly Dictionary<int, float> Costs;
		public readonly Dictionary<int, float> Heuristics;

		public const int LOOP_MAX = 1000;
 
		static Result Empty = new Result(null, ImmutableStack.Create<Node>());

		public Pathfinder(Grid grid, EntityMap<Position> positions, bool debug = false) {
			Grid = grid;
			Positions = positions;
			Frontier = new SimplePriorityQueue<Node, float>();
			Origins = new Dictionary<int, Node>();
			Costs = new Dictionary<int, float>();
			Heuristics = debug ? new Dictionary<int, float>() : null;
		}

		public ImmutableStack<Node> MoveTo(Node start, Node goal) {
			if (goal.Solid) return Empty.Path;
			return Query(
				start,
				(n, e) => !n.Solid && !e.HasValue,
				n => n.Coord == goal.Coord,
				n => Coord.ChebyshevDistance(n.Coord, goal.Coord)
			).Path;
		}
		
		public ImmutableStack<Node> MoveTo(Coord start, Coord goal) {
			var a = Grid.Get(start);
			var b = Grid.Get(goal);
			if (a == null || b == null) return Empty.Path;
			return MoveTo(a, b);
		}

		public Result MoveAdjacent(Node start, Node goal) {
			return Query(
				start,
				(n, e) => !n.Solid && !e.HasValue,
				n => Coord.ChebyshevDistance(n.Coord, goal.Coord) == 1,
				n => Coord.ChebyshevDistance(n.Coord, goal.Coord)
			);
		}
		
		public Result MoveAdjacent(Coord start, Coord goal) {
			var a = Grid.Get(start);
			var b = Grid.Get(goal);
			if (a == null || b == null) return Empty;
			return MoveAdjacent(a, b);
		}

		Result Query(
			Node start,
			Func<Node, Nullable<Entity>, bool> check,
			Func<Node, bool> isGoal,
			Func<Node, float> heuristic
		) {
			Frontier.Clear();
			Origins.Clear();
			Costs.Clear();
			Heuristics?.Clear();

			var cost = heuristic(start);
			Frontier.Enqueue(start, cost);
			Costs[Grid.Index(start)] = cost;

			Node current = null;
			for (var _ = 0; _ < LOOP_MAX; _++) {
				if (!Frontier.TryDequeue(out current)) {
					return Empty;
				}
				
				if (isGoal(current)) {
					return CreatePath(current);
				}

				var x = current.X;
				var y = current.Y;
				var neighbors = new Neighbors(x, y, Grid, Positions, check);
				var baseCost = Costs[Grid.Index(current)];
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
						var p = nextCost + heuristic(next);
						if (!Frontier.EnqueueWithoutDuplicates(next, p)) {
							Frontier.UpdatePriority(next, p);
						}
						Origins[i] = current;
						Costs[i] = nextCost;
						Heuristics?.Add(i, p);
					}
				}
			}

			return Empty;
		}

		Result CreatePath(Node last) {
			var path = new Stack<Node>();
			path.Push(last);
			var current = last;
			while (current != null) {
				if (Origins.TryGetValue(Grid.Index(current), out current)) {
					path.Push(current);
				}
			}
			path.Pop();
			return new Result(last, ImmutableStack.Create(path.Reverse().ToArray()));
		}

		public struct Result {
			public readonly Node Node;
			public readonly ImmutableStack<Node> Path;

			public bool IsEmpty => Path.IsEmpty;

			public Result(Node node, ImmutableStack<Node> path) {
				Node = node;
				Path = path;
			}

		}

	}

}
