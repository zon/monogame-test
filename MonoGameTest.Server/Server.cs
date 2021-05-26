using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class Server : INetEventListener {
		public readonly NetPacketProcessor Processor;

		public delegate void OnPeerConnected(NetPeer peer);
		public delegate void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo);
		public event OnPeerConnected PeerConnectedEvent;
		public event OnPeerDisconnected PeerDisconnectedEvent;

		readonly Dictionary<int, Session> SessionsById = new Dictionary<int, Session>();
		readonly Dictionary<int, Session> SessionsByPeerId = new Dictionary<int, Session>();
		readonly NetManager Manager;

		public Server() {
			Processor = new NetPacketProcessor();
			Manager = new NetManager(this);
		}

		public bool Start() {
			var ok = Manager.Start(Config.PORT);
			Console.WriteLine("Listening: {0}", Manager.LocalPort);
			return ok;
		}

		public void Poll() {
			Manager.PollEvents();
		}

		public void Stop() {
			Manager.DisconnectAll();
			Manager.Stop();
		}

		public bool GetSessionById(int id, out Session session) {
			return SessionsById.TryGetValue(id, out session);
		}

		public bool GetSessionByPeerId(int peerId, out Session session) {
			return SessionsByPeerId.TryGetValue(peerId, out session);
		}

		public void Send<T>(
			NetPeer peer,
			T packet,
			DeliveryMethod method = DeliveryMethod.ReliableOrdered
		) where T : class, new() {
			peer.Send(Processor.Write(packet), method);
		}

		public void SendToAll<T>(
			T packet,
			DeliveryMethod method = DeliveryMethod.ReliableOrdered
		) where T : class, new() {
			Manager.SendToAll(Processor.Write(packet), method);
		}

		public void SendToPlayer<T>(
			Player player,
			T packet,
			DeliveryMethod method = DeliveryMethod.ReliableOrdered
		) where T : class, new() {
			Session session;
			if (!GetSessionById(player.SessionId, out session)) return;
			Send(session.Client, packet, method);
		}

		void INetEventListener.OnConnectionRequest(ConnectionRequest request) {
			request.AcceptIfKey(Config.CONNECTION_KEY);
		}

		void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError) {
			Console.WriteLine("Network Error: {0}, {1}", endPoint, socketError);
		}

		void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency) {}

		void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod) {
			Processor.ReadAllPackets(reader, peer);
		}

		void INetEventListener.OnNetworkReceiveUnconnected(
			IPEndPoint remoteEndPoint,
			NetPacketReader reader,
			UnconnectedMessageType messageType
		) {
			Console.WriteLine("Receive Unconnected: {0}, {1}", remoteEndPoint, messageType);
		}

		void INetEventListener.OnPeerConnected(NetPeer peer) {
			var session = new Session(peer);

			Console.WriteLine("Connected: {0}, {1}", session.Id, peer.EndPoint);

			SessionsById.Add(session.Id, session);
			SessionsByPeerId.Add(peer.Id, session);

			if (PeerConnectedEvent == null) return;
			PeerConnectedEvent(peer);
		}

		void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
			Console.WriteLine("Disconnected: {0}, {1}", peer.Id, peer.EndPoint);

			Session session;
			if (GetSessionByPeerId(peer.Id, out session)) {
				SessionsById.Remove(session.Id);
			}
			SessionsByPeerId.Remove(peer.Id);

			if (PeerDisconnectedEvent == null) return;
			PeerDisconnectedEvent(peer, disconnectInfo);
		}

	}

}
