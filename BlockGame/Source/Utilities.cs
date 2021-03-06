﻿using BlockGame.Source.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockGame.Source {
	public static class Utilities {
		public static void DrawTile(Batcher batcher, Point gridOffset, Point worldOffset, Tile tile) {
			Point offset = new Point(Constants.pixelsPerTile * gridOffset.X, -Constants.pixelsPerTile * gridOffset.Y);
			Point totalOffset = worldOffset + offset;
			var texture = Core.Scene.Content.LoadTexture(tile.spriteLocation);
			batcher.Draw(texture, new Rectangle(totalOffset.X, totalOffset.Y, Constants.pixelsPerTile, Constants.pixelsPerTile), tile.color);
		}
		public static void DrawTile(Batcher batcher, Point gridOffset, Point worldOffset, Tile tile, Color tint) {
			Point offset = new Point(Constants.pixelsPerTile * gridOffset.X, -Constants.pixelsPerTile * gridOffset.Y);
			Point totalOffset = worldOffset + offset;
			var texture = Core.Scene.Content.LoadTexture(tile.spriteLocation);
			batcher.Draw(texture, new Rectangle(totalOffset.X, totalOffset.Y, Constants.pixelsPerTile, Constants.pixelsPerTile), tint);
		}

		public static Point[] FindOutline(Point[] shape) {
			// get all the corners of the blocks that make up the shape
			HashSet<Point> allVertices = new HashSet<Point>(shape.SelectMany(x =>
				new Point[] { x, x + new Point(1, 0), x + new Point(0, -1), x + new Point(1, -1) }));
			// if a vertex is shared by an odd number of blocks, then it is part of the outline
			List<Point> included = new List<Point>();
			foreach (var v in allVertices) {
				byte arrange = 0;
				byte count = 0;
				if (shape.Contains(v)) {
					arrange |= 1;
					count++;
				}
				if (shape.Contains(v + new Point(-1, 0))) {
					arrange |= 2;
					count++;
				}
				if (shape.Contains(v + new Point(0, 1))) {
					arrange |= 4;
					count++;
				}
				if (shape.Contains(v + new Point(-1, 1))) {
					arrange |= 8;
					count++;
				}

				if (count == 1 || count == 3) included.Add(v);
				else if (arrange == 0b0110 || arrange == 0b1001) included.Add(v);
			}

			return included.ToArray();
		}
	}
}
