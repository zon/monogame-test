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

		public Effect(AsepriteDocument document, string animation) {
			Sprite = new AnimatedSprite(document);
			Sprite.Origin = new Vector2(Sprite.Width, Sprite.Height) / 2;
			Sprite.OnAnimationLoop = OnEnd;
			Sprite.Play(animation);
		}

		void OnEnd() {
			Sprite.Stop();
		}

		public static EntityRecord CreateEntity(Context context, string animation, Coord coord) {
			var entity = context.Recorder.CreateEntity(context.World);
			entity.Set(new Effect(context.Resources.Hits, animation));
			entity.Set(new Position { Coord = coord });
			return entity;
		}

	}

}
