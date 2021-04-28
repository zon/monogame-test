using System;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class PacketListener : IDisposable {
		readonly Context Context;

		public bool IsEnabled { get; set; }

		public PacketListener(Context context) {
			Context = context;
			var processor = context.Client.Processor;
			processor.SubscribeReusable<AddCharacterPacket>(OnAddCharacter);
			processor.SubscribeReusable<MoveCharacterPacket>(OnMoveCharacter);
			processor.SubscribeReusable<RemoveCharacterPacket>(OnRemoveCharacter);
			processor.SubscribeReusable<AttackPacket>(OnAttack);
			processor.SubscribeReusable<HealthPacket>(OnHealth);
			processor.SubscribeReusable<TargetPacket>(OnTarget);
		}

		public void Dispose() {}

		void OnAddCharacter(AddCharacterPacket packet) {
			ClientCharacter.Create(Context, packet);
		}

		void OnMoveCharacter(MoveCharacterPacket packet) {
			Entity entity;
			if (!GetEntity(packet.CharacterId, out entity)) return;

			var coord = new Coord(packet.X, packet.Y);

			ref var position = ref entity.Get<Position>();
			ref var movement = ref entity.Get<MovementAnimation>();

			movement.Start(position.Coord, coord, Movement.ACTION_DURATION);
			position.Coord = coord;
		}

		void OnRemoveCharacter(RemoveCharacterPacket packet) {
			Entity entity;
			if (!GetEntity(packet.CharacterId, out entity)) return;
			entity.Dispose();
		}

		void OnAttack(AttackPacket packet) {
			Entity entity;
			if (!GetEntity(packet.CharacterId, out entity)) return;	
			ref var attack = ref entity.Get<AttackAnimation>();
			var target = new Coord(packet.TargetX, packet.TargetY);
			attack.Start(Context, entity, target, "sword");
		}

		void OnHealth(HealthPacket packet) {
			Entity entity;
			if (!GetEntity(packet.CharacterId, out entity)) return;
			ref var character = ref entity.Get<Character>();
			ref var health = ref entity.Get<Health>();
			ref var hit = ref entity.Get<HitAnimation>();
			ref var bang = ref entity.Get<Bang>();
			health.Amount = packet.Amount;
			hit.Start();
			bang.Start(packet.Delta);
		}

		void OnTarget(TargetPacket packet) {
			Entity entity;
			if (!GetEntity(packet.CharacterId, out entity)) return;
			ref var target = ref entity.Get<Target>();
			Entity other;
			if (packet.TargetId != packet.CharacterId && GetEntity(packet.TargetId, out other)) {
				target.Entity = other;
			} else {
				target.Entity = null;
			}
		}

		bool GetEntity(int characterId, out Entity entity) {
			return Context.GetEntityByCharacterId(characterId, out entity);
		}

	}

}
