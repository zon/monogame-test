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
		PacketListener PacketListener;
		PositionListener PositionListener;
		HealthListener HealthListener;
		TargetListener TargetListener;
		ProjectileListener ProjectileListener;
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

			PacketListener = new PacketListener(Context);
			PositionListener = new PositionListener(Context);
			HealthListener = new HealthListener(Context);
			TargetListener = new TargetListener(Context);
			ProjectileListener = new ProjectileListener(Context);

			Factory.SpawnMobs(Context.Grid, World);

			Characters = World.GetEntities().With<CharacterId>().AsSet();
			Players = World.GetEntities().With<CharacterId>().AsMap<Player>();
			Systems = new SequentialSystem<float>(
				new CharacterSystem(Context),
				new ProjectileSystem(Context),
				new DeathSystem(Context),
				new MobTargetSystem(Context),
				new MovementSystem(Context),
				new SkillSystem(Context)
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
				Context.Recorder.Execute();
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
			PacketListener.Dispose();
			PositionListener.Dispose();
			TargetListener.Dispose();
			Players.Dispose();
			Systems.Dispose();
			World.Dispose();
		}

		void OnPeerConnected(NetPeer peer) {
			Server.Send(peer, new SessionPacket { TileMapName = TileMapName, PeerId = peer.Id });

			foreach (var entity in Characters.GetEntities()) {
				Server.Send(peer, new AddCharacterPacket(entity));
			}

			Factory.SpawnPlayer(Context, peer.Id);
		}

		void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
			var player = new Player(peer.Id);
			Entity entity;
			if (!Players.TryGetEntity(player, out entity)) return;
			entity.Dispose();
		}

	}

}
