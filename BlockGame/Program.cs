using System;

namespace BlockGameCore {
	public static class Program {
		[STAThread]
		static void Main() {
			using (var game = new BlockGame())
				game.Run();
		}
	}
}
