using DefaultEcs;
using DefaultEcs.System;
using MonoGameTest.Common;

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
			ref var position = ref entity.Get<Position>();
			ref var camera = ref Context.WorldCamera.Get<Camera>();
			camera.Target = Context.CoordToMidVector(position);
		}

	}

}
