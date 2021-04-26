using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGameTest.Client {

	public class Camera {
		readonly GraphicsDevice GraphicsDevice;
		readonly BoxingViewportAdapter Viewport;
		readonly OrthographicCamera Orthographic;

		public Camera(GameWindow window, GraphicsDevice graphicsDevice) {
			GraphicsDevice = graphicsDevice;
			Viewport = new BoxingViewportAdapter(
				window,
				graphicsDevice,
				View.WIDTH,
				View.HEIGHT
			);
			Orthographic = new OrthographicCamera(Viewport);
		}

		public void SetWindowSize(GraphicsDeviceManager graphics) {
			graphics.PreferredBackBufferWidth = Viewport.VirtualWidth * View.SCALE;
			graphics.PreferredBackBufferHeight = Viewport.VirtualHeight * View.SCALE;
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
