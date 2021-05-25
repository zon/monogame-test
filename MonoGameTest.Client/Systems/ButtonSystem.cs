using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;

namespace MonoGameTest.Client {

	public class ButtonSystem : AComponentSystem<float, Button> {
		readonly Context Context;

		public ButtonSystem(Context context) : base(context.World) {
			Context = context;
		}

		protected override void Update(float dt, ref Button button) {
			var buttonResource = Context.Resources.Button;
			var iconsResource = Context.Resources.SkillIcons;
			var mouse = Context.Mouse;
			var left = MouseButton.Left;

			var area = new Rectangle(
				new Point(buttonResource.Size.X * button.Index, View.SCREEN_HEIGHT - View.SKILL_BAR_HEIGHT),
				buttonResource.Size
			);
			var isHovered = area.Contains(Context.Camera.ScreenToUI(mouse.Position));
			
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
			
			} else if (button.Skill.Id == Context.LocalPlayer?.Get<LocalPlayer>().SelectedSkill?.Id) {
				drawRect = buttonResource.Selected;
			
			} else if (isHovered) {
				drawRect = buttonResource.Hover;

			} else {
				drawRect = buttonResource.Idle;
				depth = 0;
			}
			
			var position = area.Location.ToVector2() + offset;
			Context.UI.Draw(
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
			
			drawRect = iconsResource.Frames[button.IconFrame].ToRectangle();
			Context.UI.Draw(
				texture: iconsResource.Texture,
				position: position + new Vector2(
					(buttonResource.Size.X - drawRect.Width) / 2,
					(buttonResource.Size.Y - drawRect.Height) / 2
				),
				sourceRectangle: drawRect,
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
