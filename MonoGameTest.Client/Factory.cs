using System;
using DefaultEcs;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite.Documents;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public static class Factory {

		public static Entity CreateCharacter(Context context, AddCharacterPacket packet) {
			var entity = context.World.CreateEntity();
			entity.Set(new CharacterId(packet.Id));
			var role = Role.Get(packet.RoleId);
			entity.Set(new Character(role));
			entity.Set((Group) packet.Group);
			if (packet.SessionId > 0) {
				entity.Set(new Player(packet.SessionId));
			}
			entity.Set(new Health { Maximum = packet.HealthMaximum, Amount = packet.HealthAmount });
			var coord = new Coord(packet.X, packet.Y);
			entity.Set(new Position { Coord = coord });
			var sprite = Sprite.Create(
				context.Resources.Characters,
				packet.Sprite,
				context.CoordToVector(coord)
			);
			sprite.Depth = Depths.Character;
			entity.Set(sprite);
			entity.Set(new MovementAnimation());
			entity.Set(new SkillAnimation(context.Resources.Skills));
			entity.Set(new HitAnimation(context.Resources.Hits));
			entity.Set(Bang.Create());

			if (packet.SessionId == context.SessionId) {
				entity.Set(new LocalPlayer());

				foreach (var be in context.Buttons.GetEntities()) {
					be.Dispose();
				}

				var i = 0;
				var iconsResource = context.Resources.SkillIcons;
				foreach (var skill in role.Skills) {
					var be = context.World.CreateEntity();
					var iconFrame = 0;
					AsepriteTag tag;
					if (iconsResource.Tags.TryGetValue(skill.Icon, out tag)) {
						iconFrame = tag.From;
					}
					be.Set(new Button {
						Index = i++,
						Skill = skill,
						IconFrame = iconFrame
					});
				}
			}

			return entity;
		}

		public static Entity CreateProjectile(Context context, Coord origin, Entity target, Skill skill) {
			var entity = context.World.CreateEntity();
			var projectile = new Projectile(origin, target, skill);
			entity.Set(projectile);
			entity.Set(new ProjectileView(context, projectile));
			return entity;
		}

	}

}
