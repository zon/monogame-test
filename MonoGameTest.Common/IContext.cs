using System;
using DefaultEcs;
using DefaultEcs.Command;

namespace MonoGameTest.Common {

	public interface IContext : IDisposable {
		Grid Grid { get; }
		World World { get; }
		EntityCommandRecorder Recorder { get; }
		EntityMap<Position> Positions { get; }
		EntityMap<CharacterId> CharacterIds { get; }
		EntityMultiMap<CharacterId> CharacterBuffs { get; }
		bool IsReady { get; }
	}

}
