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
			Rectangle drawRect;
			foreach (var node in Context.Grid.Nodes) {
				if (
					node.Solid ||
					node.Coord == mouseCoord ||
					node.Position == position ||
					!skill.InRange(position, node.Position)
				) continue;
				
				if (pathfinder.HasSight(position, node.Position)) {
					drawRect = HasSight;
				} else {
					drawRect = InRange;
				}

				Batch.Draw(
					texture: Context.Resources.Highlight.Texture,
					position: Context.CoordToVector(node.Coord),
					sourceRectangle: drawRect,
					color: Color.White,
					rotation: 0,
					origin: Vector2.Zero,
					scale: Vector2.One,
					effects: SpriteEffects.None,
					layerDepth: 0
				);
			}

			if (skill.IsValidTarget(pathfinder, position.Coord, mouseCoord)) {
				drawRect = Valid;
			} else {
				drawRect = Invalid;
			}

			Batch.Draw(
				texture: Context.Resources.Highlight.Texture,
				position: Context.CoordToVector(mouseCoord),
				sourceRectangle: drawRect,
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
