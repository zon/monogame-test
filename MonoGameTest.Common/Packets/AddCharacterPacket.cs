namespace MonoGameTest.Common {

	public class AddCharacterPacket {
		public int Id { get; set; }
		public int Group { get; set; }
		public int Sprite { get; set; }
		public int PeerId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public AddCharacterPacket() {}

		public AddCharacterPacket(
			Character character,
			Attributes attributes,
			Position position,
			int peerId = 0
		) {
			Id = character.Id;
			Group = (int) attributes.Group;
			Sprite = attributes.Sprite;
			PeerId = peerId;
			X = position.Coord.X;
			Y = position.Coord.Y;
		}

	}

}
