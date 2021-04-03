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
		readonly TiledMap TiledMap;
		readonly Grid Grid;
		readonly EntityMap<Position> Positions;
		readonly SpriteBatch Batch;
		readonly SpriteFont Font;
		readonly OrthographicCamera Camera;
		Node Start;
		Node End;
		ImmutableStack<Node> Path;
		Dictionary<int, float> Heuristics;
		Dictionary<int, Node> Origins;
		Dictionary<int, int> Costs;

		public bool IsEnabled { get; set; }

		public PathfinderDebugSystem(
			TiledMap tiledMap,
			Grid grid,
			EntityMap<Position> positions,
			SpriteBatch batch,
			SpriteFont font,
			OrthographicCamera camera
		) {
			TiledMap = tiledMap;
			Grid = grid;
			Positions = positions;
			Batch = batch;
			Font = font;
			Camera = camera;
			IsEnabled = true;

			Start = Grid.Get(14, 14);
			End = Grid.Get(11, 9);
			Path = Pathfinder.OptimalPathfind(Grid, Positions, Start, End, Debug);
		}

		public void Update(float dt) {
			var mouse = Mouse.GetState();
			var dirty = false;
			if (mouse.LeftButton == ButtonState.Pressed) {
				Start = Tiled.GetNode(TiledMap, Grid, Camera, mouse.X, mouse.Y);
				dirty = true;
			}
			if (mouse.RightButton == ButtonState.Pressed) {
				End = Tiled.GetNode(TiledMap, Grid, Camera, mouse.X, mouse.Y);
				dirty = true;
			}
			if (Start == null || End == null) return;

			if (dirty) {
				Path = Pathfinder.OptimalPathfind(Grid, Positions, Start, End, Debug);
			}

			Batch.Begin(transformMatrix: Camera.GetViewMatrix());

			foreach (var (index, cost) in Costs) {
				var node = Grid.Nodes[index];
				var p = Tiled.CoordToVector(TiledMap, node.Coord);
				Batch.DrawString(Font, cost.ToString(), p, Color.White);
			}

			var h = Tiled.Half(TiledMap);
			foreach (var (index, heuristic) in Heuristics) {
				var node = Grid.Nodes[index];
				var p = Tiled.CoordToVector(TiledMap, node.Coord);
				Batch.DrawString(Font, heuristic.ToString(), p + h, Color.Yellow);
			}

			Batch.DrawString(Font, Start.ToString(), Vector2.Zero, Color.Lime);
			Batch.DrawString(Font, End.ToString(), new Vector2(0, h.Y), Color.Lime);

			DrawPath(Batch, TiledMap, Path);

			Batch.End();
		}

		void Debug(
			Dictionary<int, float> heuristics,
			Dictionary<int, Node> origins,
			Dictionary<int, int> costs
		) {
			Heuristics = heuristics;
			Origins = origins;
			Costs = costs;
		}

		public static void DrawPath(SpriteBatch batch, TiledMap map, ImmutableStack<Node> path) {
			var h = Tiled.Half(map);
			Node a = null;
			foreach (var node in path) {
				if (a == null) {
					a = node;
					continue;
				}
				var b = node;
				batch.DrawLine(
					Tiled.CoordToVector(map, a.Coord) + h,
					Tiled.CoordToVector(map, b.Coord) + h,
					Color.Blue
				);
				a = b;
			}
		}

		public void Dispose() {}

	}

}
