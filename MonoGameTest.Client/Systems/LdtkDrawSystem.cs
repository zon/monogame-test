using DefaultEcs.System;
using ldtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest.Client {

	public class LdtkDrawSystem : ISystem<float> {
		readonly Context Context;

		LayerDefinition Collisions;
		TilesetDefinition Tileset;
		Texture2D Texture;

		public bool IsEnabled { get; set; }

		public LdtkDrawSystem(Context context) {
			Context = context;
			IsEnabled = true;
		}

		public void Update(float state) {
			if (!Context.IsReady) return;

			var world = Context.LevelResources.World;
			var matrix = Context.WorldCameraView.GetMatrix();
			var batch = Context.WorldBatch;

			if (Collisions == null) Collisions = world.GetLayerDefinition("Collisions");
			if (Tileset == null) Tileset = world.GetTileset(Collisions.AutoTilesetDefUid.Value);
			if (Texture == null) Texture = Context.LevelResources.GetTilesetSprite(Tileset).Texture;

			var tileSize = new Point((int) Tileset.TileGridSize, (int) Tileset.TileGridSize);

			batch.Begin(transformMatrix: matrix, samplerState: SamplerState.PointClamp);
			foreach (var level in world.Json.Levels) {
				var offset = new Point((int) level.WorldX, (int) level.WorldY);
				foreach (var layer in level.LayerInstances) {
					if (layer.LayerDefUid != Collisions.Uid) continue;
					foreach (var tile in layer.AutoLayerTiles) {
						batch.Draw(
							texture: Texture,
							position: (View.ToPoint(tile.Px) + offset).ToVector2(),
							sourceRectangle: new Rectangle(View.ToPoint(tile.Src), tileSize),
							color: Color.White,
							rotation: 0,
							origin: Vector2.Zero,
							scale: Vector2.One,
							effects: SpriteEffects.None,
							layerDepth: 0
						);
					}
				}
			}
			batch.End();
		}

		public void Dispose() {}

	}

}
