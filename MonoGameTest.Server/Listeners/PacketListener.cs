using System;
using DefaultEcs;
using LiteNetLib;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class PacketListener : IDisposable {
		readonly Context Context;
		readonly EntityMap<Player> Players;

		Server Server => Context.Server;
		Grid Grid => Context.Grid;

		public PacketListener(Context context) {
			Context = context;
			Players = Context.World.GetEntities().AsMap<Player>();
			Server.Processor.SubscribeReusable<MoveCommand, NetPeer>(OnMoveCommand);
			Server.Processor.SubscribeReusable<TargetCommand, NetPeer>(OnTargetCommand);
		}

		public void Dispose() {
			Server.Processor.RemoveSubscription<MoveCommand>();
			Players.Dispose();
		}

		void OnMoveCommand(MoveCommand command, NetPeer peer) {
			Entity entity;
			if (!GetPlayerEntity(peer, out entity)) return;

			ref var position = ref entity.Get<Position>();
			ref var movement = ref entity.Get<Movement>();

			var start = Grid.Get(position.Coord);
			var goal = Grid.Get(command.X, command.Y);
			if (goal == null) return;

			movement.Goal = goal.Coord;
		}

		void OnTargetCommand(TargetCommand command, NetPeer peer) {
			Entity entity;
			if (!GetPlayerEntity(peer, out entity)) return;

			Entity other;
			if (!Context.Characters.TryGetEntity(new CharacterId(command.CharacterId), out other)) return;

			ref var target = ref entity.Get<Target>();
			target.Entity = other != entity ? other : null;
			
			entity.NotifyChanged<Target>();
		}

		bool GetPlayerEntity(NetPeer peer, out Entity entity) {
			return Players.TryGetEntity(new Player(peer.Id), out entity);
		}

	}

}
