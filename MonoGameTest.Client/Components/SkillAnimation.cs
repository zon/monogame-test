using System;
using DefaultEcs;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct SkillAnimation {
		public AnimatedSprite Effects;
		public AnimatedSprite EffectsLarge;
		public Skill Skill;
		public Vector2 Forward;
		public float Rotation;
		public Entity Origin;
		public Entity? Target;
		public Coord TargetCoord;
		public float Timeout;

		public bool IsActive => Effects.Animating;
		public bool IsLeading => Timeout > Skill.Follow;
		public float LeadProgress => (Skill.Lead - Timeout - Skill.Follow) / Skill.Lead;
		public float FollowProgress => (Skill.Follow - Timeout) / Skill.Follow;

		public SkillAnimation(Context context) {
			Effects = context.Resources.GetAnimatedSprite(SpriteFile.Attacks);
			Effects.Origin = new Vector2(Effects.Width, Effects.Height) / 2;
			Effects.Stop();

			EffectsLarge = context.Resources.GetAnimatedSprite(SpriteFile.Effects);
			EffectsLarge.Origin = new Vector2(EffectsLarge.Width, EffectsLarge.Height) / 2;
			EffectsLarge.Stop();

			Skill = default;
			Forward = default;
			Rotation = default;
			Origin = default;
			Target = default;
			TargetCoord = default;
			Timeout = default;
			
			Effects.OnAnimationLoop = OnEnd;
			EffectsLarge.OnAnimationLoop = OnEnd;
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
			GetSprite().Play(skill.CastSprite.Tag);
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

			if (Target.HasValue && Target.Value.IsAlive) {
				ref var targetPosition = ref Target.Value.Get<Position>();
				TargetCoord = targetPosition.Coord;
			}
			
			GetSprite().Update(dt);
		}

		void OnEnd() {
			Effects.Stop();
			EffectsLarge.Stop();
		}

		AnimatedSprite GetSprite() {
			switch (Skill.CastSprite.File) {
				case SpriteFile.Effects:
					return EffectsLarge;
				default:
					return Effects;
			}
		}

	}

}
