using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGameTest.Common;

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
			ref var target = ref entity.Get<Target>();
			if (!target.HasEntity) return;
			var other = target.Entity.Value;
			ref var sprite = ref other.Get<Sprite>();
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
