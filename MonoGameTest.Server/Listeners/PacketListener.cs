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

			var goal = Grid.Get(command.X, command.Y);
			if (goal == null) return;

			// correct solid node targets
			if (goal.Solid) {
				var pathfinder = Context.CreatePathfinder();
				ref var position = ref entity.Get<Position>();
				var start = Grid.Get(position.Coord);
				goal = pathfinder.OptimalMoveTo(start, goal).Node;
			}
			if (goal == null) return;

			ref var character = ref entity.Get<Character>();
			character.EnqueueNext(Command.Targeting(goal.Coord));
			entity.NotifyChanged<Character>();
		}

		void OnTargetCommand(TargetCommand command, NetPeer peer) {
			Entity entity;
			if (!GetPlayerEntity(peer, out entity)) return;

			Entity other;
			if (!Context.Characters.TryGetEntity(new CharacterId(command.CharacterId), out other)) return;
			
			ref var character = ref entity.Get<Character>();
			character.EnqueueNext(Command.Targeting(other, character.Role.PrimarySkill));
			entity.NotifyChanged<Character>();
		}

		bool GetPlayerEntity(NetPeer peer, out Entity entity) {
			return Players.TryGetEntity(new Player(peer.Id), out entity);
		}

	}

}
