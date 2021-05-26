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
			if (mouse.WasButtonJustDown(MouseButton.Right)) {
				localPlayer.SelectedSkill = null;
				return;
			}

			if (!mouse.WasButtonJustDown(MouseButton.Left)) return;
			
			var tap = Context.ScreenToNode(mouse);
			if (tap == null) return;

			Entity target;
			if (!Context.GetEntityByPosition(tap, out target)) return;

			var targetCharacterId = target.Get<CharacterId>().Id;
			Context.Client.Send(new SkillTargetMobileCommand { SkillId = skillId, TargetCharacterId = targetCharacterId });
			Context.Resources.MoveConfirmSound.Play();
			localPlayer.SelectedSkill = null;
		}

	}

}
