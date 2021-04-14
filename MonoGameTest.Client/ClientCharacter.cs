using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public static class ClientCharacter {

		public static Entity Create(Context context, AddCharacterPacket packet) {
			var entity = context.World.CreateEntity();
			entity.Set(new Character(packet.Id));
			entity.Set((Group) packet.Group);
			if (packet.PeerId > 0) {
				entity.Set(new Player(packet.PeerId));
			}
			entity.Set(new Health { Maximum = packet.HealthMaximum, Amount = packet.HealthAmount });
			var coord = new Coord(packet.X, packet.Y);
			entity.Set(new Position { Coord = coord });
			entity.Set(Sprite.Create(
				context.Resources.Characters,
				packet.Sprite,
				context.CoordToVector(coord)
			));
			entity.Set(new MovementAnimation());
			entity.Set(new AttackAnimation(context.Resources.Attacks));
			if (packet.PeerId == context.PeerId) {
				entity.Set(new LocalPlayer());
			}
			return entity;
		}

	}

}
