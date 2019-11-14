using Microsoft.Xna.Framework;
using Nez.Textures;

namespace BlockGame.Source.Blocks {
	public class Tile {
		public string spriteLocation;
		public Color color;
		public Color ghostColor => Color.Multiply(color, 0.5f);

		public Tile(Color color, string spriteLocation = "Content/Sprites/tile.png") {
			this.color = color;
			this.spriteLocation = spriteLocation;
		}
	}
}
