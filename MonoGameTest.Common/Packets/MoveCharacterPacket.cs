namespace MonoGameTest.Common {

	public class MoveCharacterPacket : ICharacterPacket {
		public int CharacterId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public float Duration { get; set; }
	}

}
