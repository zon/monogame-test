using System.Collections.Generic;
using System.Collections.Immutable;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class PathfinderDebugSystem : ISystem<float> {
		readonly SpriteBatch Batch;
		readonly Context Context;
		readonly EntityMap<Position> Positions;
		readonly Pathfinder Pathfinder;
		Node Start;
		Node End;
		Pathfinder.Result Result;

		public bool IsEnabled { get; set; }

		public PathfinderDebugSystem(
			SpriteBatch batch,
			SpriteFont font,
			Context context
		) {
			Batch = batch;
			Context = context;
			Positions = Context.World.GetEntities().With<Character>().AsMap<Position>();
			Pathfinder = new Pathfinder(Context.Grid, Positions, true);
			IsEnabled = true;

			Start = Context.Grid.Get(2, 9);
			End = Context.Grid.Get(11, 9);
			Result = Pathfinder.MoveTo(Start, End);
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
				Result = Pathfinder.MoveTo(Start, End);
			}

			var font = Context.Resources.Font;
			Batch.Begin(
				transformMatrix: Context.Camera.GetMatrix(),
				samplerState: SamplerState.PointClamp
			);

			var h = Context.Half();
			foreach (var (index, note) in Pathfinder.Notes) {
				var node = Context.Grid.Nodes[index];
				var p = Context.CoordToVector(node.Coord);
				Batch.DrawString(font, $"{note.Cost:F1}", p, Color.White);
				Batch.DrawString(font, $"{note.Heuristic:F1}", p + h, Color.Yellow);
			}

			Batch.DrawString(font, $"{Start}", Vector2.Zero, Color.Lime);
			Batch.DrawString(font, $"{End} ${Result.IsGoal}", new Vector2(0, h.Y), Color.Lime);

			DrawPath(Batch, Context, Result.Path);

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
