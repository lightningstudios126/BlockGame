using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockGame.Source.Extensions {
	public static class DirectionExt {
		public static Direction ShiftRight(this Direction direction) {
			switch (direction) {
				case Direction.Up:
					return Direction.Right;
				case Direction.Right:
					return Direction.Down;
				case Direction.Down:
					return Direction.Left;
				case Direction.Left:
					return Direction.Up;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), $"{direction}");
			}
		}

		public static Direction ShiftLeft(this Direction direction) {
			switch (direction) {
				case Direction.Up:
					return Direction.Left;
				case Direction.Right:
					return Direction.Up;
				case Direction.Down:
					return Direction.Right;
				case Direction.Left:
					return Direction.Down;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), $"{direction}");
			}
		}
	}
}
