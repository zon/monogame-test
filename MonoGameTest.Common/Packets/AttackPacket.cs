namespace MonoGameTest.Common {

	public class AttackPacket : ICharacterPacket {
		public int CharacterId { get; set; }
		public int TargetX { get; set; }
		public int TargetY { get; set; }
		public int Damage { get; set; }
		public float Duration { get; set; }
	}

}
