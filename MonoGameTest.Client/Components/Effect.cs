using DefaultEcs;
using DefaultEcs.Command;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public struct Effect {
		public AnimatedSprite Sprite;

		public bool IsAnimating => Sprite.Animating;

		public Effect(Context context, SpriteLocation location) {
			Sprite = context.Resources.GetAnimatedSprite(location);
			Sprite.Origin = new Vector2(Sprite.Width, Sprite.Height) / 2;
			Sprite.OnAnimationLoop = OnEnd;
			Sprite.Play(location.Tag);
		}

		void OnEnd() {
			Sprite.Stop();
		}

		public static EntityRecord CreateEntity(
			Context context,
			SpriteLocation location,
			Coord coord
		) {
			var entity = context.Recorder.CreateEntity(context.World);
			entity.Set(new Effect(context, location));
			entity.Set(new Position { Coord = coord });
			return entity;
		}

	}

}
