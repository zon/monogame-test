using System;
using DefaultEcs;
using LiteNetLib;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class PacketListener : IDisposable {
		readonly Server Server;
		readonly Grid Grid;
		readonly EntityMap<Player> Players;
		readonly EntityMap<Position> Positions;

		public PacketListener(Context context) {
			Server = context.Server;
			Grid = context.Grid;
			var world = context.World;
			Positions = world.GetEntities().AsMap<Position>();
			Players = world.GetEntities().With<Position>().With<Movement>().AsMap<Player>();
			Server.Processor.SubscribeReusable<MoveCommand, NetPeer>(OnMoveCommand);
		}

		public void Dispose() {
			Server.Processor.RemoveSubscription<MoveCommand>();
			Players.Dispose();
			Positions.Dispose();
		}

		void OnMoveCommand(MoveCommand command, NetPeer peer) {
			Entity entity;
			if (!Players.TryGetEntity(new Player(peer.Id), out entity)) return;

			ref var position = ref entity.Get<Position>();
			ref var movement = ref entity.Get<Movement>();

			var start = Grid.Get(position.Coord);
			var goal = Grid.Get(command.X, command.Y);
			if (goal == null) return;

			movement.Goal = goal.Coord;
		}

	}

}
