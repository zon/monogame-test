using DefaultEcs;
using LiteNetLib;

namespace MonoGameTest.Server {

	public class Session {
		public readonly int Id;
		public readonly NetPeer Client;
		public Entity Entity;

		static int AutoId;

		public Session(NetPeer client) {
			Id = ++AutoId;
			Client = client;
		}

	}

}
