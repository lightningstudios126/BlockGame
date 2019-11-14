using BlockGame.Source.Components;
using BlockGame.Source.Extensions;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Linq;

namespace BlockGame.Source.Blocks {
	class TileGroup {
		public TileGroupDefinition groupDef;
		public Point[] shape;
		public Point position;
		public Direction facing;

		public Playfield playfield;

		public TileGroup(TileGroupDefinition groupDef, Point position, Direction facing = Direction.Up) {
			this.groupDef = groupDef;
			this.position = position;
			this.facing = facing;

			this.shape = new Point[groupDef.shape.Length];
			Array.Copy(groupDef.shape, shape, groupDef.shape.Length);
		}

		/// <summary>
		/// Returns true when there is no associated playfield or there are no intersecting tiles
		/// </summary>
		/// <param name="testShape">The shape to test on</param>
		/// <param name="relOffset">Translation relative to (0, 0)</param>
		/// <returns>whether the offset puts the block in a valid location</returns>
		public bool TestOffset(Point[] testShape, Point relOffset) {
			Func<Point, bool> inBounds = p => !playfield.IsPointOutOfBounds(position + relOffset + p)
				&& !playfield.IsPointIncluded(position + relOffset + p);
			return this.playfield == null || testShape.All(inBounds);
		}

		/// <summary>
		/// Returns true if there is no associated playfield or there are no intersecting tiles
		/// </summary>
		/// <param name="relOffset">Translation relative to (0, 0)</param>
		/// <returns>whether the offset puts the block in a valid location</returns>
		public bool TestOffset(Point relOffset) {
			return TestOffset(shape, relOffset);
		}

		/// <summary>
		/// Tests each offset in <paramref name="offsets"/> using the shape defined by <paramref name="testShape"/> and returns the first valid offset or <see langword="null"/>, if none are valid
		/// </summary>
		/// <param name="testShape">Shape to test on</param>
		/// <param name="offsets">Offsets to test</param>
		/// <returns>first valid offset or <see langword="null"/></returns>
		public Point? TestKickOffsets(Point[] testShape, Point[] offsets) {
			foreach (var relOffset in offsets)
				if (TestOffset(testShape, relOffset))
					return relOffset;
			return null;
		}

		// TODO: see about deduplicating code between RotateLeft and RotateRight?
		public bool RotateLeft() {
			// [x, y] -> [-y, x]
			// rotate given shape widdershins around (0, 0)
			Point[] testShape = shape.Select(a => new Point(-a.Y, a.X)).ToArray();
			Direction newDir = facing.ShiftLeft();
			// get relevant SRS offsets from kick table
			Point[] offsets = Enumerable.Zip(groupDef.GetOffset(facing), groupDef.GetOffset(newDir), (a, b) => a - b).ToArray();
			Point? offset = TestKickOffsets(testShape, offsets);
			if (offset.HasValue) {
				shape = testShape;
				facing = newDir;
				position += offset.Value;
				return true;
			} else return false;
		}

		public bool RotateRight() {
			// [x, y] -> [y, -x]
			// rotate given shape clockwise around (0, 0)
			var testShape = shape.Select(a => new Point(a.Y, -a.X)).ToArray();
			var newDir = facing.ShiftRight();
			// get relevant SRS offsets from kick table
			var offsets = Enumerable.Zip(groupDef.GetOffset(facing), groupDef.GetOffset(newDir), (a, b) => a - b).ToArray();
			var offset = TestKickOffsets(testShape, offsets);
			if (offset.HasValue) {
				shape = testShape;
				facing = newDir;
				position += offset.Value;
				return true;
			} else return false;
		}

		/// <summary>
		/// Tests a offset and performs the translation, provided <paramref name="simulate"/> is <see langword="false"/>
		/// </summary>
		/// <param name="offset">Offset to test</param>
		/// <param name="simulate">Prevent translation if the offset is valid</param>
		/// <returns>whether the offset is valid</returns>
		public bool Shift(Point offset, bool simulate = false) {
			if (TestOffset(offset)) {
				if (!simulate) position += offset;
				return true;
			} else return false;
		}

		public bool MoveDown(bool simulate = false) {
			return Shift(new Point(0, -1), simulate);
		}

		public bool MoveRight(bool simulate = false) {
			return Shift(new Point(1, 0), simulate);
		}

		public bool MoveLeft(bool simulate = false) {
			return Shift(new Point(-1, 0), simulate);
		}

		public Point GetGhostPosition () {
			int downOffset = 0;
			while (TestOffset(new Point(0, downOffset)))
				downOffset--;
			return new Point(0, downOffset + 1);
		}
	}
}
