using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockGame.Source.Extensions {
	public static class ColorExt {
		public static Color SetR(this Color color, byte r) {
			color.R = r;
			return color;
		}
		public static Color SetG(this Color color, byte g) {
			color.G = g;
			return color;
		}
		public static Color SetB(this Color color, byte b) {
			color.B = b;
			return color;
		}
		public static Color SetA(this Color color, byte a) {
			color.A = a;
			return color;
		}
	}
}
