using Microsoft.Xna.Framework;
using MonoGameTest.Client;
using Xunit;

namespace MonoGameTest.Test {

	public class ViewTest {

		const int PRECISON = 6;

		[Fact]
		public void RightVectorToRadians() {
			var r = View.ToRadians(new Vector2(1, 0));
			Assert.Equal(0, r);
		}

		[Fact]
		public void LeftVectorToRadians() {
			var r = View.ToRadians(new Vector2(-1, 0));
			Assert.Equal(View.RADIAN / 2, r);
		}

		[Fact]
		public void DownVectorToRadians() {
			var r = View.ToRadians(new Vector2(0, 1));
			Assert.Equal(View.RADIAN / 4, r);
		}

		[Fact]
		public void UpVectorToRadians() {
			var r = View.ToRadians(new Vector2(0, -1));
			Assert.Equal(View.RADIAN / -4, r);
		}

		[Fact]
		public void RightRadiansToVector() {
			var v = View.ToVector(0);
			Assert.Equal(1, v.X);
			Assert.Equal(0, v.Y);
		}

		[Fact]
		public void LeftRadiansToVector() {
			var v = View.ToVector(View.RADIAN / 2);
			Assert.Equal(-1, v.X, PRECISON);
			Assert.Equal(0, v.Y, PRECISON);
		}

		[Fact]
		public void DownRadiansToVector() {
			var v = View.ToVector(View.RADIAN / 4);
			Assert.Equal(0, v.X, PRECISON);
			Assert.Equal(1, v.Y);
		}

		[Fact]
		public void UpRadiansToVector() {
			var v = View.ToVector(View.RADIAN / -4);
			Assert.Equal(0, v.X, PRECISON);
			Assert.Equal(-1, v.Y);
		}

	}
	
}
