using System;
using System.Diagnostics;
using System.Threading;
using DefaultEcs;
using DefaultEcs.System;
using LiteNetLib;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class Game : IDisposable {
		public readonly string TileMapName;
		public readonly Context Context;
		public readonly Server Server;
		public readonly World World;
		PacketListener PacketListener;
		PositionListener PositionListener;
		HealthListener HealthListener;
		CharacterListener CharacterListener;
		ProjectileListener ProjectileListener;
		CooldownListener CooldownListener;
		ISystem<float> Systems;
		Stopwatch Timer;

		public const int SLEEP = 15;

		public bool IsActive { get; private set; }

		public Game(string tileMapName) {
			TileMapName = tileMapName;

			Server = new Server();
			Server.PeerConnectedEvent += OnPeerConnected;
			Server.PeerDisconnectedEvent += OnPeerDisconnected;

			World = new World();

			Context = new Context(Server, World);
			Context.Load(TileMapName);

			PacketListener = new PacketListener(Context);
			PositionListener = new PositionListener(Context);
			HealthListener = new HealthListener(Context);
			CharacterListener = new CharacterListener(Context);
			ProjectileListener = new ProjectileListener(Context);
			CooldownListener = new CooldownListener(Context);

			Factory.SpawnMobs(Context.Grid, World);

			Systems = new SequentialSystem<float>(
				new BuffEffectSystem(Context),
				new CharacterSystem(Context),
				new ProjectileSystem(Context),
				new DeathSystem(Context),
				new MobTargetSystem(Context),
				new MovementSystem(Context)
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
				Update((float) dt);
				Thread.Sleep(SLEEP);
			}
		}

		protected void Update(float dt) {
			Systems.Update(dt);
			Context.Recorder.Execute();
			Server.Poll();
		}

		public void Exit() {
			IsActive = false; 
		}

		public void Dispose() {
			Exit();
			Server.Stop();
			PacketListener.Dispose();
			PositionListener.Dispose();
			CharacterListener.Dispose();
			ProjectileListener.Dispose();
			CooldownListener.Dispose();
			Systems.Dispose();
			Context.Dispose();
			World.Dispose();
		}

		void OnPeerConnected(NetPeer peer) {
			Session session;
			if (!Server.GetSessionByPeerId(peer.Id, out session)) return;

			Server.Send(peer, new SessionPacket { TileMapName = TileMapName, Id = session.Id });

			foreach (var entity in Context.Characters.GetEntities()) {
				Server.Send(peer, new AddCharacterPacket(entity));
			}

			Factory.SpawnPlayer(Context, session);
		}

		void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
			var player = new Player(peer.Id);
			Entity entity;
			if (!Context.PlayerIds.TryGetEntity(player, out entity)) return;
			entity.Dispose();
		}

	}

}
