using System.Collections.Generic;
using System.Collections.Immutable;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class PathfinderDebugSystem : ISystem<float> {
		readonly SpriteBatch Batch;
		readonly SpriteFont Font;
		readonly Context Context;
		readonly EntityMap<Position> Positions;
		readonly Pathfinder Pathfinder;
		Node Start;
		Node End;
		ImmutableStack<Node> Path;

		public bool IsEnabled { get; set; }

		public PathfinderDebugSystem(
			SpriteBatch batch,
			SpriteFont font,
			Context context
		) {
			Batch = batch;
			Font = font;
			Context = context;
			Positions = Context.World.GetEntities().With<Position>().AsMap<Position>();
			Pathfinder = new Pathfinder(Context.Grid, Positions, true);
			IsEnabled = true;

			Start = Context.Grid.Get(2, 9);
			End = Context.Grid.Get(11, 9);
			Path = Pathfinder.MoveTo(Start, End).Path;
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
				Path = Pathfinder.MoveTo(Start, End).Path;
			}

			Batch.Begin(transformMatrix: Context.Camera.GetMatrix());

			foreach (var (index, cost) in Pathfinder.Costs) {
				var node = Context.Grid.Nodes[index];
				var p = Context.CoordToVector(node.Coord);
				Batch.DrawString(Font, $"{cost:F1}", p, Color.White);
			}

			var h = Context.Half();
			foreach (var (index, heuristic) in Pathfinder.Heuristics) {
				var node = Context.Grid.Nodes[index];
				var p = Context.CoordToVector(node.Coord);
				Batch.DrawString(Font, $"{heuristic:F1}", p + h, Color.Yellow);
			}

			Batch.DrawString(Font, $"{Start}", Vector2.Zero, Color.Lime);
			Batch.DrawString(Font, $"{End}", new Vector2(0, h.Y), Color.Lime);

			DrawPath(Batch, Context, Path);

			Batch.End();
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

		public void Dispose() {
			Positions.Dispose();
		}

	}

}
