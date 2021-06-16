using DefaultEcs;
using DefaultEcs.Command;

namespace MonoGameTest.Common {

	public struct BuffEffect {
		public Buff Buff;
		public float Timeout;

		public static EntityRecord CreateEntity(IContext context, CharacterId characterId, Buff buff) {
			var entity = context.Recorder.CreateEntity(context.World);
			entity.Set(characterId);
			entity.Set(BuffEffectId.Create());
			entity.Set(new BuffEffect { Buff = buff, Timeout = buff.Duration });
			return entity;
		}

	}

}
