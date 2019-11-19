using System;

namespace BlockGame {
	public static class Program {
		[STAThread]
		static void Main() {
			using (var game = new BlockGame())
				game.Run();
		}
	}
}
