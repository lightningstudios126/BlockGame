using BlockGame.Source.Blocks;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockGame.Source {
	class Utilities {
		public static void DrawTile(Batcher batcher, Point gridOffset, Point worldOffset, Tile tile) {
			Point offset = new Point(Constants.pixelsPerTile * gridOffset.X, -Constants.pixelsPerTile * gridOffset.Y);
			var texture = Core.Scene.Content.LoadTexture(tile.spriteLocation);
			batcher.Draw(texture, new Rectangle(worldOffset + offset, new Point(Constants.pixelsPerTile)), tile.color);
		}
		public static void DrawTile(Batcher batcher, Point gridOffset, Point worldOffset, Tile tile, Color tint) {
			Point offset = new Point(Constants.pixelsPerTile * gridOffset.X, -Constants.pixelsPerTile * gridOffset.Y);
			var texture = Core.Scene.Content.LoadTexture(tile.spriteLocation);
			batcher.Draw(texture, new Rectangle(worldOffset + offset, new Point(Constants.pixelsPerTile)), tint);
		}
	}
}
