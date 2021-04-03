using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace MonoGameTest.Client {

	public class TilemapDrawSystem : ISystem<float> {
		readonly TiledMapRenderer Renderer;
		readonly OrthographicCamera Camera;

		public bool IsEnabled { get; set; }

		public TilemapDrawSystem(
			GraphicsDevice graphicsDevice,
			TiledMap tiledMap,
			OrthographicCamera camera
		) {
			Renderer = new TiledMapRenderer(graphicsDevice, tiledMap);
			Camera = camera;
			IsEnabled = true;
		}

		public void Update(float state) {
			Renderer.Draw(Camera.GetViewMatrix());
		}

		public void Dispose() {
			Renderer.Dispose();
		}

	}

}
