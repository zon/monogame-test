using System;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class PacketListener : IDisposable {
		readonly Context Context;
		readonly EntityMap<Character> Characters;

		public bool IsEnabled { get; set; }

		public PacketListener(Context context) {
			Context = context;
			Characters = context.World.GetEntities().AsMap<Character>();
			var processor = context.Client.Processor;
			processor.SubscribeReusable<AddCharacterPacket>(OnAddCharacter);
			processor.SubscribeReusable<MoveCharacterPacket>(OnMoveCharacter);
			processor.SubscribeReusable<RemoveCharacterPacket>(OnRemoveCharacter);
			processor.SubscribeReusable<AttackPacket>(OnAttack);
			processor.SubscribeReusable<HealthPacket>(OnHealth);
		}

		public void Dispose() {
			Characters.Dispose();
		}

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
			health.Amount = packet.Amount;
			hit.Start();
		}

		bool GetEntity(int characterId, out Entity entity) {
			var character = new Character(characterId);
			return Characters.TryGetEntity(character, out entity);
		}

	}

}
