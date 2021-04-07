using DefaultEcs.System;
using MonoGame.Extended.Tiled.Renderers;

namespace MonoGameTest.Client {

	public class TilemapDrawSystem : ISystem<float> {
		readonly Context Context;

		TiledMapRenderer Renderer;

		public bool IsEnabled { get; set; }

		public TilemapDrawSystem(Context context) {
			Context = context;
			IsEnabled = true;
		}

		public void Update(float state) {
			if (!Context.IsReady) {
				if (Renderer != null) {
					Renderer.Dispose();
					Renderer = null;
				}
				return;
			}
			if (Renderer == null) {
				Renderer = new TiledMapRenderer(Context.GraphicsDevice, Context.TiledMap);
			}
			Renderer.Draw(Context.Camera.GetMatrix());
		}

		public void Dispose() {
			if (Renderer != null) Renderer.Dispose();
		}

	}

}
