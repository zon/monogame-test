using System;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class ButtonSystem : AComponentSystem<float, Button> {
		readonly Context Context;
		readonly Rectangle ShadeRect;

		public ButtonSystem(Context context) : base(context.World) {
			Context = context;

			var f = Context.Resources.SkillIcons.Tags["shade"].From;
			ShadeRect = Context.Resources.SkillIcons.Frames[f].ToRectangle();
		}

		protected override void Update(float dt, ref Button button) {
			var buttonResource = Context.Resources.Button;
			var iconsResource = Context.Resources.SkillIcons;
			var mouse = Context.Mouse;
			var left = MouseButton.Left;

			var playerEntity = Context.LocalPlayer;
			var player = playerEntity?.Get<LocalPlayer>();
			var character = playerEntity?.Get<Character>();

			var area = new Rectangle(
				new Point(buttonResource.Size.X * button.Index, View.HEIGHT - View.ENERGY_BAR_HEIGHT - View.SKILL_BAR_HEIGHT),
				buttonResource.Size
			);
			var isHovered = area.Contains(Context.UICameraView.ScreenToWorld(mouse.Position.ToVector2()));
			
			var offset = Vector2.Zero;
			Rectangle drawRect;
			var depth = 0.1f;
			if (isHovered && mouse.WasButtonJustDown(left)) {
				drawRect = buttonResource.Pressed;
				Context.World.Publish(new ButtonMessage { SkillId = button.Skill.Id });
				offset = new Vector2(0, 2);

			} else if (isHovered && mouse.IsButtonDown(left)) {
				drawRect = buttonResource.Down;
				offset = new Vector2(0, 1);
			
			} else if (button.Skill.Id == player?.SelectedSkill?.Id) {
				drawRect = buttonResource.Selected;
			
			} else if (isHovered) {
				drawRect = buttonResource.Hover;

			} else {
				drawRect = buttonResource.Idle;
				depth = 0;
			}
			
			var position = area.Location.ToVector2() + offset;
			Context.UIBatch.Draw(
				texture: buttonResource.Document.Texture,
				position: position,
				sourceRectangle: drawRect,
				color: Color.White,
				rotation: 0,
				origin: Vector2.Zero,
				scale: Vector2.One,
				effects: SpriteEffects.None,
				layerDepth: depth
			);
			
			var iconPosition = position + new Vector2(
				(buttonResource.Size.X - button.IconRect.Width) / 2,
				(buttonResource.Size.Y - button.IconRect.Height) / 2
			);
			Context.UIBatch.Draw(
				texture: iconsResource.Texture,
				position: iconPosition,
				sourceRectangle: button.IconRect,
				color: Color.White,
				rotation: 0,
				origin: Vector2.Zero,
				scale: Vector2.One,
				effects: SpriteEffects.None,
				layerDepth: depth
			);

			if (character == null) return;
			var cooldown = character.Value.GetCooldown(button.Skill.Id);
			if (cooldown <= 0) return;

			var p = cooldown / button.Skill.Cooldown;
			var h = Calc.Floor(buttonResource.Size.Y * p);
			var y = buttonResource.Size.Y - h;

			Context.UIBatch.Draw(
				texture: buttonResource.Document.Texture,
				position: position + new Vector2(0, y),
				sourceRectangle: new Rectangle(
					buttonResource.Cooldown.X,
					buttonResource.Cooldown.Y + y,
					buttonResource.Cooldown.Width,
					h
				),
				color: Color.White,
				rotation: 0,
				origin: Vector2.Zero,
				scale: Vector2.One,
				effects: SpriteEffects.None,
				layerDepth: depth
			);
		}

	}

}
