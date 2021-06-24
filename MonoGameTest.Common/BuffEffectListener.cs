using System;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class BuffEffectServerListener : IDisposable {
		readonly IContext Context;
		readonly IDisposable AddedListener;
		readonly IDisposable RemoveListener;

		public BuffEffectServerListener(IContext context) {
			Context = context;
			var world = context.World;
			AddedListener = world.SubscribeComponentAdded<BuffEffect>(OnAdd);
			RemoveListener = world.SubscribeComponentRemoved<BuffEffect>(OnRemove);
		}

		public void Dispose() {
			AddedListener.Dispose();
			RemoveListener.Dispose();
		}

		void OnAdd(in Entity entity, in BuffEffect effect) {
			TotalAttributes(entity, effect);
		}

		void OnRemove(in Entity entity, in BuffEffect effect) {
			TotalAttributes(entity, effect);
		}

		void TotalAttributes(in Entity entity, in BuffEffect effect) {
			ref var buff = ref entity.Get<Buff>();
			if (buff.Attributes == null) return;

			var characterId = entity.Get<CharacterId>();
			Entity characterEntity;
			if (!Context.CharacterIds.TryGetEntity(characterId, out characterEntity)) return;

			ref var attributes = ref characterEntity.Get<Attributes>();
			attributes.Total(Context, characterId);
		}

	}

}
