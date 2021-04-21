using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class LocalInputSystem : AEntitySetSystem<float> {
		readonly Context Context;

		public LocalInputSystem(Context context) : base(context.World
			.GetEntities() 
			.With<LocalPlayer>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			var mouse = MouseExtended.GetState();
			if (
				!mouse.WasButtonJustDown(MouseButton.Left) &&
				!mouse.WasButtonJustDown(MouseButton.Right)
			) return;

			var goal = Context.GetNode(mouse.X, mouse.Y);
			if (goal == null) return;

			Context.Client.Send(new MoveCommand { X = goal.X, Y = goal.Y });

			Effect.CreateEntity(Context, "ping-small", goal.Coord);
		}

	}

}
