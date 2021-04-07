using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class ServerPositionSystem : ISystem<float> {
		readonly World World;
		readonly Server Server;
		readonly EntitySet Added;
		readonly EntitySet Changed;
		readonly EntitySet Removed;

		public bool IsEnabled { get; set; }

		public ServerPositionSystem(World world, Server server) {
			World = world;
			Server = server;
			Added = world.GetEntities().With<Character>().WhenAdded<Position>().AsSet();
			Changed = world.GetEntities().With<Character>().WhenChanged<Position>().AsSet();
			Removed = world.GetEntities().With<Character>().WhenRemoved<Position>().AsSet();
		}

		public void Update(float dt) {
			foreach (var entity in Added.GetEntities()) {
				ref var character = ref entity.Get<Character>();
				ref var position = ref entity.Get<Position>();

				var peerId = -1;
				if (entity.Has<Player>()) {
					peerId = entity.Get<Player>().PeerId;
				}

				Server.SendToAll(new AddCharacterPacket {
					Id = character.Id,
					PeerId = peerId,
					X = position.Coord.X,
					Y = position.Coord.Y
				});
			}
			Added.Complete();

			foreach (var entity in Changed.GetEntities()) {
				ref var character = ref entity.Get<Character>();
				ref var position = ref entity.Get<Position>();
				Server.SendToAll(new MoveCharacterPacket {
					Id = character.Id,
					X = position.Coord.X,
					Y = position.Coord.Y
				});
			}
			Changed.Complete();

			foreach (var entity in Removed.GetEntities()) {
				ref var character = ref entity.Get<Character>();
				Server.SendToAll(new RemoveCharacterPacket {
					Id = character.Id
				});
			}
			Removed.Complete();
		}

		public void Dispose() {
			Added.Dispose();
			Changed.Dispose();
			Removed.Dispose();
		}

	}

}
