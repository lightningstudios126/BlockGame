using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockGame.Source {
	public static class Constants {
		public const int standardWidth = 10;
		public const int standardHeight = 40;
		public const int pixelsPerTile = 32;
		public const float sixtieth = 1 / 60f;

		public static readonly Color IColor = Color.Aqua;
		public static readonly Color TColor = Color.Magenta;
		public static readonly Color JColor = Color.DodgerBlue;
		public static readonly Color LColor = Color.Orange;
		public static readonly Color SColor = Color.Lime;
		public static readonly Color ZColor = Color.Red;
		public static readonly Color OColor = Color.Yellow;

		public static readonly Point defaultGenerationLocation = new Point(4, 20);
	}
}
