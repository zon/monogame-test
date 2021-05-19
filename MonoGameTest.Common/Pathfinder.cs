using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
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
				isOpen: (n, e) => !n.Solid && !e.HasValue,
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
				isOpen: (n, e) => !n.Solid,
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
				isOpen: (n, e) => !n.Solid && !e.HasValue,
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

		public Result MoveWithinRange(Node start, Node goal, int range) {
			return Query(
				start,
				isOpen: (n, e) => !n.Solid && !e.HasValue,
				isGoal: n => {
					var a = ToCenter(n);
					var b = ToCenter(goal);
					var ds = Vector2.DistanceSquared(a, b);
					if (ds > range * range) return false;
					return Cast(a, b) == goal;
				},
				heuristic: n => {
					var a = ToCenter(n);
					var b = ToCenter(goal);
					return Vector2.DistanceSquared(a, b);
				}
			);
		}

		public Result MoveWithinRange(Coord start, Coord goal, int range) {
			var a = Grid.Get(start);
			var b = Grid.Get(goal);
			if (a == null || b == null) return Result.NotFound;
			return MoveWithinRange(a, b, range);
		}

		public Result MoveToAttack(Coord start, Coord goal, Attack attack) {
			var a = Grid.Get(start);
			var b = Grid.Get(goal);
			if (a == null || b == null) return Result.NotFound;
			if (attack.IsMelee) {
				return MoveAdjacent(start, goal);
			} else {
				return MoveWithinRange(start, goal, attack.Range);
			}
		}

		Vector2 ToCenter(Node node) {
			return new Vector2(node.X + 0.5f, node.Y + 0.5f);
		}

		Result Query(
			Node start,
			Func<Node, Nullable<Entity>, bool> isOpen,
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
				var neighbors = new Neighbors(x, y, Grid, Positions, isOpen);
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

		public Node Cast(Vector2 a, Vector2 b) {
			var v = b - a;
			var n = Vector2.Normalize(new Vector2(v.X, v.Y));
			var r = new Vector2(
				1 / Math.Abs(n.X),
				1 / Math.Abs(n.Y)
			);

			var tx = Calc.Floor(a.X);
			var ty = Calc.Floor(a.Y);
			var tsx = n.X < 0 ? -1 : 1;
			var tsy = n.Y < 0 ? -1 : 1;
			var gx = Calc.Floor(b.X);
			var gy = Calc.Floor(b.Y);

			var dx = 0f;
			var dy = 0f;
			if (n.X >= 0) {
				dx = 1 - (a.X % 1);
			} else {
				dx = a.X % 1;
			}
			if (n.Y >= 0) {
				dy = 1 - (a.Y % 1);
			} else {
				dy = a.Y % 1;
			}
			dx = dx * r.X;
			dy = dy * r.Y;

			var td = 0f;
			var m = Vector2.Distance(Vector2.Zero, v);
			var s = 0f;
			var tile = Grid.Get(tx, ty);
			var lx = a.X;
			var ly = a.Y;
			while (td < m) {
				s = s + 1;

				if (dx < dy) {
					td = td + dx;
					dy = dy - dx;
					dx = r.X;
					tx = tx + tsx;
				} else {
					td = td + dy;
					dx = dx - dy;
					dy = r.Y;
					ty = ty + tsy;
				}

				var px = a.X + n.X * td;
				var py = a.Y + n.Y * td;
				lx = px;
				ly = py;

				var last = tile;
				tile = Grid.Get(tx, ty);
				if (tile == null || tile.Solid) {
					return last;
				} else if (tile.X == gx && tile.Y == gy) {
					return tile;
				}
			}

			return tile;
		}

		public Node Cast(float ax, float ay, float bx, float by) {
			return Cast(new Vector2(ax, ay), new Vector2(bx, by));
		}

		public bool HasSight(Vector2 a, Vector2 b) {
			var node = Cast(a, b);
			if (node == null) return false;
			var bx = Calc.Floor(b.X);
			var by = Calc.Floor(b.Y);
			return node.Coord.X == bx && node.Coord.Y == by;
		}

		public bool HasSight(Coord a, Coord b) {
			return HasSight(new Vector2(a.X + 0.5f, a.Y + 0.5f), new Vector2(b.X + 0.5f, b.Y + 0.5f));
		}

		public bool HasSight(Position a, Position b) {
			return HasSight(a.Coord, b.Coord);
		}

		public bool HasEntity(Position p) {
			return Positions.ContainsKey(p);
		}

		public bool HasEntity(Coord c) {
			return HasEntity(new Position { Coord = c });
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
