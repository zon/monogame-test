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
			var skillsResource = Context.Resources.Skills;
			var mouse = Context.Mouse;
			var left = MouseButton.Left;
			var area = new Rectangle(button.Position, buttonResource.Size);
			var isHovered = area.Contains(Context.Camera.ScreenToUI(mouse.Position));
			
			var offset = Vector2.Zero;
			Rectangle drawRect;
			var depth = 0.1f;
			if (isHovered && mouse.WasButtonJustDown(left)) {
				drawRect = buttonResource.Pressed;
				Context.World.Publish(new ButtonMessage { ButtonId = button.Id });
				offset = new Vector2(0, 2);

			} else if (isHovered && mouse.IsButtonDown(left)) {
				drawRect = buttonResource.Down;
				offset = new Vector2(0, 1);
			
			} else if (button.IsSelected) {
				drawRect = buttonResource.Selected;
			
			} else if (isHovered) {
				drawRect = buttonResource.Hover;

			} else {
				drawRect = buttonResource.Idle;
				depth = 0;
			}
			
			Context.UI.Draw(
				texture: buttonResource.Document.Texture,
				position: button.Position.ToVector2() + offset,
				sourceRectangle: drawRect,
				color: Color.White,
				rotation: 0,
				origin: Vector2.Zero,
				scale: Vector2.One,
				effects: SpriteEffects.None,
				layerDepth: depth
			);
			
			Context.UI.Draw(
				texture: Context.Resources.Skills.Texture,
				position: button.Position.ToVector2() + offset + new Vector2(
					(buttonResource.Size.X - skillsResource.Frames[0].Width) / 2,
					(buttonResource.Size.Y - skillsResource.Frames[0].Height) / 2
				),
				sourceRectangle: button.Skill,
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
