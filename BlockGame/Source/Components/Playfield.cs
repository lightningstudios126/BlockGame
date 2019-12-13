using System;
using System.Collections.Generic;
using System.Linq;
using BlockGame.Source.Blocks;
using Microsoft.Xna.Framework;
using Nez;

namespace BlockGame.Source.Components {
	/// <summary>
	/// For a standard field height, row 20 is the highest row fully visible.<br/>
	/// Row 21 should be only partially displayed.<br/>
	/// </summary>
	class Playfield : UIPanel {
		int width, height, buffer;
		int fullHeight => height + buffer;

		/// <summary>
		/// The data structure representing the contents of the Playfield.<br/>
		/// Index [x,0] corresponds to the bottom row of the matrix.<br/>
		/// Index [0,y] corresponds to the left most column of the matrix.<br/>
		/// </summary>
		Tile[,] grid;
		List<PlayerController> players;

		public event Action StartedProcessing;
		public event Action FinishedProcessing;

		public Playfield(int width = Constants.standardWidth, int height = Constants.standardHeight, int buffer = Constants.standardHeight) {
			this.width = width;
			this.height = height;
			this.buffer = buffer;
			this.grid = new Tile[width, fullHeight];
			this.players = new List<PlayerController>();

			this.BackgroundColour = new Color(40, 40, 40);
		}

		public Piece SpawnTileGroup(PieceDefinition def) {
			return SpawnTileGroup(def, Constants.defaultGenerationLocation);
		}

		public Piece SpawnTileGroup(PieceDefinition def, Point spawnLocation) {
			Piece group = new Piece(def, spawnLocation);
			group.playfield = this;
			return group;
		}

		public void AddPlayer(PlayerController player) {
			players.Add(player);
		}

		public Tile[] this[int i] => Enumerable.Range(0, width).Select(x => grid[x, i]).ToArray();

		/// <summary>Returns the indices of all the rows that are full</summary>
		public int[] FullRows => Enumerable.Range(0, fullHeight).Where(x => IsRowFull(x)).ToArray();

		/// <summary>Checks and returns whether the row number <paramref name="row"/> is completely full of tiles</summary>
		/// <param name="row">Row number to test</param>
		/// <returns>whether <paramref name="row"/> is full</returns>
		public bool IsRowFull(int row) => this[row].All(x => x != null);

		/// <summary>Checks the tile grid and returns true if the position <paramref name="p"/> is occupied by a tile (or piece, if <paramref name="includeGroups"/> is true)</summary>
		/// <param name="p">Position to test</param>
		/// <param name="includeGroups">Should include tile groups when checking</param>
		/// <returns>whether the <paramref name="p"/> is occupied</returns>
		public bool IsPointIncluded(Point p, bool includeGroups = false) => grid[p.X, p.Y] != null || (includeGroups ? players.Select(x => x.piece).Any(t => t.shape.Any(i => p == i)) : false);

		/// <summary>Returns true if the position <paramref name="p"/> is out of bounds defined by the Playfield's full height and width</summary>
		/// <param name="p">Position to test</param>
		/// <returns>whether the <paramref name="p"/> is out of bounds</returns>
		public bool IsPointOutOfBounds(Point p) => !Mathf.Between(p.X, 0, width - 1) || !Mathf.Between(p.Y, 0, fullHeight - 1);

		/// <summary>
		/// Removes the tiles in <paramref name="rows"/> and drops down the rows above them
		/// </summary>
		/// <param name="rows">Rows to clear</param>
		public void ClearAndDropLines(params int[] rows) {
			if (rows.Any(x => x < 0 || x > fullHeight))
				throw new ArgumentOutOfRangeException(nameof(rows), $"A line index in {nameof(rows)} is invalid");

			var sorted = new Queue<int>(rows.OrderBy(x => x));
			for (int y = sorted.Peek(), offset = 0; y < fullHeight; y++) {
				while (sorted.Count > 0 && y + offset == sorted.Peek()) {
					sorted.Dequeue();
					offset++;
				}

				int takeFrom = y + offset;
				for (int x = 0; x < width; x++)
					grid[x, y] = takeFrom >= fullHeight ? null : grid[x, takeFrom];
			}
		}

		/// <summary>
		/// Adds the cells occupied by a tile group to the tile grid, then removes it from the internal list of tile groups
		/// </summary>
		/// <param name="group">Group to lock</param>
		public void LockTileGroup(Piece group) {
			foreach (var pos in group.shape) {
				var absPos = pos + group.position;
				grid[absPos.X, absPos.Y] = group.definition.type;
			}
		}

		public override float Height => Constants.pixelsPerTile * (height + 1);
		public override float Width => Constants.pixelsPerTile * width;
		public override void Render(Batcher batcher, Camera camera) {
			batcher.DrawCircle(Transform.Position, 2, Color.Red);
			batcher.DrawHollowRect((Transform.Position.RoundToPoint() - new Point(0, height * Constants.pixelsPerTile)).ToVector2(), Width, Height, OutlineColour, 6);
			batcher.DrawRect((Transform.Position.RoundToPoint() - new Point(0, height * Constants.pixelsPerTile)).ToVector2(),
				Width, Height, BackgroundColour);

			for (int y = 0; y < height + 1; y++) {
				for (int x = 0; x < width; x++) {
					DrawGridTile(batcher, new Point(x, y));
				}
			}

			foreach (var player in players) {
				var piece = player.piece;
				if (piece != null) {
					var ghost = piece.GetLandedOffset();
					foreach (Point point in piece.shape) {
						DrawOutline(batcher, point + piece.position, player.outlineTint, 6);
						DrawOutline(batcher, point + piece.position + ghost, player.outlineTint, 6);
					}
					foreach (Point point in piece.shape) {
						Utilities.DrawTile(batcher, point + piece.position + ghost, Transform.Position.ToPoint(), piece.definition.type, piece.definition.type.ghostColor);
					}
					foreach (Point point in piece.shape) {
						Utilities.DrawTile(batcher, point + piece.position, Transform.Position.ToPoint(), piece.definition.type);
					}
				}
			}
		}

		public void DrawGridTile(Batcher batcher, Point gridLocation) {
			Tile tile = grid[gridLocation.X, gridLocation.Y];
			if (tile != null)
				Utilities.DrawTile(batcher, gridLocation, Transform.Position.ToPoint(), tile);
		}

		public void DrawOutline(Batcher batcher, Point gridLocation, Color color, int thickness = 1) {
			Point offset = new Point(Constants.pixelsPerTile * gridLocation.X, -Constants.pixelsPerTile * gridLocation.Y);
			batcher.DrawHollowRect(new Rectangle((offset.ToVector2() + Transform.Position).RoundToPoint(), new Point(Constants.pixelsPerTile)), color, thickness);
		}

		public override string ToString() {
			return string.Join("\n",
				Enumerable.Range(0, fullHeight).Reverse().Select(
					y => Enumerable.Range(0, width).Reverse().Select(x => grid[x, y])
				).Select(x => string.Join(" ", x)));
		}

		public enum PlayfieldState {
			Gameplay, // controllers have control
			Locked, // control is passed to the playfield
			Match, // check matrix for patterns and mark delete, instant
			Animate, // fancy visual effects stuff, delayed
			Clear, // delete blocks and update score, separate?, instant
			Complete // update information, then pass control back to controllers
		}

		public void StartSequence() {
			StartedProcessing();
			if (FullRows.Length > 0)
				ClearAndDropLines(FullRows);
			FinishedProcessing();
		}
	}
}
