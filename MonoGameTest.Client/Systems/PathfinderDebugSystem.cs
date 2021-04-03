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
using Priority_Queue;

namespace MonoGameTest.Client {

	public class PathfinderDebugSystem : ISystem<float> {
		readonly TiledMap TiledMap;
		readonly Grid Grid;
		readonly EntityMap<Position> Positions;
		readonly SpriteBatch Batch;
		readonly OrthographicCamera Camera;
		Node Start;
		Node End;
		ImmutableStack<Node> Path;
		SimplePriorityQueue<Node> Frontier;
		Dictionary<int, Node> Origins;
		Dictionary<int, int> Costs;

		public bool IsEnabled { get; set; }

		public PathfinderDebugSystem(
			TiledMap tiledMap,
			Grid grid,
			EntityMap<Position> positions,
			SpriteBatch batch,
			OrthographicCamera camera
		) {
			TiledMap = tiledMap;
			Grid = grid;
			Positions = positions;
			Batch = batch;
			Camera = camera;
			IsEnabled = true;
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

			DrawPath(Batch, TiledMap, Path);

			Batch.End();
		}

		void Debug(
			SimplePriorityQueue<Node> frontier,
			Dictionary<int, Node> origins,
			Dictionary<int, int> costs
		) {
			Frontier = frontier;
			Origins = origins;
			Costs = costs;
		}

		public static void DrawPath(SpriteBatch batch, TiledMap map, ImmutableStack<Node> path) {
			var h = new Vector2(map.TileWidth, map.TileHeight) / 2;
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
