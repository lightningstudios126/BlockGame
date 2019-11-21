using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlockGame.Source.Blocks;
using Microsoft.Xna.Framework;
using Nez;
using Nez.AI.FSM;
using Nez.Textures;

namespace BlockGame.Source.Components {
	/// <summary>
	/// For a standard field height, row 20 is the highest row fully visible.<br/>
	/// Row 21 should be only partially displayed.<br/>
	/// </summary>
	class Playfield : RenderableComponent, IUpdatable {
		int width, height;

		/// <summary>
		/// The data structure representing the contents of the Playfield.<br/>
		/// Index [x,0] corresponds to the bottom row of the matrix.<br/>
		/// Index [0,y] corresponds to the left most column of the matrix.<br/>
		/// </summary>
		Tile[,] grid;
		List<PlayerController> playerControllers;
		List<TileGroup> tileGroups;

		StateMachine<Playfield> stateMachine;

		public Playfield(int width = Constants.standardWidth, int height = Constants.standardHeight) {
			this.width = width;
			this.height = height;
			this.grid = new Tile[width, height];

			this.playerControllers = new List<PlayerController>();
			this.tileGroups = new List<TileGroup>();
		}

		public override void OnAddedToEntity() {
			stateMachine = new StateMachine<Playfield>(this, new States.StateGameplay());
		}

		// HACK: properly integrate state machine in game for potential multiplayer
		public void Update() {
			if (FullRows.Length > 0)
				ClearAndDropLines(FullRows);
		}

		public TileGroup AddGroup(TileGroupDefinition def) {
			TileGroup group = new TileGroup(def, Constants.defaultGenerationLocation);
			this.AddGroup(group);
			return group;
		}

		public void AddGroup(TileGroup group) {
			tileGroups.Add(group);
			group.playfield = this;
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
		public bool IsPointIncluded(Point p, bool includeGroups = false) => grid[p.X, p.Y] != null || (includeGroups ? tileGroups.Any(t => t.shape.Any(i => p == i)) : false);

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
			tileGroups.Remove(group);
			foreach (var pos in group.shape) {
				var absPos = pos + group.position;
				grid[absPos.X, absPos.Y] = group.groupDef.type;
			}

			group.groupDef.type.color = Nez.Random.NextColor();
		}

		public override float Height => Constants.pixelsPerTile * height;
		public override float Width => Constants.pixelsPerTile * width;
		public override void Render(Batcher batcher, Camera camera) {
			batcher.DrawCircle(Transform.Position, 2, Color.Red);
			batcher.DrawRect(new Rectangle(Transform.Position.RoundToPoint() - new Point(0, (height - 1) * Constants.pixelsPerTile), new Point(width * Constants.pixelsPerTile, height * Constants.pixelsPerTile)), Color.MediumPurple);

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					DrawTile(batcher, new Point(x, y));
				}
			}

			foreach (var group in tileGroups) {
				var ghost = group.GetLandedOffset();
				foreach (Point point in group.shape) {
					DrawTile(batcher, point + group.position, group.groupDef.type);
					DrawTile(batcher, point + group.position + ghost, group.groupDef.type, true);
				}
			}
		}

		public void DrawTile(Batcher batcher, Point gridLocation) {
			Tile tile = grid[gridLocation.X, gridLocation.Y];
			if (tile != null) DrawTile(batcher, gridLocation, tile);
		}

		public void DrawTile(Batcher batcher, Point gridLocation, Tile tile, bool ghost = false) {
			Point offset = new Point(Constants.pixelsPerTile * gridLocation.X, -Constants.pixelsPerTile * gridLocation.Y);
			var texture = Entity.Scene.Content.LoadTexture(tile.spriteLocation);
			batcher.Draw(texture, new Rectangle((offset.ToVector2() + Transform.Position).RoundToPoint(), new Point(Constants.pixelsPerTile)), ghost ? tile.ghostColor : tile.color);
		}

		public override string ToString() {
			return string.Join("\n",
				Enumerable.Range(0, height).Reverse().Select(
					y => Enumerable.Range(0, width).Reverse().Select(x => grid[x, y])
				).Select(x => string.Join(" ", x)));
		}

		public void StartSequence() {

		}


		// do i really need a state machine?
		// the sequence is entirely linear
		private static class States {
			public enum PlayfieldState {
				Gameplay, // controllers have control
				Locked, // control is passed to the playfield
				Match, // check matrix for patterns and mark delete
				Animate, // fancy visual effects stuff
				Clear, // delete blocks and update score, separate?
				Complete // update information, then pass control back to controllers
			}

			public class StateGameplay : State<Playfield> {
				public override void Update(float deltaTime) {
					throw new NotImplementedException();
				}
			}
		}
	}
}
