using System;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class CharacterListener : IDisposable {
		readonly Server Server;
		readonly IDisposable Listener;

		public CharacterListener(Context context) {
			Server = context.Server;
			Listener = context.World.SubscribeComponentChanged<Character>(OnChange);
		}

		public void Dispose() {
			Listener.Dispose();
		}

		void OnChange(in Entity entity, in Character oldCharacter, in Character newCharacter) {
			if (!entity.Has<Player>()) return;
			ref var player = ref entity.Get<Player>();

			var oldCommand = oldCharacter.GetCurrentCommand();
			var newCommand = newCharacter.GetCurrentCommand();
			if (oldCommand?.Id == newCommand?.Id) return;

			var characterId = entity.Get<CharacterId>().Id;

			var target = newCommand?.Target;
			if (target == null) {
				Server.SendToPlayer(player, new TargetEmptyPacket { CharacterId = characterId });
				return;
			}

			var other = target?.Entity;
			if (other != null) {
				var targetCharacterId = other.Value.Get<CharacterId>().Id;
				Server.SendToPlayer(player, new TargetMobilePacket {
					CharacterId = characterId,
					TargetCharacterId = targetCharacterId
				});

			} else {
				var coord = target.Value.Coord.Value;
				Server.SendToPlayer(player, new TargetFixedPacket {
					CharacterId = characterId,
					X = coord.X,
					Y = coord.Y
				});
			}
		}
		
	}

}
