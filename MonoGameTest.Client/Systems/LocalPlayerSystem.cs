using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Input;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class LocalPlayerSystem : AEntitySetSystem<float> {
		readonly Context Context;

		public LocalPlayerSystem(Context context) : base(context.World
			.GetEntities() 
			.With<LocalPlayer>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			var mouse = Mouse.GetState();
			if (
				mouse.LeftButton != ButtonState.Pressed &&
				mouse.RightButton != ButtonState.Pressed
			) return;

			var goal = Context.GetNode(mouse.X, mouse.Y);
			if (goal == null) return;

			Context.Client.Send(new MoveCommand { X = goal.X, Y = goal.Y });
		}

	}

}
