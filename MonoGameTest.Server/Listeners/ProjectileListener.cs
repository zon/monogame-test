using System;
using DefaultEcs;
using MonoGameTest.Common;

namespace MonoGameTest.Server {

	public class ProjectileListener : IDisposable {
		readonly Server Server;
		readonly IDisposable AddedListener;

		public ProjectileListener(Context context) {
			Server = context.Server;
			var world = context.World;
			AddedListener = world.SubscribeComponentAdded<Projectile>(OnAdd);
		}

		public void Dispose() {
			AddedListener.Dispose();
		}

		void OnAdd(in Entity entity, in Projectile projectile) {
			ref var targetCharacterId = ref projectile.Target.Get<CharacterId>();
			Server.SendToAll(new ProjectilePacket {
				OriginX = projectile.Origin.X,
				OriginY = projectile.Origin.Y,
				TargetCharacterId = targetCharacterId.Id,
				SkillId = projectile.Skill.Id
			});
		}

	}

}
