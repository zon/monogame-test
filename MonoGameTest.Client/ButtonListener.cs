using System;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class ButtonListener : IDisposable {
		readonly Context Context;
		readonly EntitySet Buttons;
		readonly IDisposable Subscriber;

		public ButtonListener(Context context) {
			Context = context;
			Buttons = context.World.GetEntities().With<Button>().AsSet();
			Subscriber = Context.World.Subscribe<ButtonMessage>(OnMessage);
		}

		public void Dispose() {
			Subscriber.Dispose();
		}

		void OnMessage(in ButtonMessage message) {

		}

	}

}
 