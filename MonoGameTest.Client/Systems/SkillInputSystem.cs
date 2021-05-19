using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class SkillInputSystem : AEntitySetSystem<float> {
		readonly Context Context;

		public SkillInputSystem(Context context) : base(context.World
			.GetEntities() 
			.With<LocalPlayer>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var localPlayer = ref entity.Get<LocalPlayer>();
			if (localPlayer.SelectedSkill == null) return;
			var skillId = localPlayer.SelectedSkill.Id;

			var mouse = Context.Mouse;
			var leftDown = mouse.WasButtonJustDown(MouseButton.Left);
			var rightDown = mouse.WasButtonJustDown(MouseButton.Right);
			if (!leftDown && !rightDown) return;

			if (rightDown) {
				localPlayer.SelectedSkill = null;
				return;
			}
		}

	}

}
