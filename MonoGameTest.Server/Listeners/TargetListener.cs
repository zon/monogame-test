using System;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class TargetListener : IDisposable {
		readonly Server Server;
		readonly IDisposable Listener;

		public TargetListener(Context context) {
			Server = context.Server;
			Listener = context.World.SubscribeComponentChanged<Target>(OnChange);
		}

		public void Dispose() {
			Listener.Dispose();
		}

		void OnChange(in Entity entity, in Target oldTarget, in Target newTarget) {
			if (oldTarget.Entity == newTarget.Entity) return;
			ref var character = ref entity.Get<CharacterId>();
			var packet = new TargetPacket { CharacterId = character.Id };
			if (newTarget.HasEntity) {
				ref var other = ref newTarget.Entity.Value.Get<CharacterId>();
				packet.TargetId = other.Id;
			} else {
				packet.TargetId = character.Id;
			}
			Server.SendToAll(packet);
		}
		
	}

}
