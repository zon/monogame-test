namespace MonoGameTest.Common {

	public class AttackPacket : ICharacterPacket {
		public int AttackId { get; set; }
		public int OriginCharacterId { get; set; }
		public int TargetCharacterId { get; set; }
	}

}
