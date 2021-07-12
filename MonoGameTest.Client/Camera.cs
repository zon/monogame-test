using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGameTest.Client {

	public class Camera : IDisposable {
		public readonly ScalingViewportAdapter Viewport;
		public readonly RenderTarget2D RenderTarget;
		readonly GameWindow Window;
		readonly OrthographicCamera Orthographic;

		public float Zoom {
			get => Orthographic.Zoom;
			set { Orthographic.Zoom = value; }
		}

		public Camera(GameWindow window, GraphicsDevice graphicsDevice, int width, int height) {
			Window = window;
			Viewport = new ScalingViewportAdapter(graphicsDevice, width, height);
			Orthographic = new OrthographicCamera(Viewport);
			RenderTarget = new RenderTarget2D(
				graphicsDevice: graphicsDevice,
				width: Viewport.ViewportWidth,
				height: Viewport.ViewportHeight,
				mipMap: false,
				preferredFormat: graphicsDevice.PresentationParameters.BackBufferFormat,
				preferredDepthFormat: DepthFormat.Depth24
			);
		}

		public Camera(GameWindow window, GraphicsDevice graphicsDevice) : this(
			window,
			graphicsDevice,
			View.WIDTH,
			View.HEIGHT
		) {}

		public void Dispose() {
			Viewport.Dispose();
			RenderTarget.Dispose();
		}

		public void LookAt(Vector2 position) => Orthographic.LookAt(position);
		public void Move(Vector2 direction) => Orthographic.Move(direction);

		public float Depth(float worldY, float offset = 0) {
			var y = WorldToScreen(0, worldY + offset).Y;
			var height = Viewport.ViewportHeight;
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
