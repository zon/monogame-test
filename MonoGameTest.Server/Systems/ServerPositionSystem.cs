using System;
using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class ServerPositionSystem : ISystem<float> {
		readonly World World;
		readonly Server Server;
		readonly IDisposable AddedListener;
		readonly IDisposable ChangedListener;
		readonly IDisposable RemoveListener;

		public bool IsEnabled { get; set; }

		public ServerPositionSystem(Context context) {
			World = context.World;
			Server = context.Server;
			AddedListener = World.SubscribeComponentAdded<Position>(OnAddPosition);
			ChangedListener = World.SubscribeComponentChanged<Position>(OnChangePosition);
			RemoveListener = World.SubscribeComponentRemoved<Position>(OnRemovePosition);
		}

		public void Update(float dt) {}

		public void Dispose() {
			AddedListener.Dispose();
			ChangedListener.Dispose();
			RemoveListener.Dispose();
		}

		void OnAddPosition(in Entity entity, in Position position) {
			ref var character = ref entity.Get<Character>();
			ref var attributes = ref entity.Get<Attributes>();

			var peerId = 0;
			if (entity.Has<Player>()) {
				peerId = entity.Get<Player>().PeerId;
			}

			Server.SendToAll(new AddCharacterPacket(character, attributes, position, peerId));
		}

		void OnChangePosition(in Entity entity, in Position oldPosition, in Position newPosition) {
			ref var character = ref entity.Get<Character>();
			Server.SendToAll(new MoveCharacterPacket {
				Id = character.Id,
				X = newPosition.Coord.X,
				Y = newPosition.Coord.Y
			});
		}

		void OnRemovePosition(in Entity entity, in Position position) {
			ref var character = ref entity.Get<Character>();
			Server.SendToAll(new RemoveCharacterPacket {
				Id = character.Id
			});
		}

	}

}
