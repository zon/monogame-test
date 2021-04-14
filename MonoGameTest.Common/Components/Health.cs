namespace MonoGameTest.Common {

	public struct Health {
		public int Maximum;
		public int Amount;

		public Health(int maximum, int amount) {
			Maximum = maximum;
			Amount = amount;
		}

		public Health(int maximum) {
			Maximum = maximum;
			Amount = maximum;
		}

	}

}
