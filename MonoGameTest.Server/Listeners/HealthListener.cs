using System;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class HealthListener : IDisposable {
		readonly Server Server;
		readonly IDisposable Listener;

		public HealthListener(Context context) {
			Server = context.Server;
			Listener = context.World.SubscribeComponentChanged<Health>(OnChange);
		}

		public void Dispose() {
			Listener.Dispose();
		}

		void OnChange(in Entity entity, in Health oldHealth, in Health newHealth) {
			ref var group = ref entity.Get<Group>();
			if (group != Group.Player) return;
			ref var character = ref entity.Get<Character>();
			Server.SendToAll(new HealthPacket { CharacterId = character.Id, Amount = newHealth.Amount });
		}
		
	}

}
