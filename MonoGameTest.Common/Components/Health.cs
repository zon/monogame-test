namespace MonoGameTest.Common {

	public struct Health {
		public int Maximum;
		public int Amount;

		public float Percentage => (float) Amount / Maximum;
		public bool IsFull => Amount >= Maximum;
		public bool IsEmpty => Amount <= 0;

		public Health(int maximum, int amount) {
			Maximum = maximum;
			Amount = amount;
		}

		public Health(int maximum) {
			Maximum = maximum;
			Amount = maximum;
		}

		public Health(Attributes attributes) {
			Maximum = attributes.Health;
			Amount = Maximum;
		}

	}

}
