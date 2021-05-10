using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public static class ClientCharacter {

		public static Entity Create(Context context, AddCharacterPacket packet) {
			var entity = context.World.CreateEntity();
			entity.Set(new CharacterId(packet.Id));
			entity.Set((Group) packet.Group);
			if (packet.PeerId > 0) {
				entity.Set(new Player(packet.PeerId));
			}
			entity.Set(new Health { Maximum = packet.HealthMaximum, Amount = packet.HealthAmount });
			var coord = new Coord(packet.X, packet.Y);
			entity.Set(new Position { Coord = coord });
			var sprite = Sprite.Create(
				context.Resources.Characters,
				packet.Sprite,
				context.CoordToVector(coord)
			);
			sprite.Depth = Depths.Character;
			entity.Set(sprite);
			Entity other;
			if (
				packet.TargetId != packet.Id &&
				context.Characters.TryGetEntity(new CharacterId(packet.TargetId), out other)
			) {
				entity.Set(new Target { Entity = other });
			} else {
				entity.Set(new Target());
			}
			entity.Set(new MovementAnimation());
			entity.Set(new AttackAnimation(context.Resources.Attacks));
			entity.Set(new HitAnimation(context.Resources.Hits));
			entity.Set(Bang.Create());
			if (packet.PeerId == context.PeerId) {
				entity.Set(new LocalPlayer());
			}
			return entity;
		}

	}

}
