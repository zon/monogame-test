using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class ClientNetworkSystem : ISystem<float> {
		readonly Context Context;
		readonly EntityMap<Character> Characters;

		public bool IsEnabled { get; set; }

		public ClientNetworkSystem(Context context) {
			Context = context;
			Characters = Context.World.GetEntities().With<Position>().AsMap<Character>();
			Context.Client.Processor.SubscribeReusable<AddCharacterPacket>(OnAddCharacter);
			Context.Client.Processor.SubscribeReusable<MoveCharacterPacket>(OnMoveCharacter);
			Context.Client.Processor.SubscribeReusable<RemoveCharacterPacket>(OnRemoveCharacter);
		}

		public void Update(float dt) {}

		public void Dispose() {
			Characters.Dispose();
		}

		void OnAddCharacter(AddCharacterPacket packet) {
			ClientCharacter.Create(Context, packet);
		}

		void OnMoveCharacter(MoveCharacterPacket packet) {
			Entity entity;
			var character = new Character(packet.Id);
			if (!Characters.TryGetEntity(character, out entity)) return;
			ref var position = ref entity.Get<Position>();
			position.Coord = new Coord(packet.X, packet.Y);
		}

		void OnRemoveCharacter(RemoveCharacterPacket packet) {
			Entity entity;
			var character = new Character(packet.Id);
			if (!Characters.TryGetEntity(character, out entity)) return;
			entity.Dispose();
		}

	}

}
