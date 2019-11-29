namespace BlockGame.Source.Blocks {
	public static class Tetrominos {

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

		public static readonly PieceDefinition ITetromino = new PieceDefinition(
			new Tile(Constants.IColor),
			new (int, int)[] { (-1, 0), (0, 0), (1, 0), (2, 0) },
			iOffsets
		);
		public static readonly PieceDefinition OTetromino = new PieceDefinition(
			new Tile(Constants.OColor),
			new (int, int)[] { (0, 1), (0, 0), (1, 0), (1, 1) },
			oOffsets
		);
		public static readonly PieceDefinition LTetromino = new PieceDefinition(
			new Tile(Constants.LColor),
			new (int, int)[] { (-1, 0), (0, 0), (1, 0), (1, 1) },
			normalOffsets
		);
		public static readonly PieceDefinition JTetromino = new PieceDefinition(
			new Tile(Constants.JColor),
			new (int, int)[] { (-1, 1), (-1, 0), (0, 0), (1, 0) },
			normalOffsets
		);
		public static readonly PieceDefinition STetromino = new PieceDefinition(
			new Tile(Constants.SColor),
			new (int, int)[] { (-1, 0), (0, 0), (0, 1), (1, 1) },
			normalOffsets
		);
		public static readonly PieceDefinition ZTetromino = new PieceDefinition(
			new Tile(Constants.ZColor),
			new (int, int)[] { (-1, 1), (0, 1), (0, 0), (1, 0) },
			normalOffsets
		);
		public static readonly PieceDefinition TTetromino = new PieceDefinition(
			new Tile(Constants.TColor),
			new (int, int)[] { (-1, 0), (0, 0), (1, 0), (0, 1) },
			normalOffsets
		);

		public static readonly PieceDefinition[] tetrominos = new PieceDefinition[]
			{ ITetromino, OTetromino, LTetromino, JTetromino, STetromino, ZTetromino, TTetromino };
	}
}
