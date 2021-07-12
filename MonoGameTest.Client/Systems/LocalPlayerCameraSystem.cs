using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;

namespace MonoGameTest.Client {

	public class LocalPlayerCameraSystem : AEntitySetSystem<float> {
		readonly Context Context;

		public LocalPlayerCameraSystem(Context context) : base(context.World
			.GetEntities() 
			.With<LocalPlayer>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var sprite = ref entity.Get<Sprite>();
			var center = (
				sprite.Position +
				sprite.Rectangle.Size.ToVector2() / 2 +
				Vector2.UnitY * View.UI_HEIGHT / 2
			);
			Context.WorldCamera.LookAt(center);
		}

	}

}
