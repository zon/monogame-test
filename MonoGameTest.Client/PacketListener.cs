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
			processor.SubscribeReusable<SkillStartMobilePacket>(OnMobileSkill);
			processor.SubscribeReusable<SkillStartFixedPacket>(OnFixedSkill);
			processor.SubscribeReusable<SkillStartPacket>(OnSkill);
			processor.SubscribeReusable<HealthPacket>(OnHealth);
			processor.SubscribeReusable<TargetMobilePacket>(OnMobileTarget);
			processor.SubscribeReusable<TargetFixedPacket>(OnFixedTarget);
			processor.SubscribeReusable<TargetEmptyPacket>(OnEmptyTarget);
			processor.SubscribeReusable<ProjectilePacket>(OnProjectile);
			processor.SubscribeReusable<CooldownPacket>(OnCooldown);
		}

		public void Dispose() {}

		void OnAddCharacter(AddCharacterPacket packet) {
			Factory.CreateCharacter(Context, packet);
		}

		void OnMoveCharacter(MoveCharacterPacket packet) {
			Entity entity;
			if (!GetEntity(packet.OriginCharacterId, out entity)) return;

			var coord = new Coord(packet.X, packet.Y);

			ref var position = ref entity.Get<Position>();
			ref var movement = ref entity.Get<MovementAnimation>();

			movement.Start(position.Coord, coord, Movement.ACTION_DURATION);
			
			position.Coord = coord;
			entity.NotifyChanged<Position>();
		}

		void OnRemoveCharacter(RemoveCharacterPacket packet) {
			Entity entity;
			if (!GetEntity(packet.OriginCharacterId, out entity)) return;
			entity.Dispose();
		}

		void OnMobileSkill(SkillStartMobilePacket packet) {
			Entity origin;
			if (!GetEntity(packet.OriginCharacterId, out origin)) return;
			Entity target;
			if (!GetEntity(packet.TargetCharacterId, out target)) return;
			var skill = Skill.Get(packet.SkillId);
			ref var animation = ref origin.Get<SkillAnimation>();
			animation.Start(Context, origin, target, skill);
		}

		void OnFixedSkill(SkillStartFixedPacket packet) {
			Entity origin;
			if (!GetEntity(packet.OriginCharacterId, out origin)) return;
			var target = new Coord(packet.TargetX, packet.TargetY);
			var skill = Skill.Get(packet.SkillId);
			ref var animation = ref origin.Get<SkillAnimation>();
			animation.Start(Context, origin, target, skill);
		}

		void OnSkill(SkillStartPacket packet) {
			Console.WriteLine("Untargeted skill");
		}

		void OnHealth(HealthPacket packet) {
			Entity entity;
			if (!GetEntity(packet.OriginCharacterId, out entity)) return;
			ref var character = ref entity.Get<CharacterId>();
			ref var health = ref entity.Get<Health>();
			ref var hit = ref entity.Get<HitAnimation>();
			ref var bang = ref entity.Get<Bang>();
			health.Amount = packet.Amount;
			hit.Start(Context);
			bang.Start(packet.Delta);
		}

		void OnMobileTarget(TargetMobilePacket packet) {
			Entity entity;
			if (!GetEntity(packet.CharacterId, out entity)) return;
			ref var localPlayer = ref entity.Get<LocalPlayer>();
			Entity other;
			if (packet.TargetCharacterId != packet.CharacterId && GetEntity(packet.TargetCharacterId, out other)) {
				localPlayer.Target = new Target(other);
			} else {
				localPlayer.Target = null;
			}
		}

		void OnFixedTarget(TargetFixedPacket packet) {
			Entity entity;
			if (!GetEntity(packet.CharacterId, out entity)) return;
			ref var localPlayer = ref entity.Get<LocalPlayer>();
			localPlayer.Target = new Target(new Coord(packet.X, packet.Y));
		}

		void OnEmptyTarget(TargetEmptyPacket packet) {
			Entity entity;
			if (!GetEntity(packet.CharacterId, out entity)) return;
			ref var localPlayer = ref entity.Get<LocalPlayer>();
			localPlayer.Target = null;
		}

		void OnProjectile(ProjectilePacket packet) {
			Entity target;
			if (!GetEntity(packet.TargetCharacterId, out target)) return;

			var skill = Skill.Get(packet.SkillId);
			if (skill == null) return;

			var origin = new Coord(packet.OriginX, packet.OriginY);
			Factory.CreateProjectile(Context, origin, target, skill);
		}

		void OnCooldown(CooldownPacket cooldown) {
			if (!Context.LocalPlayer.HasValue) return;
			var entity = Context.LocalPlayer.Value;

			var skill = Skill.Get(cooldown.SkillId);
			if (skill == null) return;

			ref var character = ref entity.Get<Character>();
			character.StartCooldown(entity, skill);
		}

		bool GetEntity(int characterId, out Entity entity) {
			return Context.GetEntityByCharacterId(characterId, out entity);
		}

	}

}
