using System.Collections.Generic;
using System.Collections.Immutable;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class PathfinderDebugSystem : ISystem<float> {
		readonly EntityMap<Position> Positions;
		readonly SpriteBatch Batch;
		readonly SpriteFont Font;
		readonly Context Context;
		Node Start;
		Node End;
		ImmutableStack<Node> Path;
		Dictionary<int, float> Heuristics;
		Dictionary<int, Node> Origins;
		Dictionary<int, float> Costs;

		public bool IsEnabled { get; set; }

		public PathfinderDebugSystem(
			EntityMap<Position> positions,
			SpriteBatch batch,
			SpriteFont font,
			Context context
		) {
			Positions = positions;
			Batch = batch;
			Font = font;
			Context = context;
			IsEnabled = true;

			Start = Context.Grid.Get(2, 9);
			End = Context.Grid.Get(11, 9);
			Path = Pathfinder.OptimalPathfind(Context.Grid, Positions, Start, End, Debug);
		}

		public void Update(float dt) {
			var mouse = Mouse.GetState();
			var dirty = false;
			if (mouse.LeftButton == ButtonState.Pressed) {
				Start = Context.GetNode(mouse.X, mouse.Y);
				dirty = true;
			}
			if (mouse.RightButton == ButtonState.Pressed) {
				End = Context.GetNode(mouse.X, mouse.Y);
				dirty = true;
			}
			if (Start == null || End == null) return;

			if (dirty) {
				Path = Pathfinder.OptimalPathfind(Context.Grid, Positions, Start, End, Debug);
			}

			Batch.Begin(transformMatrix: Context.Camera.GetMatrix());

			foreach (var (index, cost) in Costs) {
				var node = Context.Grid.Nodes[index];
				var p = Context.CoordToVector(node.Coord);
				Batch.DrawString(Font, $"{cost:F1}", p, Color.White);
			}

			var h = Context.Half();
			foreach (var (index, heuristic) in Heuristics) {
				var node = Context.Grid.Nodes[index];
				var p = Context.CoordToVector(node.Coord);
				Batch.DrawString(Font, $"{heuristic:F1}", p + h, Color.Yellow);
			}

			Batch.DrawString(Font, $"{Start}", Vector2.Zero, Color.Lime);
			Batch.DrawString(Font, $"{End}", new Vector2(0, h.Y), Color.Lime);

			DrawPath(Batch, Context, Path);

			Batch.End();
		}

		void Debug(
			Dictionary<int, float> heuristics,
			Dictionary<int, Node> origins,
			Dictionary<int, float> costs
		) {
			Heuristics = heuristics;
			Origins = origins;
			Costs = costs;
		}

		public static void DrawPath(SpriteBatch batch, Context context, ImmutableStack<Node> path) {
			var h = context.Half();
			Node a = null;
			foreach (var node in path) {
				if (a == null) {
					a = node;
					continue;
				}
				var b = node;
				batch.DrawLine(
					context.CoordToVector(a.Coord) + h,
					context.CoordToVector(b.Coord) + h,
					Color.Blue
				);
				a = b;
			}
		}

		public void Dispose() {}

	}

}
