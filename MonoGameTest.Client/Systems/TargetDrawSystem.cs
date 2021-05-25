using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest.Client {

	public class TargetDrawSystem : AEntitySetSystem<float> {
		readonly Context Context;
		readonly Texture2D Texture;
		readonly Rectangle Rectangle;
		readonly Vector2 Origin;

		public TargetDrawSystem(Context context) : base(context.World
			.GetEntities() 
			.With<LocalPlayer>()
			.AsSet()
		) {
			Context = context;
			Texture = Context.Resources.Hits.Texture;
			Rectangle = Context.Resources.Hits.Frames[14].ToRectangle();
			Origin = Rectangle.Size.ToVector2() / 2;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var localPlayer = ref entity.Get<LocalPlayer>();

			var other = localPlayer.Target?.Entity;
			if (other == null) return;

			ref var sprite = ref other.Value.Get<Sprite>();
			Context.Foreground.Draw(
				texture: Texture,
				position: sprite.Position + Context.HalfTileSize,
				sourceRectangle: Rectangle,
				color: Color.White,
				rotation: 0,
				origin: Origin,
				scale: Vector2.One,
				effects: SpriteEffects.None,
				layerDepth: Context.Camera.Depth(sprite.Position, Depths.Shadow)
			);
		}

	}

}
