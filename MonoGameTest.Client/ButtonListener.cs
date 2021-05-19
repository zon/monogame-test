using System;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class ButtonListener : IDisposable {
		readonly Context Context;
		readonly IDisposable Subscriber;

		public ButtonListener(Context context) {
			Context = context;
			Subscriber = Context.World.Subscribe<ButtonMessage>(OnMessage);
		}

		public void Dispose() {
			Subscriber.Dispose();
		}

		void OnMessage(in ButtonMessage message) {
			if (!Context.LocalPlayer.HasValue) return;
			ref var localPlayer = ref Context.LocalPlayer.Value.Get<LocalPlayer>();
			localPlayer.SelectedSkill = Attack.Get(message.AttackId);
		}

	}

}
 