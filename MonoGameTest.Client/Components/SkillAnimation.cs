using System;
using DefaultEcs;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct SkillAnimation {
		public AnimatedSprite Sprite;
		public Skill Skill;
		public Vector2 Forward;
		public float Rotation;
		public Entity Origin;
		public Entity Target;
		public Coord TargetCoord;
		public float Timeout;

		public bool IsActive => Sprite.Animating;
		public bool IsLeading => Timeout > Skill.Follow;
		public float LeadProgress => (Skill.Lead - Timeout - Skill.Follow) / Skill.Lead;
		public float FollowProgress => (Skill.Follow - Timeout) / Skill.Follow;

		public SkillAnimation(AsepriteDocument document) {
			Sprite = new AnimatedSprite(document);
			Sprite.Origin = new Vector2(Sprite.Width, Sprite.Height) / 2;
			Sprite.Stop();
			Skill = default;
			Forward = default;
			Rotation = default;
			Origin = default;
			Target = default;
			TargetCoord = default;
			Timeout = default;
			Sprite.OnAnimationLoop = OnEnd;
		}
		
		public void Start(
			Context context,
			in Entity origin,
			Coord target,
			Skill skill
		) {
			ref var originPosition = ref origin.Get<Position>();
			Skill = skill;
			Forward = Vector2.Normalize((target - originPosition.Coord).ToVector());
			Rotation = View.ToRadians(Forward);
			Origin = origin;
			TargetCoord = target;
			Sprite.Play(skill.Animation);
			Timeout = skill.Duration;
		}
		
		public void Start(
			Context context,
			in Entity origin,
			in Entity target,
			Skill skill
		) {
			Target = target;
			ref var targetPosition = ref target.Get<Position>();
			Start(context, origin, targetPosition.Coord, skill);
		}

		public void Update(float dt) {
			Timeout = Math.Max(Timeout - dt, 0);

			if (Target != null && Target.IsAlive) {
				ref var targetPosition = ref Target.Get<Position>();
				TargetCoord = targetPosition.Coord;
			}
			
			Sprite.Update(dt);
		}

		void OnEnd() {
			Sprite.Stop();
		}

	}

}
