using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;
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
			Context.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			Renderer.Draw(Context.WorldCamera.GetMatrix());
		}

		public void Dispose() {
			if (Renderer != null) Renderer.Dispose();
		}

	}

}
