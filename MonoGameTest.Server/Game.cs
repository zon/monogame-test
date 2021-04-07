using System;
using System.Diagnostics;
using System.Threading;
using DefaultEcs;
using DefaultEcs.System;
using LiteNetLib;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class Game : IDisposable {
		string TileMapName;
		Context Context;
		Server Server;
		World World;
		EntitySet Characters;
		EntityMap<Player> Players;
		ISystem<float> Systems;
		Stopwatch Timer;

		const int SLEEP = 15;

		public bool IsActive { get; private set; }

		public Game() {
			TileMapName = "first";

			Server = new Server();
			Server.PeerConnectedEvent += OnPeerConnected;
			Server.PeerDisconnectedEvent += OnPeerDisconnected;

			World = new World();

			Context = new Context(World);
			Context.Load(TileMapName);

			Characters = World.GetEntities().With<Character>().AsSet();
			Players = World.GetEntities().With<Character>().AsMap<Player>();
			Systems = new SequentialSystem<float>(
				new CooldownSystem(World),
				new MovementSystem(World, Context),
				new ServerPositionSystem(World, Server)
			);
		}

		public void Run() {
			Server.Start();

			Timer = Stopwatch.StartNew();
			long previous = 0;

			IsActive = true;
			while(IsActive) {
				var elapsed = Timer.ElapsedMilliseconds;
				var dt = elapsed - previous;
				previous = elapsed;
				Systems.Update(dt);
				Server.Poll();
				Thread.Sleep(SLEEP);
			}
		}

		public void Exit() {
			IsActive = false;
		}

		public void Dispose() {
			Exit();
			Server.Stop();
			Players.Dispose();
			Systems.Dispose();
			World.Dispose();
		}

		void OnPeerConnected(NetPeer peer) {
			Server.Send(peer, new TilemapPacket { Name = TileMapName });

			foreach (var entity in Characters.GetEntities()) {
				ref var character = ref entity.Get<Character>();
				ref var position = ref entity.Get<Position>();
				Server.Send(peer, new AddCharacterPacket {
					Id = character.Id,
					PeerId = peer.Id,
					X = position.Coord.X,
					Y = position.Coord.Y
				});
			}

			var e = ServerEntity.CreatePlayer(World, peer.Id, new Coord(7, 7));
			var c = e.Get<Character>();
		}

		void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
			var player = new Player(peer.Id);
			Entity entity;
			if (!Players.TryGetEntity(player, out entity)) return;
			entity.Dispose();
		}

	}

}
