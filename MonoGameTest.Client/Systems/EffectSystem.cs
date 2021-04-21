using System;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class EffectSystem : AEntitySetSystem<float> {
		readonly Context Context;

		SpriteBatch Batch => Context.Foreground;
		Camera Camera => Context.Camera;

		public EffectSystem(Context context) : base(context.World
			.GetEntities()
			.With<Effect>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var hit = ref entity.Get<Effect>();
			ref var position = ref entity.Get<Position>();

			hit.Sprite.Update(dt);
			if (!hit.IsAnimating) {
				Context.Recorder.Record(entity).Dispose();
				return;
			}

			hit.Sprite.Position = Context.CoordToMidVector(position.Coord);
			hit.Sprite.LayerDepth = Camera.Depth(hit.Sprite.Position, Depths.Bang);
			hit.Sprite.Render(Batch);
		}

	}

}
