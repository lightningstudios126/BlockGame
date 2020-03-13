using Nez;
using System;

namespace BlockGame.Source.Extensions {
	public static class DirectionExt {
		public static Direction ShiftRight(this Direction direction) {
			return direction switch
			{
				Direction.Up => Direction.Right,
				Direction.Right => Direction.Down,
				Direction.Down => Direction.Left,
				Direction.Left => Direction.Up,
				_ => throw new ArgumentOutOfRangeException(nameof(direction), $"{direction}"),
			};
		}

		public static Direction ShiftLeft(this Direction direction) {
			return direction switch
			{
				Direction.Up => Direction.Left,
				Direction.Right => Direction.Up,
				Direction.Down => Direction.Right,
				Direction.Left => Direction.Down,
				_ => throw new ArgumentOutOfRangeException(nameof(direction), $"{direction}"),
			};
		}
	}
}
