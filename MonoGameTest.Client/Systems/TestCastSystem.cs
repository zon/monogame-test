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

	public class TestCastSystem : ISystem<float> {
		readonly Context Context;
		readonly Pathfinder Pathfinder;
		Vector2 Start;
		Vector2 End;
		Node Result;

		public bool IsEnabled { get; set; }
		
		CameraView Camera => Context.WorldCameraView;
		EntityMap<Position> Positions => Context.Positions;
		SpriteBatch Batch => Context.WorldBatch;

		public TestCastSystem(
			SpriteBatch batch,
			SpriteFont font,
			Context context
		) {
			Context = context;
			Pathfinder = new Pathfinder(Context.Grid, Positions, true);
			IsEnabled = true;

			Start = new Vector2(2.5f, 9.5f);
			End = new Vector2(11.5f, 9.5f);
			Result = Pathfinder.Cast(Start, End);
		}

		public void Dispose() {}

		public void Update(float dt) {
			var scale = View.TILE;
			var mouse = Mouse.GetState();
			var dirty = false;
			if (mouse.LeftButton == ButtonState.Pressed) {
				Start = Camera.ScreenToWorld(mouse.X, mouse.Y) / scale;
				dirty = true;
			}
			if (mouse.RightButton == ButtonState.Pressed) {
				End = Camera.ScreenToWorld(mouse.X, mouse.Y) / scale;
				dirty = true;
			}

			if (dirty) {
				Result = Pathfinder.Cast(Start, End);
			}

			var font = Context.Resources.Font;
			Batch.Begin(
				transformMatrix: Context.WorldCameraView.GetMatrix(),
				samplerState: SamplerState.PointClamp
			);

			Batch.DrawLine(Start * scale, End * scale, Color.Green);

			Batch.DrawRectangle(new RectangleF(
				Result.X * scale,
				Result.Y * scale,
				1 * scale,
				1 * scale
			), Color.Red);

			Batch.End();
		}

	}

}
