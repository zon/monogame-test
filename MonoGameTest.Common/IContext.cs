using DefaultEcs;

namespace MonoGameTest.Common {

	public interface IContext {
		Grid Grid { get; }
		World World { get; }
		EntityMap<Position> Positions { get; }
		bool IsReady { get; }
	}

}
