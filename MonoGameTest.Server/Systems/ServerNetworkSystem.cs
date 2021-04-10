using DefaultEcs;
using DefaultEcs.System;
using LiteNetLib;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class ServerNetworkSystem : ISystem<float> {
		readonly Context Context;
		readonly EntityMap<Player> Players;
		readonly EntityMap<Position> Positions;

		public bool IsEnabled { get; set; }

		public ServerNetworkSystem(Context context) {
			Context = context;
			Positions = Context.World.GetEntities().AsMap<Position>();
			Players = Context.World.GetEntities().With<Position>().With<Movement>().AsMap<Player>();
			Context.Server.Processor.SubscribeReusable<MoveCommand, NetPeer>(OnMoveCommand);
			IsEnabled = true;
		}

		public void Update(float dt) {}

		public void Dispose() {
			Context.Server.Processor.RemoveSubscription<MoveCommand>();
			Players.Dispose();
			Positions.Dispose();
		}

		void OnMoveCommand(MoveCommand command, NetPeer peer) {
			Entity entity;
			if (!Players.TryGetEntity(new Player(peer.Id), out entity)) return;

			ref var position = ref entity.Get<Position>();
			ref var movement = ref entity.Get<Movement>();

			var start = Context.Grid.Get(position.Coord);
			var goal = Context.Grid.Get(command.X, command.Y);
			if (goal == null) return;

			movement.Goal = goal.Coord;
		}

	}

}
