using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockGame.Source.Extensions {
	public static class PointExt {
		public static Point Scale(this Point p, int scale) {
			p.X *= scale;
			p.Y *= scale;
			return p;
		}
	}
}
