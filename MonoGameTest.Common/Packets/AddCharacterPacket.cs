using DefaultEcs;

namespace MonoGameTest.Common {

	public class AddCharacterPacket {
		public int Id { get; set; }
		public int Group { get; set; }
		public int Sprite { get; set; }
		public int PeerId { get; set; }
		public int HealthMaximum { get; set; }
		public int HealthAmount { get; set; }
		public int TargetId { get; set; }
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
			ref var health = ref entity.Get<Health>();
			HealthMaximum = health.Maximum;
			HealthAmount = health.Amount;
			ref var position = ref entity.Get<Position>();
			Id = character.Id;
			Group = (int) group;
			Sprite = attributes.Sprite;
			PeerId = peerId;
			ref var target = ref entity.Get<Target>();
			if (target.HasEntity) {
				ref var other = ref target.Entity.Value.Get<Character>();
				TargetId = other.Id;
			} else {
				TargetId = Id;
			}
			X = position.Coord.X;
			Y = position.Coord.Y;
		}

	}

}
