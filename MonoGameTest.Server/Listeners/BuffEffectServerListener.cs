using System;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class BuffEffectServerListener : IDisposable {
		readonly Server Server;
		readonly IDisposable AddedListener;
		readonly IDisposable RemoveListener;

		public BuffEffectServerListener(Context context) {
			Server = context.Server;
			var world = context.World;
			AddedListener = world.SubscribeComponentAdded<BuffEffect>(OnAdd);
			RemoveListener = world.SubscribeComponentRemoved<BuffEffect>(OnRemove);
		}

		public void Dispose() {
			AddedListener.Dispose();
			RemoveListener.Dispose();
		}

		void OnAdd(in Entity entity, in BuffEffect effect) {
			var buffEffectId = entity.Get<BuffEffectId>().Id;
			var characterId = entity.Get<CharacterId>().Id;
			Server.SendToAll(new BuffEffectAddPacket {
				BuffEffectId = buffEffectId,
				CharacterId = characterId,
				SkillId = effect.Buff.Skill.Id,
				Timeout = effect.Timeout
			});
		}

		void OnRemove(in Entity entity, in BuffEffect effect) {
			var buffEffectId = entity.Get<BuffEffectId>().Id;
			Server.SendToAll(new BuffEffectRemovePacket {
				BuffEffectId = buffEffectId
			});
		}

	}

}
