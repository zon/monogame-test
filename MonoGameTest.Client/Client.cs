using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class Client : INetEventListener {
		readonly NetManager Manager;

		NetPeer Server;

		public readonly NetPacketProcessor Processor;

		public delegate void OnConnected();
		public delegate void OnDisconnected(DisconnectInfo disconnectInfo);
		public event OnConnected ConnectedEvent;
		public event OnDisconnected DisconnectedEvent;

		public int Latency { get; private set; }

		public Client() {
			Manager = new NetManager(this);
			Processor = new NetPacketProcessor();
		}

		public NetPeer Connect() {
			Manager.Start();
			var peer = Manager.Connect(Config.HOST, Config.PORT, Config.CONNECTION_KEY);
			if (peer != null) Server = peer;
			return Server;
		}

		public void Poll() {
			Manager.PollEvents();
		}

		public void Send<T>(T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new() {
			Server.Send(Processor.Write(packet), method);
		}

		public void OnConnectionRequest(ConnectionRequest request) {
			Console.WriteLine("Connect Request: {0}", request.RemoteEndPoint);
		}

		public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) {
			Console.WriteLine("Network Error: {0}, {1}", endPoint, socketError);
		}

		public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {
			Latency = latency;
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
			Console.WriteLine("Connected: {0}", peer.EndPoint);
			if (ConnectedEvent == null) return;
			ConnectedEvent();
		}

		void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
			Console.WriteLine("Disconnected: {0}, {1}", peer.EndPoint, disconnectInfo);
			Server = null;
			if (DisconnectedEvent == null) return;
			DisconnectedEvent(disconnectInfo);
		}

	}

}
