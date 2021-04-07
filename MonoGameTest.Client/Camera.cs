using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGameTest.Client {

	public class Camera {
		readonly TiledMap TiledMap;
		readonly OrthographicCamera Orthographic;

		public Camera(GameWindow window, GraphicsDevice graphicsDevice, TiledMap tiledMap) {
			TiledMap = tiledMap;
			var viewport = new BoxingViewportAdapter(
				window,
				graphicsDevice,
				tiledMap.WidthInPixels,
				tiledMap.HeightInPixels
			);
			Orthographic = new OrthographicCamera(viewport);
		}

		public void SetWindowSize(GraphicsDeviceManager graphics) {
			graphics.PreferredBackBufferWidth = TiledMap.WidthInPixels * View.SCALE;
			graphics.PreferredBackBufferHeight = TiledMap.HeightInPixels * View.SCALE;
			graphics.ApplyChanges();
		}

		public Vector2 ScreenToWorld(Vector2 screenPosition) {
			return Orthographic.ScreenToWorld(screenPosition);
		}

		public Vector2 ScreenToWorld(float x, float y) {
			return Orthographic.ScreenToWorld(x, y);
		}

		public Matrix GetMatrix() {
			return Orthographic.GetViewMatrix();
		}

	}

}
