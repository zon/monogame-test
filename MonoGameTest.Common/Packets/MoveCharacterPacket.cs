namespace MonoGameTest.Common {

	public class MoveCharacterPacket : ICharacterPacket {
		public int OriginCharacterId { get; set; }
		public long X { get; set; }
		public long Y { get; set; }
		public float Duration { get; set; }
	}

}
