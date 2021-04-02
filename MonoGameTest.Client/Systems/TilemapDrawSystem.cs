using DefaultEcs.System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace MonoGameTest.Client {

	public class TilemapDrawSystem : ISystem<float> {
		TiledMapRenderer renderer;

		public bool IsEnabled { get; set; }

		public TilemapDrawSystem(GraphicsDeviceManager manager, TiledMap tiledMap) {
			renderer = new TiledMapRenderer(manager.GraphicsDevice, tiledMap);
			IsEnabled = true;
		}

		public void Update(float state)
		{
			renderer.Draw();
		}

		public void Dispose() {
			renderer.Dispose();
		}

	}

}
