using DefaultEcs;

namespace MonoGameTest.Common {

	public interface IContext {
		Grid Grid { get; }
		World World { get; }
		bool IsReady { get; }
	}

}
