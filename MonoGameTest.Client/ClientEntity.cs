using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public static class ClientEntity {

		public static Entity Create(Context context, AddCharacterPacket packet) {
			var sprite = Sprite.Create(context.Resources.Characters, 4);
			var entity = context.World.CreateEntity();
			entity.Set(new Character(packet.Id));
			if (packet.PeerId >= 0) {
				entity.Set(new Player(packet.PeerId));
			}
			entity.Set(new Position { Coord = new Coord(packet.X, packet.Y) });
			entity.Set(sprite);
			if (packet.PeerId == context.Client.PeerId) {
				entity.Set(new LocalPlayer());
			}
			return entity;
		}

	}

}
