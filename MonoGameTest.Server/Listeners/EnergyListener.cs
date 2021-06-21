using System;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class EnergyListener : IDisposable {
		readonly Server Server;
		readonly IDisposable Listener;

		public EnergyListener(Context context) {
			Server = context.Server;
			Listener = context.World.SubscribeComponentChanged<Energy>(OnChange);
		}

		public void Dispose() {
			Listener.Dispose();
		}

		void OnChange(in Entity entity, in Energy oldEnergy, in Energy newEnergy) {
			if (!entity.Has<Player>()) return;
			ref var player = ref entity.Get<Player>();
			Server.SendToPlayer(player, new EnergyPacket {
				Delta = Calc.Floor(newEnergy.Amount - oldEnergy.Amount),
				Amount = newEnergy.Amount
			});
		}
		
	}

}
