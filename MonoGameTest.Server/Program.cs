﻿using System;

namespace MonoGameTest.Server {

	class Program {

		static void Main(string[] args) {
			using (var game = new Game("first"))
				game.Run();
		}
		
	}

}
