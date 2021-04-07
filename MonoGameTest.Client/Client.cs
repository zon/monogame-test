using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class Client : INetEventListener {
		readonly NetManager Manager;

		NetPeer Peer;

		public readonly NetPacketProcessor Processor;

		public delegate void OnPeerConnected(NetPeer peer);
		public delegate void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo);
		public delegate void OnTilemap(TilemapPacket packet);
		public delegate void OnAddCharacter(AddCharacterPacket packet);
		public delegate void OnMoveCharacter(MoveCharacterPacket packet);
		public delegate void OnRemoveCharacter(RemoveCharacterPacket packet);
		public event OnPeerConnected PeerConnectedEvent;
		public event OnPeerDisconnected PeerDisconnectedEvent;
		public event OnTilemap TilemapEvent;
		public event OnAddCharacter AddCharacterEvent;
		public event OnMoveCharacter MoveCharacterEvent;
		public event OnRemoveCharacter RemoveCharacterEvent;

		public int PeerId {
			get {
				if (Peer != null) {
					return Peer.Id;
				} else {
					return 0;
				}
			}
		}

		public Client() {
			Manager = new NetManager(this);
			Processor = new NetPacketProcessor();
			Processor.SubscribeReusable<TilemapPacket>(p => TilemapEvent(p));
			Processor.SubscribeReusable<AddCharacterPacket>(p => AddCharacterEvent(p));
			Processor.SubscribeReusable<MoveCharacterPacket>(p => MoveCharacterEvent(p));
			Processor.SubscribeReusable<RemoveCharacterPacket>(p => RemoveCharacterEvent(p));
		}

		public NetPeer Connect() {
			Manager.Start();
			var peer = Manager.Connect(Config.HOST, Config.PORT, Config.CONNECTION_KEY);
			if (peer != null) Peer = peer;
			return Peer;
		}

		public void Poll() {
			Manager.PollEvents();
		}

		public void Send<T>(T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new() {
			Peer.Send(Processor.Write(packet), method);
		}

		public void OnConnectionRequest(ConnectionRequest request) {
			Console.WriteLine("Connect Request: {0}", request.RemoteEndPoint);
		}

		public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) {
			Console.WriteLine("Network Error: {0}, {1}", endPoint, socketError);
		}

		public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {
			Console.WriteLine("Latency Update: {0}, {1}", peer, latency);
		}

		public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod) {
			Processor.ReadAllPackets(reader);
		}

		public void OnNetworkReceiveUnconnected(
			IPEndPoint remoteEndPoint,
			NetPacketReader reader,
			UnconnectedMessageType messageType
		) {
			Console.WriteLine("Receive Unconnected: {0}, {1}", remoteEndPoint, messageType);
		}

		void INetEventListener.OnPeerConnected(NetPeer peer) {
			Console.WriteLine("Connected: {0}, {1}", peer.Id, peer.EndPoint);
			if (PeerConnectedEvent == null) return;
			PeerConnectedEvent(peer);
		}

		void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
			Console.WriteLine("Disconnected: {0}, {1}", peer.Id, peer.EndPoint);
			Peer = null;
			if (PeerDisconnectedEvent == null) return;
			PeerDisconnectedEvent(peer, disconnectInfo);
		}

	}

}
