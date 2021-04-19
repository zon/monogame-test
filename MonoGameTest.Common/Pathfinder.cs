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
		public readonly Dictionary<int, Note> Notes;

		public const int LOOP_MAX = 1000;

		public Pathfinder(Grid grid, EntityMap<Position> positions, bool debug = false) {
			Grid = grid;
			Positions = positions;
			Frontier = new SimplePriorityQueue<Node, float>();
			Notes = new Dictionary<int, Note>();
		}

		public Result MoveTo(Node start, Node goal) {
			return Query(
				start,
				check: (n, e) => !n.Solid && !e.HasValue,
				isGoal: n => n.Coord == goal.Coord,
				heuristic: n => Coord.ChebyshevDistance(n.Coord, goal.Coord)
			);
		}
		
		public Result MoveTo(Coord start, Coord goal) {
			var a = Grid.Get(start);
			var b = Grid.Get(goal);
			if (a == null || b == null) return Result.NotFound;
			return MoveTo(a, b);
		}

		public Result OptimalMoveTo(Node start, Node goal) {
			return Query(
				start,
				check: (n, e) => !n.Solid,
				isGoal: n => n.Coord == goal.Coord,
				heuristic: n => Coord.ChebyshevDistance(n.Coord, goal.Coord)
			);
		}
		
		public Result OptimalMoveTo(Coord start, Coord goal) {
			var a = Grid.Get(start);
			var b = Grid.Get(goal);
			if (a == null || b == null) return Result.NotFound;
			return OptimalMoveTo(a, b);
		}

		public Result MoveAdjacent(Node start, Node goal) {
			return Query(
				start,
				check: (n, e) => !n.Solid && !e.HasValue,
				isGoal: n => Coord.ChebyshevDistance(n.Coord, goal.Coord) == 1,
				heuristic: n => Coord.ChebyshevDistance(n.Coord, goal.Coord)
			);
		}
		
		public Result MoveAdjacent(Coord start, Coord goal) {
			var a = Grid.Get(start);
			var b = Grid.Get(goal);
			if (a == null || b == null) return Result.NotFound;
			return MoveAdjacent(a, b);
		}

		Result Query(
			Node start,
			Func<Node, Nullable<Entity>, bool> check,
			Func<Node, bool> isGoal,
			Func<Node, float> heuristic
		) {
			if (isGoal(start)) {
				return Result.Arrived(start);
			}

			Frontier.Clear();
			Notes.Clear();

			var h = heuristic(start);
			Frontier.Enqueue(start, h);
			var note = new Note {
				Cost = 0,
				Origin = null,
				Heuristic = h
			};
			Notes[Grid.Index(start)] = note;

			var best = start;
			var bestNote = note;

			Node current = null;
			for (var _ = 0; _ < LOOP_MAX; _++) {
				if (!Frontier.TryDequeue(out current)) {
					return CreatePath(best, false);
				}
				
				if (isGoal(current)) {
					return CreatePath(current, true);
				}

				note = Notes[Grid.Index(current)];
				h = heuristic(current);
				if (h < bestNote.Heuristic) {
					best = current;
					bestNote = note;
				}

				var x = current.X;
				var y = current.Y;
				var neighbors = new Neighbors(x, y, Grid, Positions, check);
				foreach (var next in neighbors) {
					var i = Grid.Index(next);
					var dx = next.X - x;
					var dy = next.Y - y;
					var nextCost = note.Cost + Movement.COST;
					if (dx != 0 && dy != 0) {
						nextCost = note.Cost + Movement.DIAGONAL_COST;
					}
					Note prevNote;
					var hasPrevCost = Notes.TryGetValue(i, out prevNote);
					if (!hasPrevCost || nextCost < prevNote.Cost) {
						h = heuristic(next);
						var p = nextCost + h;
						if (!Frontier.EnqueueWithoutDuplicates(next, p)) {
							Frontier.UpdatePriority(next, p);
						}
						Notes[i] = new Note {
							Origin = current,
							Cost = nextCost,
							Heuristic = h
						};
					}
				}
			}

			return CreatePath(best, false);
		}

		Result CreatePath(Node last, bool isGoal) {
			var path = new Stack<Node>();
			path.Push(last);
			var current = last;
			while (current != null) {
				Note note;
				if (Notes.TryGetValue(Grid.Index(current), out note)) {
					if (note.Origin != null) {
						path.Push(note.Origin);
					}
					current = note.Origin;
				}
			}
			path.Pop();
			return new Result(last, ImmutableStack.Create(path.Reverse().ToArray()), isGoal);
		}

		public struct Note {
			public Node Origin;
			public float Cost;
			public float Heuristic;
		}

		public struct Result {
			public readonly Node Node;
			public readonly ImmutableStack<Node> Path;
			public readonly bool IsGoal;

			public Result(Node node, ImmutableStack<Node> path, bool isGoal) {
				Node = node;
				Path = path;
				IsGoal = isGoal;
			}

			public static Result Arrived(Node node) {
				return new Result(node, NotFound.Path, true);
			}

			public static Result NotFound = new Result(null, ImmutableStack.Create<Node>(), false);

		}

	}

}
