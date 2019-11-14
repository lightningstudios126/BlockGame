using Nez.Textures;

namespace BlockGame.Source.Blocks {
	public static class Tetrominos {

		public static string spriteLocation = "";

		static (int, int)[][] normalOffsets = new (int, int)[][] {
			new (int, int)[] { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
			new (int, int)[] { (0, 0), (1, 0), (1, -1), (0, +2), (1, 2) },
			new (int, int)[] { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
			new (int, int)[] { (0, 0), (-1, 0), (-1, -1), (0, 2), (-1, 2) },
		};

		static (int, int)[][] iOffsets = new (int, int)[][] {
			new (int, int)[] { (0, 0), (-1, 0), (2, 0), (-1, 0), (2, 0) },
			new (int, int)[] { (-1, 0), (0, 0), (0, 0), (0, 1), (0, -2) },
			new (int, int)[] { (-1, 1), (1, 1), (-2, 1), (1, 0), (-2, 0) },
			new (int, int)[] { (0, 1), (0, 1), (0, 1), (0, -1), (0, 2) },
		};

		static (int, int)[][] oOffsets = new (int, int)[][] {
			new (int, int)[] { (0, 0) },
			new (int, int)[] { (0, -1) },
			new (int, int)[] { (-1, -1) },
			new (int, int)[] { (-1, 0) },
		};

		public static readonly TileGroupDefinition ITetromino = new TileGroupDefinition(
			new Tile(Constants.IColor),
			new (int, int)[] { (-1, 0), (0, 0), (1, 0), (2, 0) },
			iOffsets
		);
		public static readonly TileGroupDefinition OTetromino = new TileGroupDefinition(
			new Tile(Constants.OColor),
			new (int, int)[] { (0, 1), (0, 0), (1, 0), (1, 1) },
			oOffsets
		);
		public static readonly TileGroupDefinition LTetromino = new TileGroupDefinition(
			new Tile(Constants.LColor),
			new (int, int)[] { (-1, 0), (0, 0), (1, 0), (1, 1) },
			normalOffsets
		);
		public static readonly TileGroupDefinition JTetromino = new TileGroupDefinition(
			new Tile(Constants.JColor),
			new (int, int)[] { (-1, 1), (-1, 0), (0, 0), (1, 0) },
			normalOffsets
		);
		public static readonly TileGroupDefinition STetromino = new TileGroupDefinition(
			new Tile(Constants.SColor),
			new (int, int)[] { (-1, 0), (0, 0), (0, 1), (1, 1) },
			normalOffsets
		);
		public static readonly TileGroupDefinition ZTetromino = new TileGroupDefinition(
			new Tile(Constants.ZColor),
			new (int, int)[] { (-1, 1), (0, 1), (0, 0), (1, 0) },
			normalOffsets
		);
		public static readonly TileGroupDefinition TTetromino = new TileGroupDefinition(
			new Tile(Constants.TColor),
			new (int, int)[] { (-1, 0), (0, 0), (1, 0), (0, 1) },
			normalOffsets
		);

		public static readonly TileGroupDefinition[] tetrominos = new TileGroupDefinition[]
			{ ITetromino, OTetromino, LTetromino, JTetromino, STetromino, ZTetromino, TTetromino };
	}
}
