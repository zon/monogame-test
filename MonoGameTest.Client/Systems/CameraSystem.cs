using DefaultEcs;
using DefaultEcs.System;

namespace MonoGameTest.Client {

	public class CameraSystem : AEntitySetSystem<float> {

		public CameraSystem(Context context) : base(context.World
			.GetEntities() 
			.With<Camera>()
			.AsSet()
		) {}

		protected override void Update(float dt, in Entity entity) {
			ref var camera = ref entity.Get<Camera>();

			if (camera.Center == camera.Target) return;

			camera.Center = Vector.Move(camera.Center, camera.Target, camera.Speed * dt);
			camera.View.LookAt(camera.Center + camera.Offset);
		}

	}

}
