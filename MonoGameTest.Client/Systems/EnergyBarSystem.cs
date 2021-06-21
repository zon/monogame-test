using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGameTest.Common;

namespace MonoGameTest.Client {

	public class EnergyBarSystem : AEntitySetSystem<float> {
		Context Context;

		SpriteBatch Batch => Context.UI;
		
		public EnergyBarSystem(Context context) : base(context.World
			.GetEntities()
			.With<Energy>()
			.AsSet()
		) {
			Context = context;
		}

		protected override void Update(float dt, in Entity entity) {
			ref var energy = ref entity.Get<Energy>();

			var req = new RectangleF(
				x: 0,
				y: View.SCREEN_HEIGHT - View.ENERGY_BAR_HEIGHT,
				width: View.WIDTH,
				height: View.ENERGY_BAR_HEIGHT
			);
			Batch.FillRectangle(req, Color.DarkBlue);

			req.Width *= energy.Percentage;
			Batch.FillRectangle(req, Color.LightBlue, 0.01f);
		}

	}

}
