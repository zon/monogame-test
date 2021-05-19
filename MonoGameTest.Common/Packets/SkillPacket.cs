namespace MonoGameTest.Common {

	public class SkillPacket : ICharacterPacket {
		public int SkillId { get; set; }
		public int OriginCharacterId { get; set; }
		public int TargetCharacterId { get; set; }
	}

}
