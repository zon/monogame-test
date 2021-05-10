using System;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class PositionListener : IDisposable {
		readonly Server Server;
		readonly IDisposable AddedListener;
		readonly IDisposable ChangedListener;
		readonly IDisposable RemoveListener;

		public PositionListener(Context context) {
			Server = context.Server;
			var world = context.World;
			AddedListener = world.SubscribeComponentAdded<Position>(OnAddPosition);
			ChangedListener = world.SubscribeComponentChanged<Position>(OnChangePosition);
			RemoveListener = world.SubscribeComponentRemoved<Position>(OnRemovePosition);
		}

		public void Dispose() {
			AddedListener.Dispose();
			ChangedListener.Dispose();
			RemoveListener.Dispose();
		}

		void OnAddPosition(in Entity entity, in Position position) {
			Server.SendToAll(new AddCharacterPacket(entity));
		}

		void OnChangePosition(in Entity entity, in Position oldPosition, in Position newPosition) {
			ref var character = ref entity.Get<CharacterId>();
			Server.SendToAll(new MoveCharacterPacket {
				OriginCharacterId = character.Id,
				X = newPosition.Coord.X,
				Y = newPosition.Coord.Y,
				Duration = Movement.ACTION_DURATION
			});
		}

		void OnRemovePosition(in Entity entity, in Position position) {
			ref var character = ref entity.Get<CharacterId>();
			Server.SendToAll(new RemoveCharacterPacket {
				OriginCharacterId = character.Id
			});
		}

	}

}
