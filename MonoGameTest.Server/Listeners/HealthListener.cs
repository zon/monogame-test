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
			ref var character = ref entity.Get<CharacterId>();
			Server.SendToAll(new HealthPacket {
				OriginCharacterId = character.Id,
				Delta = newHealth.Amount - oldHealth.Amount,
				Amount = newHealth.Amount
			});
		}
		
	}

}
