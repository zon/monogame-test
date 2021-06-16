using System;
using DefaultEcs;
using DefaultEcs.Command;

namespace MonoGameTest.Common {

	public interface IContext : IDisposable {
		Grid Grid { get; }
		World World { get; }
		EntityCommandRecorder Recorder { get; }
		EntityMap<Position> Positions { get; }
		bool IsReady { get; }
	}

}
