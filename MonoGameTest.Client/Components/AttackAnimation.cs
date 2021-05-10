using System;
using DefaultEcs;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct AttackAnimation {
		public AnimatedSprite Sprite;
		public Attack Attack;
		public Vector2 Forward;
		public float Rotation;
		public Entity Origin;
		public Entity Target;
		public Coord TargetCoord;
		public float Timeout;

		public bool IsActive => Sprite.Animating;
		public bool IsLeading => Timeout > Attack.Follow;
		public float LeadProgress => (Attack.Lead - Timeout - Attack.Follow) / Attack.Lead;
		public float FollowProgress => (Attack.Follow - Timeout) / Attack.Follow;

		public AttackAnimation(AsepriteDocument document) {
			Sprite = new AnimatedSprite(document);
			Sprite.Origin = new Vector2(Sprite.Width, Sprite.Height) / 2;
			Sprite.Stop();
			Attack = default;
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
			Attack attack
		) {
			ref var originPosition = ref origin.Get<Position>();
			ref var targetPosition = ref target.Get<Position>();
			Attack = attack;
			Forward = Vector2.Normalize((targetPosition.Coord - originPosition.Coord).ToVector());
			Rotation = View.ToRadians(Forward);
			Origin = origin;
			Target = target;
			TargetCoord = targetPosition.Coord;
			Sprite.Play(attack.Animation);
			Timeout = attack.Duration;
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
