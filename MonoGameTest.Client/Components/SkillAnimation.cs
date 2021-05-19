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
			in Entity target,
			Skill skill
		) {
			ref var originPosition = ref origin.Get<Position>();
			ref var targetPosition = ref target.Get<Position>();
			Skill = skill;
			Forward = Vector2.Normalize((targetPosition.Coord - originPosition.Coord).ToVector());
			Rotation = View.ToRadians(Forward);
			Origin = origin;
			Target = target;
			TargetCoord = targetPosition.Coord;
			Sprite.Play(skill.Animation);
			Timeout = skill.Duration;
		}

		public void Update(float dt) {
			Timeout = Math.Max(Timeout - dt, 0);
			Sprite.Update(dt);
		}

		void OnEnd() {
			Sprite.Stop();
		}

	}

}
