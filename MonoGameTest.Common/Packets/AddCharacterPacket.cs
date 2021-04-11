using DefaultEcs;

namespace MonoGameTest.Common {

	public class AddCharacterPacket {
		public int Id { get; set; }
		public int Group { get; set; }
		public int Sprite { get; set; }
		public int PeerId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public AddCharacterPacket() {}

		public AddCharacterPacket(Entity entity) {
			ref var character = ref entity.Get<Character>();
			ref var group = ref entity.Get<Group>();
			ref var attributes = ref entity.Get<Attributes>();
			var peerId = 0;
			if (entity.Has<Player>()) {
				peerId = entity.Get<Player>().PeerId;
			}
			ref var position = ref entity.Get<Position>();
			Id = character.Id;
			Group = (int) group;
			Sprite = attributes.Sprite;
			PeerId = peerId;
			X = position.Coord.X;
			Y = position.Coord.Y;
		}

	}

}
