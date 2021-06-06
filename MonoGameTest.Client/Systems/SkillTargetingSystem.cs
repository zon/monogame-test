using System;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class SkillTargetingSystem : AEntitySetSystem<float> {
		readonly Context Context;
		readonly Rectangle HasSight;
		readonly Rectangle InRange;
		readonly Rectangle Valid;
		readonly Rectangle Invalid;

		SpriteBatch Batch => Context.Foreground;

		public SkillTargetingSystem(Context context) : base(context.World
			.GetEntities() 
			.With<LocalPlayer>()
			.AsSet()
		) {
			Context = context;

			var highlight = Context.Resources.Highlight;
			HasSight = highlight.Frames[0].ToRectangle();
			InRange = highlight.Frames[1].ToRectangle();
			Valid = highlight.Frames[2].ToRectangle();
			Invalid = highlight.Frames[3].ToRectangle();
		}

		protected override void Update(float dt, in Entity entity) {
			ref var localPlayer = ref entity.Get<LocalPlayer>();
			if (localPlayer.SelectedSkill == null) return;

			var skill = localPlayer.SelectedSkill;
			var mouseCoord = Context.ScreenToCoord(new Vector2(Context.Mouse.X, Context.Mouse.Y) / View.SCALE);
			ref var position = ref entity.Get<Position>();
			var pathfinder = Context.CreatePathfinder();
			Rectangle sourceRect;
			foreach (var node in Context.Grid.Nodes) {
				if (
					node.Solid ||
					node.Coord == mouseCoord ||
					node.Position == position ||
					!skill.InRange(position, node.Position)
				) continue;
				
				if (pathfinder.HasSight(position, node.Position)) {
					sourceRect = HasSight;
				} else {
					sourceRect = InRange;
				}

				Batch.Draw(
					texture: Context.Resources.Highlight.Texture,
					position: Context.CoordToVector(node.Coord),
					sourceRectangle: sourceRect,
					color: Color.White,
					rotation: 0,
					origin: Vector2.Zero,
					scale: Vector2.One,
					effects: SpriteEffects.None,
					layerDepth: 0
				);
			}

			if (skill.IsValidTarget(pathfinder, position.Coord, mouseCoord)) {
				sourceRect = Valid;
			} else {
				sourceRect = Invalid;
			}

			if (skill.HasAreaEffect) {
				var area = new RadiusArea(mouseCoord, skill.Area);
				foreach (var coord in area) {
					var node = Context.Grid.Get(coord);
					if (node == null || node.Solid) continue;
					Batch.Draw(
						texture: Context.Resources.Highlight.Texture,
						position: Context.CoordToVector(coord),
						sourceRectangle: sourceRect,
						color: Color.White,
						rotation: 0,
						origin: Vector2.Zero,
						scale: Vector2.One,
						effects: SpriteEffects.None,
						layerDepth: 0.1f
					);
				}
				return;
			}

			Batch.Draw(
				texture: Context.Resources.Highlight.Texture,
				position: Context.CoordToVector(mouseCoord),
				sourceRectangle: sourceRect,
				color: Color.White,
				rotation: 0,
				origin: Vector2.Zero,
				scale: Vector2.One,
				effects: SpriteEffects.None,
				layerDepth: 0
			);
		}
		
	}

}
