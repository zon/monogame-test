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

			Context = new Context(Server, World);
			Context.Load(TileMapName);

			ServerCharacter.SpawnMobs(Context.Grid, World);

			Characters = World.GetEntities().With<Character>().AsSet();
			Players = World.GetEntities().With<Character>().AsMap<Player>();
			Systems = new SequentialSystem<float>(
				new CooldownSystem(Context),
				new MobTargetSystem(Context),
				new MovementSystem(Context),
				new ServerPositionSystem(Context),
				new ServerNetworkSystem(Context)
			);
		}

		public void Run() {
			Server.Start();

			Timer = Stopwatch.StartNew();
			double previous = 0;

			IsActive = true;
			while(IsActive) {
				var elapsed = Timer.Elapsed.TotalSeconds;
				var dt = elapsed - previous;
				previous = elapsed;
				Systems.Update((float) dt);
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
			Server.Send(peer, new SessionPacket { TileMapName = TileMapName, PeerId = peer.Id });

			foreach (var entity in Characters.GetEntities()) {
				ref var character = ref entity.Get<Character>();
				ref var attributes = ref entity.Get<Attributes>();
				ref var position = ref entity.Get<Position>();
				Server.Send(peer, new AddCharacterPacket(character, attributes, position, peer.Id));
			}

			ServerCharacter.SpawnPlayer(Context, peer.Id);
		}

		void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
			var player = new Player(peer.Id);
			Entity entity;
			if (!Players.TryGetEntity(player, out entity)) return;
			entity.Dispose();
		}

	}

}
