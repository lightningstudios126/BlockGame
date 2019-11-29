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
	class Playfield : RenderableComponent {
		int width, height, skyline;

		/// <summary>
		/// The data structure representing the contents of the Playfield.<br/>
		/// Index [x,0] corresponds to the bottom row of the matrix.<br/>
		/// Index [0,y] corresponds to the left most column of the matrix.<br/>
		/// </summary>
		Tile[,] grid;
		List<PlayerController> players;

		public event Action StartedProcessing;
		public event Action FinishedProcessing;

		Color backgroundColor = new Color(30, 30, 30);

		public Playfield(int width = Constants.standardWidth, int height = Constants.standardHeight, int skyline = 20) {
			this.width = width;
			this.height = height;
			this.skyline = skyline;
			this.grid = new Tile[width, height];

			this.players = new List<PlayerController>();
		}

		public TileGroup SpawnTileGroup(TileGroupDefinition def) {
			return SpawnTileGroup(def, Constants.defaultGenerationLocation);
		}

		public TileGroup SpawnTileGroup(TileGroupDefinition def, Point spawnLocation) {
			TileGroup group = new TileGroup(def, spawnLocation);
			group.playfield = this;
			return group;
		}

		public void AddPlayer(PlayerController player) {
			players.Add(player);
		}

		public Tile[] this[int i] => Enumerable.Range(0, width).Select(x => grid[x, i]).ToArray();

		/// <summary>Returns the indices of all the rows that are full</summary>
		public int[] FullRows => Enumerable.Range(0, height).Where(x => IsRowFull(x)).ToArray();

		/// <summary>Checks and returns whether the row number <paramref name="row"/> is completely full of tiles</summary>
		/// <param name="row">Row number to test</param>
		/// <returns>whether <paramref name="row"/> is full</returns>
		public bool IsRowFull(int row) => this[row].All(x => x != null);

		/// <summary>Checks the tile grid and returns true if the position <paramref name="p"/> is occupied by a tile (or tile group, if <paramref name="includeGroups"/> is true)</summary>
		/// <param name="p">Position to test</param>
		/// <param name="includeGroups">Should include tile groups when checking</param>
		/// <returns>whether the <paramref name="p"/> is occupied</returns>
		public bool IsPointIncluded(Point p, bool includeGroups = false) => grid[p.X, p.Y] != null || (includeGroups ? players.Select(x => x.activeGroup).Any(t => t.shape.Any(i => p == i)) : false);

		/// <summary>Returns true if the position <paramref name="p"/> is out of bounds defined by the Playfield's height and width</summary>
		/// <param name="p">Position to test</param>
		/// <returns>whether the <paramref name="p"/> is out of bounds</returns>
		public bool IsPointOutOfBounds(Point p) => !Mathf.Between(p.X, 0, width - 1) || !Mathf.Between(p.Y, 0, height - 1);

		/// <summary>
		/// Removes the tiles in <paramref name="rows"/> and drops down the rows above them
		/// </summary>
		/// <param name="rows">Rows to clear</param>
		public void ClearAndDropLines(params int[] rows) {
			if (rows.Any(x => x < 0 || x > height))
				throw new ArgumentOutOfRangeException(nameof(rows), $"A line index in {nameof(rows)} is invalid");

			var sorted = new Queue<int>(rows.OrderBy(x => x));
			for (int y = sorted.Peek(), offset = 0; y < height; y++) {
				while (sorted.Count > 0 && y + offset == sorted.Peek()) {
					sorted.Dequeue();
					offset++;
				}

				int takeFrom = y + offset;
				for (int x = 0; x < width; x++)
					grid[x, y] = takeFrom >= height ? null : grid[x, takeFrom];
			}
		}

		/// <summary>
		/// Adds the cells occupied by a tile group to the tile grid, then removes it from the internal list of tile groups
		/// </summary>
		/// <param name="group">Group to lock</param>
		public void LockTileGroup(TileGroup group) {
			foreach (var pos in group.shape) {
				var absPos = pos + group.position;
				grid[absPos.X, absPos.Y] = group.groupDef.type;
			}
		}

		public override float Height => Constants.pixelsPerTile * height;
		public override float Width => Constants.pixelsPerTile * width;
		public override void Render(Batcher batcher, Camera camera) {
			batcher.DrawCircle(Transform.Position, 2, Color.Red);
			batcher.DrawRect(new Rectangle(Transform.Position.RoundToPoint() - new Point(0, (height - 1) * Constants.pixelsPerTile), new Point(width * Constants.pixelsPerTile, height * Constants.pixelsPerTile)), backgroundColor);

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					DrawGridTile(batcher, new Point(x, y));
				}
			}

			foreach (var player in players) {
				var group = player.activeGroup;
				if (group != null) {
					var ghost = group.GetLandedOffset();
					foreach (Point point in group.shape) {
						DrawOutline(batcher, point + group.position, player.outlineTint, 5);
						DrawOutline(batcher, point + group.position + ghost, player.outlineTint, 5);
					}
					foreach (Point point in group.shape) {
						Utilities.DrawTile(batcher, point + group.position + ghost, Transform.Position.ToPoint(), group.groupDef.type, group.groupDef.type.ghostColor);
					}
					foreach (Point point in group.shape) {
						Utilities.DrawTile(batcher, point + group.position, Transform.Position.ToPoint(), group.groupDef.type);
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
				Enumerable.Range(0, height).Reverse().Select(
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
