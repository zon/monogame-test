using System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class CooldownListener : IDisposable {
		readonly Context Context;
		readonly IDisposable Listener;

		public CooldownListener(Context context) {
			Context = context;
			Listener = Context.World.Subscribe<CooldownMessage>(OnCooldown);
		}

		public void Dispose() {
			Listener.Dispose();
		}

		void OnCooldown(in CooldownMessage cooldown) {
			ref var player = ref cooldown.Entity.Get<Player>();
			Context.Server.SendToPlayer(player, new CooldownPacket { SkillId = cooldown.SkillId });
		}

	}

}
