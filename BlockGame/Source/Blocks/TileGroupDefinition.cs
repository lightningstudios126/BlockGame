﻿using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Linq;

namespace BlockGame.Source.Blocks {
	public class TileGroupDefinition {
		// assume up direction
		public readonly Point[] shape;
		public readonly Tile type;
		public readonly Point[][] kickOffsets;

		public TileGroupDefinition(Tile type, (int x, int y)[] shape, (int x, int y)[][] kickOffsets) {
			this.shape = shape.Select(a => new Point(a.x, a.y)).ToArray();
			this.type = type;
			this.kickOffsets = kickOffsets.Select(a => a.Select(b => new Point(b.x, b.y)).ToArray()).ToArray();
		}

		/// <summary>
		/// Get the offset values associated with a certain direction.
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		public Point[] GetOffset(Direction direction) {
			switch (direction) {
				case Direction.Up:
					return kickOffsets[0];
				case Direction.Right:
					return kickOffsets[1];
				case Direction.Down:
					return kickOffsets[2];
				case Direction.Left:
					return kickOffsets[3];
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), $"{direction} is not a valid value for {nameof(direction)}");
			}
		}
	}
}