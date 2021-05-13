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
			var mouse = Context.Mouse;
			var leftDown = mouse.WasButtonJustDown(MouseButton.Left);
			var rightDown = mouse.WasButtonJustDown(MouseButton.Right);
			if (!leftDown && !rightDown) return;

			var goal = Context.GetNode(mouse.X, mouse.Y);
			if (goal == null) return;

			if (rightDown) {
				Context.Client.Send(new MoveCommand { X = goal.X, Y = goal.Y });
				Effect.CreateEntity(Context, "ping-small", goal.Coord);
				Context.Resources.MoveConfirmSound.Play();
				return;
			}

			Entity other;
			if (leftDown && Context.GetEntityByPosition(goal, out other)) {
				var character = other.Get<CharacterId>();
				Context.Client.Send(new TargetCommand { CharacterId = character.Id });
				Context.Resources.MoveConfirmSound.Play();
			}
		}

	}

}
