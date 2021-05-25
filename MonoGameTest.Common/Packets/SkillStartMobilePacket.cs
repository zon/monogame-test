namespace MonoGameTest.Common {

	public class SkillStartMobilePacket : ICharacterPacket {
		public int SkillId { get; set; }
		public int OriginCharacterId { get; set; }
		public int TargetCharacterId { get; set; }
	}

}
