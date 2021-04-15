using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGameTest.Client {

	public class Camera {
		readonly GraphicsDevice GraphicsDevice;
		readonly TiledMap TiledMap;
		readonly OrthographicCamera Orthographic;

		public Camera(GameWindow window, GraphicsDevice graphicsDevice, TiledMap tiledMap) {
			GraphicsDevice = graphicsDevice;
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

		public float Depth(float worldY, float offset = 0) {
			var y = WorldToScreen(0, worldY + offset).Y;
			var height = GraphicsDevice.Viewport.Height;
			return MathHelper.Clamp(y / height, 0, 1);
		}

		public float Depth(Vector2 worldPosition, float offset = 0) => Depth(worldPosition.Y, offset);

		public Vector2 ScreenToWorld(Vector2 screenPosition) => Orthographic.ScreenToWorld(screenPosition);
		public Vector2 ScreenToWorld(float x, float y) => Orthographic.ScreenToWorld(x, y);
		public Vector2 WorldToScreen(Vector2 worldPosition) => Orthographic.WorldToScreen(worldPosition);
		public Vector2 WorldToScreen(float x, float y) => Orthographic.WorldToScreen(x, y);
		public Matrix GetMatrix() => Orthographic.GetViewMatrix();

	}

}
