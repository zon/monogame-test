using DefaultEcs;

namespace MonoGameTest.Common {

	public class AddCharacterPacket {
		public int Id { get; set; }
		public int RoleId { get; set; }
		public int Group { get; set; }
		public int Sprite { get; set; }
		public int SessionId { get; set; }
		public int HealthMaximum { get; set; }
		public int HealthAmount { get; set; }
		public int EnergyMaximum { get; set; }
		public float EnergyAmount { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public AddCharacterPacket() {}

		public AddCharacterPacket(Entity entity) {
			ref var characterId = ref entity.Get<CharacterId>();
			Id = characterId.Id;

			ref var character = ref entity.Get<Character>();
			RoleId = character.Role.Id;

			ref var group = ref entity.Get<Group>();
			Group = (int) group;

			ref var attributes = ref entity.Get<Attributes>();
			Sprite = attributes.Sprite;

			var sessionId = 0;
			if (entity.Has<Player>()) {
				sessionId = entity.Get<Player>().SessionId;
			}
			SessionId = sessionId;

			ref var health = ref entity.Get<Health>();
			HealthMaximum = health.Maximum;
			HealthAmount = health.Amount;

			ref var energy = ref entity.Get<Energy>();
			EnergyMaximum = energy.Maximum;
			EnergyAmount = energy.Amount;
			
			ref var position = ref entity.Get<Position>();
			X = position.Coord.X;
			Y = position.Coord.Y;
		}

	}

}
