namespace MonoGameTest.Common {

	public class HealthPacket : ICharacterPacket {
		public int OriginCharacterId { get; set; }
		public int Delta { get; set; }
		public int Amount { get; set; }
	}

}
