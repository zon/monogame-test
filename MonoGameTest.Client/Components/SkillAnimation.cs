using System;
using DefaultEcs;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct SkillAnimation {
		public AnimatedSprite Attack;
		public Skill Skill;
		public Vector2 Forward;
		public float Rotation;
		public Entity Origin;
		public Entity? Target;
		public Coord TargetCoord;
		public CharacterState State;
		public float Timeout;

		public bool IsActive => Timeout > 0;
		public bool IsLeading => State == CharacterState.Lead;
		public float LeadProgress => (Skill.Lead - Timeout - Skill.Follow) / Skill.Lead;
		public float FollowProgress => (Skill.Follow - Timeout) / Skill.Follow;

		public SkillAnimation(Context context) {
			Attack = context.Resources.GetAnimatedSprite(SpriteFile.Attacks);
			Attack.Origin = new Vector2(Attack.Width, Attack.Height) / 2;
			Attack.Stop();

			Skill = default;
			Forward = default;
			Rotation = default;
			Origin = default;
			Target = default;
			TargetCoord = default;
			State = CharacterState.Charge;
			Timeout = default;
			
			Attack.OnAnimationLoop = OnEnd;
		}
		
		public void Start(
			Context context,
			in Entity origin,
			Coord target,
			Skill skill
		) {
			Skill = skill;
			Forward = new Vector2(1, 0);
			Rotation = 0;
			Origin = origin;
			TargetCoord = target;
			Timeout = skill.Duration;
			State = GetState();
			if (skill.Charge > 0 && skill.ChargeSprite.HasValue) {
				Attack.Play(skill.ChargeSprite.Value.Tag);
			} else {
				StartCast();
			}
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

			var state = GetState();
			if (State == CharacterState.Charge && state != State) {
				StartCast();
			}
			State = state;
			
			Attack.Update(dt);
		}

		void OnEnd() {
			if (State != CharacterState.Charge) {
				Attack.Stop();
			}
		}

		CharacterState GetState() {
			if (Skill.Charge > 0 && Timeout > Skill.Lead + Skill.Follow) {
				return CharacterState.Charge;
			} else if (Skill.Lead > 0 && Timeout > Skill.Follow) {
				return CharacterState.Lead;
			} else {
				return CharacterState.Follow;
			}
		}

		void StartCast() {
			ref var originPosition = ref Origin.Get<Position>();
			Forward = Vector2.Normalize((TargetCoord - originPosition.Coord).ToVector());
			Rotation = View.ToRadians(Forward);
			Attack.Play(Skill.CastSprite.Tag);
		}

	}

}
